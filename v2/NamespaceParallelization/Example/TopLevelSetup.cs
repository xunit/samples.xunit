using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

// This overrides the default test framework with one that discovers & executes in this alternative method
[assembly: TestFramework("NamespaceParallelization.XunitExtensions.NamespaceParallelizationTestFramework", "NamespaceParallelization")]

namespace NamespaceParallelization.Example;

// Note: All usage of IAsyncLifetime could be replaced with constructor + IDisposable if
// you don't need asynchronous initialization and cleanup. Replace InitializeAsync() with
// a standard constructor, and replace DisposeAsync() with Dispose().

public class TopLevelSetup(IMessageSink diagnosticMessageSink) : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        Console.WriteLine("TopLevelSetup Dispose");
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        Console.WriteLine("TopLevelSetup Initialize");
        return Task.CompletedTask;
    }

    public void SendDiagnosticMessage(string message) =>
        diagnosticMessageSink.OnMessage(new DiagnosticMessage(message));
}
