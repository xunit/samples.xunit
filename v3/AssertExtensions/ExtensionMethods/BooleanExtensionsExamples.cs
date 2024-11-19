using Xunit;
using Xunit.Extensions.AssertExtensions;

// This is an example of using existing assertions in a new way: by adding extension methods to existing
// types. So rather than seeing Assert.Xyz, you'll see extenion methods off of boolean values like
// ShouldBeTrue and ShouldBeFalse.
public class BooleanExtensionsExamples
{
    [Fact]
    public void ShouldBeTrue() =>
        true.ShouldBeTrue();

    [Fact]
    public void ShouldBeFalse() =>
        false.ShouldBeFalse();

    [Fact]
    public void ShouldBeTrueWithMessage()
    {
        var ex = Record.Exception(() => false.ShouldBeTrue("should be true"));

        Assert.NotNull(ex);
        Assert.StartsWith("should be true", ex.Message);
    }

    [Fact]
    public void ShouldBeFalseWithMessage()
    {
        var ex = Record.Exception(() => true.ShouldBeFalse("should be false"));

        Assert.NotNull(ex);
        Assert.StartsWith("should be false", ex.Message);
    }
}
