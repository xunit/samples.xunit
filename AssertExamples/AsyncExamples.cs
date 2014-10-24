using System;
using System.Threading.Tasks;
using Xunit;

public class AsyncExamples
{
    [Fact]
    public async void CodeThrowsAsync()
    {
        Func<Task> testCode = () => Task.Factory.StartNew(ThrowingMethod);

        var ex = await Assert.ThrowsAsync<NotImplementedException>(testCode);

        Assert.IsType<NotImplementedException>(ex);
    }

    [Fact]
    public async void RecordAsync()
    {
        Func<Task> testCode = () => Task.Factory.StartNew(ThrowingMethod);

        var ex = await Record.ExceptionAsync(testCode);

        Assert.IsType<NotImplementedException>(ex);
    }

    void ThrowingMethod()
    {
        throw new NotImplementedException();
    }
}

