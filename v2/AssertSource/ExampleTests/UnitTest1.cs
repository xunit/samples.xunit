using System;
using Xunit;

namespace ExampleTests;

public class UnitTest1
{
    [Fact]
    public void CustomAssertionExample()
    {
        Assert.IsEven(5);
    }

    static string? GetSomePossiblyNullStringValue() => "Hello world";

    [Fact]
    public void NullableExample()
    {
        string? x = GetSomePossiblyNullStringValue();

        Assert.NotNull(x);  // If you comment out this line, then the line below will yield CS8604 (possible null reference)
        Assert.NotEmpty(x);
    }

    [Fact]
    public void SpanExample()
    {
        Span<int> x = new[] { 3, 4 }.AsSpan();
        Span<int> y = new[] { 5, 6 }.AsSpan();

        Assert.Equal(x, y);
    }
}
