using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace ObservationExample;

[DebuggerDisplay(@"\{ class = {TestClass.TestClassName}, method = {MethodName} \}")]
public class ObservationTestMethod : ITestMethod, IXunitSerializable
{
    MethodInfo? method;
    ObservationTestClass? testClass;
    readonly Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> traits;
    readonly Lazy<string> uniqueID;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public ObservationTestMethod()
    {
        traits = new(() => ExtensibilityPointFactory.GetMethodTraits(Method, TestClass.Traits));
        uniqueID = new(() => UniqueIDGenerator.ForTestMethod(TestClass.UniqueID, MethodName));
    }

#pragma warning disable CS0618
    public ObservationTestMethod(
        ObservationTestClass testClass,
        MethodInfo method) :
            this()
#pragma warning restore CS0618
    {
        this.testClass = Guard.ArgumentNotNull(testClass);
        this.method = Guard.ArgumentNotNull(method);
    }

    public string DisplayName =>
        Method.Name.Replace('_', ' ');

    public MethodInfo Method =>
        method ?? throw new InvalidOperationException($"Attempted to retrieve an uninitialized {nameof(ObservationTestMethod)}.{nameof(Method)}");

    public string MethodName =>
        Method.Name;

    public ObservationTestClass TestClass =>
        testClass ?? throw new InvalidOperationException($"Attempted to retrieve an uninitialized {nameof(ObservationTestMethod)}.{nameof(TestClass)}");

    ITestClass ITestMethod.TestClass =>
        TestClass;

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits =>
        traits.Value;

    public string UniqueID =>
        uniqueID.Value;

    public void Deserialize(IXunitSerializationInfo info)
    {
        testClass = Guard.NotNull("Could not retrieve TestClass from serialization", info.GetValue<ObservationTestClass>("c"));

        var reflectedType = Guard.NotNull("Could not retrieve the class name of the test method", info.GetValue<string>("t"));
        var @class = Guard.NotNull(() => $"Could not look up type {reflectedType}", TypeHelper.GetType(reflectedType));
        var methodName = Guard.NotNull("Could not retrieve MethodName from serialization", info.GetValue<string>("n"));
        method = Guard.NotNull(() => $"Could not find test method {methodName} on test class {testClass.TestClassName}", @class.GetMethod(methodName, ObservationTestClass.MethodBindingFlags));
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        Guard.NotNull("Method does not appear to come from a reflected type", Method.ReflectedType);
        Guard.NotNull("Method's reflected type does not have an assembly qualified name", Method.ReflectedType.AssemblyQualifiedName);

        info.AddValue("t", Method.ReflectedType.AssemblyQualifiedName);
        info.AddValue("n", Method.Name);
        info.AddValue("c", TestClass);
    }
}
