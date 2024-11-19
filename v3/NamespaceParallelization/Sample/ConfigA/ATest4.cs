using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace NamespaceParallelization.Sample.ConfigA;

// This one is slightly different, to verify that theory data rows run in parallel, even when the
// theory data is not necessarily serializable

#pragma warning disable xUnit1041  // This is an unconventional "fixture" arrangement

// You have access to the setup object (though we're not using it)
public sealed class ATest4(Setup setup) : IAsyncLifetime
{
    public ValueTask DisposeAsync()
    {
        TestContext.Current.SendDiagnosticMessage("ATest4 Dispose for {0}", TestContext.Current.Test?.TestDisplayName ?? "(null)");
        return default;
    }

    public ValueTask InitializeAsync()
    {
        TestContext.Current.SendDiagnosticMessage("ATest4 Initialize for {0}", TestContext.Current.Test?.TestDisplayName ?? "(null)");
        return default;
    }

    public static IEnumerable<TheoryDataRow<object?>> TestData = [42, 21.12m, "Hello, World!", new(null), new object()];

    [Theory]
    [MemberData(nameof(TestData), DisableDiscoveryEnumeration = true)]
    public async Task ATest4_Test(object? data)
    {
        TestContext.Current.SendDiagnosticMessage($"ATest4_Test({ArgumentFormatter.Format(data)}) starting");
        await DemoDelay.WaitRandomTimeAsync();
        Assert.NotNull(setup);
        TestContext.Current.SendDiagnosticMessage($"ATest4_Test({ArgumentFormatter.Format(data)}) finished");
    }
}
