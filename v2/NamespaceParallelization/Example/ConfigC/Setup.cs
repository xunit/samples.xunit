using System.Threading.Tasks;
using Xunit;

namespace NamespaceParallelization.Example.ConfigC;

public class Setup(TopLevelSetup parent) : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        SendDiagnosticMessage("ConfigC.Setup - Dispose");
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        SendDiagnosticMessage("ConfigC.Setup - Start Setup");

        await DemoDelay.WaitFixedTimeAsync(4000);

        SendDiagnosticMessage("ConfigC.Setup - Finish Setup");
    }

    public void SendDiagnosticMessage(string message) =>
        parent.SendDiagnosticMessage(message);
}
