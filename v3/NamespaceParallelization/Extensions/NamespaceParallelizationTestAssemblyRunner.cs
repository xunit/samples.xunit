using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace NamespaceParallelization.Extensions;

public class NamespaceParallelizationTestAssemblyRunner :
    XunitTestAssemblyRunnerBase<NamespaceParallelizationTestAssemblyRunnerContext, IXunitTestAssembly, IXunitTestCollection, IXunitTestCase>
{
    readonly Dictionary<Type, object?> topLevelSetupInstances = [];

    public static NamespaceParallelizationTestAssemblyRunner Instance { get; } = new();

    protected override async ValueTask<bool> OnTestAssemblyFinished(
        NamespaceParallelizationTestAssemblyRunnerContext ctxt,
        RunSummary summary)
    {
        foreach (var instance in topLevelSetupInstances.Values)
            if (instance is IAsyncLifetime asyncLifetime)
                await ctxt.Aggregator.RunAsync(asyncLifetime.DisposeAsync);
            else if (instance is IDisposable disposable)
                ctxt.Aggregator.Run(disposable.Dispose);

        return await base.OnTestAssemblyFinished(ctxt, summary);
    }

    public async ValueTask<RunSummary> Run(
        IXunitTestAssembly testAssembly,
        IReadOnlyCollection<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
    {
        await using var ctxt = new NamespaceParallelizationTestAssemblyRunnerContext(testAssembly, testCases, executionMessageSink, executionOptions);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTestCollection(
        NamespaceParallelizationTestAssemblyRunnerContext ctxt,
        IXunitTestCollection testCollection,
        IReadOnlyCollection<IXunitTestCase> testCases) =>
            NamespaceParallelizationTestCollectionRunner.Instance.Run(
                ctxt.TopLevelSetupInstances,
                testCollection,
                testCases,
                ctxt.ExplicitOption,
                ctxt.MessageBus,
                ctxt.Aggregator.Clone(),
                ctxt.CancellationTokenSource
            );

    // Always run all collections sequentially, since each collection represents a namespace, and we want tests inside a
    // namespace to run in parallel against each other, but each namespace runs sequentially.
    protected override async ValueTask<RunSummary> RunTestCollections(
        NamespaceParallelizationTestAssemblyRunnerContext ctxt,
        Exception? exception)
    {
        // We're going to run all parallelism on the thread pool, so let's make sure we have enough threads available
        var threadCount = ctxt.MaxParallelThreads;
        if (threadCount > 0)
        {
            ThreadPool.GetMaxThreads(out var workerThreads, out var ioThreads);
            if (workerThreads < threadCount)
                ThreadPool.SetMaxThreads(threadCount, ioThreads);
        }

        // Group test cases by collection and run them sequentially
        var summary = new RunSummary();
        var testCasesByCollection =
            ctxt.TestCases
                .GroupBy(tc => tc.TestMethod.TestClass.TestCollection, TestCollectionComparer<IXunitTestCollection>.Instance)
                .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var kvp in testCasesByCollection)
        {
            summary.Aggregate(await RunTestCollection(ctxt, kvp.Key, kvp.Value));
            if (ctxt.CancellationTokenSource.IsCancellationRequested)
                break;
        }

        return summary;
    }
}

public class NamespaceParallelizationTestAssemblyRunnerContext(
    IXunitTestAssembly testAssembly,
    IReadOnlyCollection<IXunitTestCase> testCases,
    IMessageSink executionMessageSink,
    ITestFrameworkExecutionOptions executionOptions) :
        XunitTestAssemblyRunnerBaseContext<IXunitTestAssembly, IXunitTestCase>(testAssembly, testCases, executionMessageSink, executionOptions)
{
    public Dictionary<Type, object> TopLevelSetupInstances { get; } = [];
}
