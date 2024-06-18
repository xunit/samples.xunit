using System.Threading.Tasks;
using Xunit;

namespace NamespaceParallelization.Example.ConfigA;

public class Setup(TopLevelSetup parent) : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        SendDiagnosticMessage("ConfigA.Setup - Dispose");
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        SendDiagnosticMessage("ConfigA.Setup - Start Setup");

        await DemoDelay.WaitFixedTimeAsync(4000);

        SendDiagnosticMessage("ConfigA.Setup - Finish Setup");
    }

    public void SendDiagnosticMessage(string message) =>
        parent.SendDiagnosticMessage(message);
}
