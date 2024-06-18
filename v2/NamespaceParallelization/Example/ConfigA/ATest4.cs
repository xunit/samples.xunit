using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace NamespaceParallelization.Example.ConfigA;

// This one is slightly different, to verify that theory data rows run in parallel, even when the
// theory data is not necessarily serializable

#pragma warning disable xUnit1041  // This is an unconventional "fixture" arrangement

public class ATest4(Setup setup) : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        setup.SendDiagnosticMessage("ATest4 Dispose");
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        setup.SendDiagnosticMessage("ATest4 Initialize");
        return Task.CompletedTask;
    }

    public static TheoryData<object?> TestData = new() { 42, 21.12m, "Hello, World!", null, new object() };

    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task ATest4_Test(object? data)
    {
        setup.SendDiagnosticMessage($"ATest4_Test({ArgumentFormatter.Format(data)}) starting");
        await DemoDelay.WaitRandomTimeAsync();
        setup.SendDiagnosticMessage($"ATest4_Test({ArgumentFormatter.Format(data)}) finished");
    }
}
