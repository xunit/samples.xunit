using System;
using System.Threading.Tasks;
using Xunit;

public class AsyncExamples
{
    // Your async tests can return Task...
    [Fact]
    public async Task CodeThrowsAsync()
    {
        var ex = await Assert.ThrowsAsync<NotImplementedException>(ThrowingMethod);

        Assert.IsType<NotImplementedException>(ex);
    }

    // ...or ValueTask
    [Fact]
    public async ValueTask RecordAsync()
    {
        var ex = await Record.ExceptionAsync(ThrowingMethod);

        Assert.IsType<NotImplementedException>(ex);
    }

    async Task ThrowingMethod()
    {
        await Task.Yield();

        throw new NotImplementedException();
    }
}
