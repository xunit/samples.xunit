using System.Threading.Tasks;
using Xunit;

namespace NamespaceParallelization.Sample.ConfigC;

// You have access to the top level setup object (though we're not using it)
public sealed class Setup(TopLevelSetup topLevelSetup) : IAsyncLifetime
{
    public ValueTask DisposeAsync()
    {
        TestContext.Current.SendDiagnosticMessage("ConfigC.Setup - Dispose");
        return default;
    }

    public async ValueTask InitializeAsync()
    {
        TestContext.Current.SendDiagnosticMessage("ConfigC.Setup - Start Setup");

        await DemoDelay.WaitFixedTimeAsync(4000);
        Assert.NotNull(topLevelSetup);

        TestContext.Current.SendDiagnosticMessage("ConfigC.Setup - Finish Setup");
    }
}
