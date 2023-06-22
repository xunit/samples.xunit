using System;
using System.Threading.Tasks;
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

    // Important: although the assertion library enables support for ValueTask, your
    // test methods must still return void or Task; if you return ValueTask, the behavior
    // is undefined (they will likely silently pass rather than fail appropriately). This
    // is because support for ValueTask-based test methods is not supported with the
    // v2 core framework.
    [Fact]
    public async Task ValueTaskExample()
    {
        int[] values = new[] { 4, 5 };

        await Assert.AllAsync(
            values,
            value => Assert.IsEvenAsync(value)
        );
    }
}
