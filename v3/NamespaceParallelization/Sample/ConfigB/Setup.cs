using System.Threading.Tasks;
using Xunit;

namespace NamespaceParallelization.Sample.ConfigB;

// You have access to the top level setup object (though we're not using it)
public sealed class Setup(TopLevelSetup topLevelSetup) : IAsyncLifetime
{
    public ValueTask DisposeAsync()
    {
        TestContext.Current.SendDiagnosticMessage("ConfigB.Setup - Dispose");
        return default;
    }

    public async ValueTask InitializeAsync()
    {
        TestContext.Current.SendDiagnosticMessage("ConfigB.Setup - Start Setup");

        await DemoDelay.WaitFixedTimeAsync(4000);
        Assert.NotNull(topLevelSetup);

        TestContext.Current.SendDiagnosticMessage("ConfigB.Setup - Finish Setup");
    }
}
