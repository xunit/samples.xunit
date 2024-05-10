using System.Threading.Tasks;
using Xunit;

namespace NamespaceParallelization.Example.ConfigB;

#pragma warning disable xUnit1041  // This is an unconventional "fixture" arrangement

public class BTest2(Setup setup) : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        setup.SendDiagnosticMessage("BTest2 Dispose");
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        setup.SendDiagnosticMessage("BTest2 Initialize");
        return Task.CompletedTask;
    }

    [Fact]
    public async Task BTest2_Test1()
    {
        setup.SendDiagnosticMessage("BTest2_Test1");
        await DemoDelay.WaitRandomTimeAsync();
        //Assert.Fail();
    }

    [Fact]
    public async Task BTest2_Test2()
    {
        setup.SendDiagnosticMessage("BTest2_Test2");
        await DemoDelay.WaitRandomTimeAsync();
        //Assert.Fail();
    }

    [Fact]
    public async Task BTest2_Test3()
    {
        setup.SendDiagnosticMessage("BTest2_Test3");
        await DemoDelay.WaitRandomTimeAsync();
    }
}
