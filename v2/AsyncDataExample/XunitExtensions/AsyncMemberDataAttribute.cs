using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Xunit;

// We had to copy most of this code from MemberDataAttributeBase because the visibility was wrong with some
// things and/or we were missing virtuals in required places.

[DataDiscoverer("Xunit.Sdk.AsyncDataDiscoverer", "AsyncDataExample")]
public class AsyncMemberDataAttribute : DataAttribute
{
    public AsyncMemberDataAttribute(string memberName, params object?[] parameters)
    {
        MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
        Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    public bool DisableDiscoveryEnumeration { get; set; }

    public string MemberName { get; private set; }

    public Type? MemberType { get; set; }

    public object?[] Parameters { get; private set; }

    object?[]? ConvertDataItem(MethodInfo testMethod, object? item)
    {
        if (item is null)
            return null;

        var array = item as object[];
        if (array is null)
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Member {0} on {1} yielded an item that is not an object[]",
                    MemberName,
                    MemberType ?? testMethod.DeclaringType
                )
            );

        return array;
    }

    public override IEnumerable<object?[]?>? GetData(MethodInfo testMethod)
    {
        if (testMethod is null)
            throw new ArgumentNullException(nameof(testMethod));

        var type = MemberType ?? testMethod.DeclaringType ?? throw new ArgumentException("testMethod.DeclaringType returned null", nameof(testMethod));
        var accessor = GetPropertyAccessor(type) ?? GetFieldAccessor(type) ?? GetMethodAccessor(type);
        if (accessor is null)
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Could not find public static member (property, field, or method) named '{0}' on {1}{2}",
                    MemberName,
                    type.FullName,
                    Parameters?.Length > 0
                        ? string.Format(CultureInfo.CurrentCulture, " with parameter types: {0}", string.Join(", ", Parameters.Select(p => p?.GetType().FullName ?? "(null)")))
                        : ""
                )
            );

        // We can't change the signature of DataAttribute, so we have to rely on being able to launch
        // a task off the current thread and then use the normally unsafe GetAwaiter().GetResult() to
        // pull the result.
        var obj = Task.Run(accessor).GetAwaiter().GetResult();
        if (obj is null)
            return null;

        var dataItems = obj as IEnumerable;
        if (dataItems is not null)
            return dataItems.Cast<object?>().Select(item => ConvertDataItem(testMethod, item));

        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Member {0} on {1} did not return a Task<T> where T is compatible with IEnumerable", MemberName, type.FullName));
    }

    Func<Task<object?>>? GetFieldAccessor(Type? type)
    {
        FieldInfo? fieldInfo = null;
        for (var reflectionType = type; reflectionType is not null; reflectionType = reflectionType.GetTypeInfo().BaseType)
        {
            fieldInfo = reflectionType.GetRuntimeField(MemberName);
            if (fieldInfo is not null)
                break;
        }

        if (fieldInfo is null || !fieldInfo.IsStatic)
            return null;

        return () => ValidateTask(fieldInfo.GetValue(null));
    }

    Func<Task<object?>>? GetMethodAccessor(Type? type)
    {
        if (type is null)
            return null;

        MethodInfo? methodInfo = null;
        var parameterTypes = Parameters is null ? [] : Parameters.Select(p => p?.GetType()).ToArray();
        for (var reflectionType = type; reflectionType is not null; reflectionType = reflectionType.GetTypeInfo().BaseType)
        {
            var runtimeMethodsWithGivenName = reflectionType.GetRuntimeMethods()
                                                            .Where(m => m.Name == MemberName)
                                                            .ToArray();
            methodInfo = runtimeMethodsWithGivenName.FirstOrDefault(m => ParameterTypesCompatible(m.GetParameters(), parameterTypes));

            if (methodInfo is not null)
                break;

            if (runtimeMethodsWithGivenName.Any(m => m.GetParameters().Any(p => p.IsOptional)))
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Method '{0}.{1}' contains optional parameters, which are not currently supported. Please use overloads if necessary.",
                        type.FullName,
                        MemberName
                    )
                );
        }

        if (methodInfo is null || !methodInfo.IsStatic)
            return null;

        return () => ValidateTask(methodInfo.Invoke(null, Parameters));
    }

    Func<Task<object?>>? GetPropertyAccessor(Type? type)
    {
        PropertyInfo? propInfo = null;
        for (var reflectionType = type; reflectionType is not null; reflectionType = reflectionType.GetTypeInfo().BaseType)
        {
            propInfo = reflectionType.GetRuntimeProperty(MemberName);
            if (propInfo is not null)
                break;
        }

        if (propInfo is null || propInfo.GetMethod is null || !propInfo.GetMethod.IsStatic)
            return null;

        return () => ValidateTask(propInfo.GetValue(null, null));
    }

    static bool ParameterTypesCompatible(ParameterInfo[] parameters, Type?[] parameterTypes)
    {
        if (parameters?.Length != parameterTypes.Length)
            return false;

        for (int idx = 0; idx < parameters.Length; ++idx)
            if (parameterTypes[idx] != null && !parameters[idx].ParameterType.GetTypeInfo().IsAssignableFrom(parameterTypes[idx]?.GetTypeInfo()))
                return false;

        return true;
    }

    static async Task<object?> ValidateTask(object? value)
    {
        if (value is null)
            return null;

        if (value is not Task taskValue)
            return null;

        // We'll await it right away, even when it might not match up with things we want,
        // because the await will cause it to run and perhaps throw, which we always want to
        // happen even when there are type incompatbilities
        await taskValue;

        // We need to dive down through all the concrete types, since we might end up with
        // something that's derived from Task<T> instead of directly getting a Task<T>.
        // This is common for methods using the "async" keyword, as the compiler auto-generates
        // the "real" method body and can change the return type underneath when doing so.
        for (var valueType = value.GetType(); ; valueType = valueType.BaseType)
        {
            if (valueType is null)
                return null;

            if (!valueType.IsGenericType)
                continue;

            if (valueType.GetGenericTypeDefinition() != typeof(Task<>))
                continue;

            var resultProperty = valueType.GetProperty(nameof(Task<object>.Result));
            if (resultProperty is null)
                continue;

            var getMethod = resultProperty.GetGetMethod();
            if (getMethod is null)
                continue;

            return getMethod.Invoke(value, null);
        }
    }
}
