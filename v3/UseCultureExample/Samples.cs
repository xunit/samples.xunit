using Xunit;

public class Samples
{
    [Fact]
    [UseCulture("en-US")]
    public void US_UsesPeriod() =>
        Assert.Equal("21.12", 21.12m.ToString());

    [Fact]
    [UseCulture("fr-FR")]
    public void France_UsesComma() =>
        Assert.Equal("21,12", 21.12m.ToString());
}
