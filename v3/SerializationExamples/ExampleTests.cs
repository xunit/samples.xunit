using System.Collections.Generic;
using Xunit;

namespace SerializationExamples;

// By making the data serializable, it should render as individually runnable tests
// in Test Explorer.

public class ExampleTests
{
    public static IEnumerable<TheoryDataRow<SelfSerializingModel>> SelfSerializingData =
    [
        new SelfSerializingModel{ IntValue = 42, StringValue = "Hello" },
        new SelfSerializingModel{ IntValue = 0, StringValue = "World" },
    ];

    [Theory]
    [MemberData(nameof(SelfSerializingData))]
    public void SelfSerializingTest(SelfSerializingModel model)
    {
        Assert.NotEqual(0, model.IntValue);
    }

    public static IEnumerable<TheoryDataRow<ExternallySerializedModel>> ExternallySerializedData =
    [
        new ExternallySerializedModel(42, "Hello"),
        new ExternallySerializedModel(0, "World"),
    ];

    [Theory]
    [MemberData(nameof(ExternallySerializedData))]
    public void ExternallySerializedTest(ExternallySerializedModel model)
    {
        Assert.NotEqual(0, model.IntValue);
    }
}
