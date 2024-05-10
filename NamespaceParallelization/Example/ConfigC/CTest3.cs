using System.Threading.Tasks;
using Xunit;

namespace NamespaceParallelization.Example.ConfigC;

#pragma warning disable xUnit1041  // This is an unconventional "fixture" arrangement

public class CTest3(Setup setup) : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        setup.SendDiagnosticMessage("CTest3 Dispose");
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        setup.SendDiagnosticMessage("CTest3 Initialize");
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CTest3_Test1()
    {
        setup.SendDiagnosticMessage("CTest3_Test1");
        await DemoDelay.WaitRandomTimeAsync();
        //Assert.Fail();
    }

    [Fact]
    public async Task CTest3_Test2()
    {
        setup.SendDiagnosticMessage("CTest3_Test2");
        await DemoDelay.WaitRandomTimeAsync();
        //Assert.Fail();
    }

    [Fact]
    public async Task CTest3_Test3()
    {
        setup.SendDiagnosticMessage("CTest3_Test3");
        await DemoDelay.WaitRandomTimeAsync();
    }
}
