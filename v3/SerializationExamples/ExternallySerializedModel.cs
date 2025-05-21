using System;
using System.Diagnostics.CodeAnalysis;
using SerializationExamples;
using Xunit.Sdk;

[assembly: RegisterXunitSerializer(typeof(ExternallySerializedModelSerializer), typeof(ExternallySerializedModel))]

namespace SerializationExamples;

public record ExternallySerializedModel(int IntValue, string StringValue);

public class ExternallySerializedModelSerializer : IXunitSerializer
{
    public object Deserialize(Type type, string serializedValue)
    {
        if (type != typeof(ExternallySerializedModel))
            throw new ArgumentException($"Type {type.FullName} is not supported", nameof(type));

        var pieces = serializedValue.Split('\n');
        if (pieces.Length < 2)
            throw new ArgumentException($"Serialized value '{serializedValue}' is malformed", nameof(serializedValue));

        return new ExternallySerializedModel(int.Parse(pieces[0]), pieces[1]);
    }

    public bool IsSerializable(Type type, object? value, [NotNullWhen(false)] out string? failureReason)
    {
        if (type != typeof(ExternallySerializedModel))
        {
            failureReason = $"Type {type.FullName} is not supported";
            return false;
        }

        if (value is not ExternallySerializedModel)
        {
            failureReason = $"Value of type {value?.GetType().FullName ?? "null"} is not supported";
            return false;
        }

        failureReason = null;
        return true;
    }

    public string Serialize(object value)
    {
        if (value is not ExternallySerializedModel model)
            throw new ArgumentException($"Value of type {value?.GetType().FullName ?? "null"} is not supported", nameof(value));

        return $"{model.IntValue}\n{model.StringValue}";
    }
}
