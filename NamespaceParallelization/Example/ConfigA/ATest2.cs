using System.Threading.Tasks;
using Xunit;

namespace NamespaceParallelization.Example.ConfigA;

#pragma warning disable xUnit1041  // This is an unconventional "fixture" arrangement

public class ATest2(Setup setup) : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        setup.SendDiagnosticMessage("ATest2 Dispose");
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        setup.SendDiagnosticMessage("ATest2 Initialize");
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ATest2_Test1()
    {
        setup.SendDiagnosticMessage("ATest2_Test1");
        await DemoDelay.WaitRandomTimeAsync();
        //Assert.Fail();
    }

    [Fact]
    public async Task ATest2_Test2()
    {
        setup.SendDiagnosticMessage("ATest2_Test2");
        await DemoDelay.WaitRandomTimeAsync();
        //Assert.Fail();
    }

    [Fact]
    public async Task ATest2_Test3()
    {
        setup.SendDiagnosticMessage("ATest2_Test3");
        await DemoDelay.WaitRandomTimeAsync();
    }
}
