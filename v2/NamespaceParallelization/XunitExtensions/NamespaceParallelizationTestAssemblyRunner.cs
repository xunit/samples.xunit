using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NamespaceParallelization.XunitExtensions;

public class NamespaceParallelizationTestAssemblyRunner(ITestAssembly testAssembly,
                                                        IEnumerable<IXunitTestCase> testCases,
                                                        IMessageSink diagnosticMessageSink,
                                                        IMessageSink executionMessageSink,
                                                        ITestFrameworkExecutionOptions executionOptions)
    : XunitTestAssemblyRunner(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
{
    readonly Dictionary<Type, object?> topLevelSetupInstances = new();

    // Clean up any of the top-level startup classes
    protected override async Task BeforeTestAssemblyFinishedAsync()
    {
        foreach (var instance in topLevelSetupInstances.Values)
            if (instance is IAsyncLifetime asyncLifetime)
                await Aggregator.RunAsync(asyncLifetime.DisposeAsync);
            else if (instance is IDisposable disposable)
                Aggregator.Run(disposable.Dispose);
    }

    // Always run all collections sequentially, since each collection represents a namespace, and we want tests inside a
    // namespace to run in parallel against each other, but each namespace runs sequentially. We are also ignoring any
    // requested test collection orderer here, though we could fall back to OrderTestCollections() if the default
    // behavior is desired.
    protected override async Task<RunSummary> RunTestCollectionsAsync(IMessageBus messageBus,
                                                                      CancellationTokenSource cancellationTokenSource)
    {
        // We're going to run all parallelism on the thread pool, so let's make sure we have enough threads available
        var threadCount = ExecutionOptions.MaxParallelThreadsOrDefault();
        if (threadCount > 0)
        {
            ThreadPool.GetMaxThreads(out var workerThreads, out var ioThreads);
            if (workerThreads < threadCount)
                ThreadPool.SetMaxThreads(threadCount, ioThreads);
        }

        // Group test cases by collection and run them sequentially
        var summary = new RunSummary();
        var testCasesByCollection = TestCases.GroupBy(tc => tc.TestMethod.TestClass.TestCollection, TestCollectionComparer.Instance)
                                             .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var kvp in testCasesByCollection)
        {
            summary.Aggregate(await RunTestCollectionAsync(messageBus, kvp.Key, kvp.Value, cancellationTokenSource));
            if (cancellationTokenSource.IsCancellationRequested)
                break;
        }

        return summary;
    }

    protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus,
                                                               ITestCollection testCollection,
                                                               IEnumerable<IXunitTestCase> testCases,
                                                               CancellationTokenSource cancellationTokenSource)
        => new NamespaceParallelizationTestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), topLevelSetupInstances, cancellationTokenSource).RunAsync();
}
