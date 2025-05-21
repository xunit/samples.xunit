using Xunit.Sdk;

namespace SerializationExamples;

public class SelfSerializingModel : IXunitSerializable
{
    public required int IntValue { get; set; }

    public required string StringValue { get; set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        IntValue = info.GetValue<int?>(nameof(IntValue)) ?? int.MinValue;
        StringValue = info.GetValue<string?>(nameof(StringValue)) ?? "<unset>";
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(IntValue), IntValue);
        info.AddValue(nameof(StringValue), StringValue);
    }

    // You can customize how the class is displayed in parameters by overriding ToString
    public override string ToString() =>
        $"{IntValue} :: \"{StringValue}\"";
}
