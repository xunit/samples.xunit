using System;
using System.Threading.Tasks;
using Xunit;

// This overrides the default test framework with one that discovers & executes in this alternative method
[assembly: TestFramework(typeof(NamespaceParallelization.Extensions.NamespaceParallelizationTestFramework))]

namespace NamespaceParallelization.Sample;

// Note: All usage of IAsyncLifetime could be replaced with constructor + IDisposable if
// you don't need asynchronous initialization and cleanup. Replace InitializeAsync() with
// a standard constructor, and replace DisposeAsync() with Dispose().

public class TopLevelSetup : IAsyncLifetime
{
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        TestContext.Current.SendDiagnosticMessage("TopLevelSetup Dispose");
        return default;
    }

    public ValueTask InitializeAsync()
    {
        TestContext.Current.SendDiagnosticMessage("TopLevelSetup Initialize");
        return default;
    }
}
