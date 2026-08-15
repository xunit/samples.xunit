using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace NamespaceParallelization.Extensions;

public class NamespaceParallelizationTestCaseRunner :
    XunitTestCaseRunnerBase<NamespaceParallelizationTestCaseRunnerContext, IXunitTestCase, IXunitTest>
{
    public static NamespaceParallelizationTestCaseRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        IXunitTestCase testCase,
        IReadOnlyCollection<IXunitTest> tests,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        string displayName,
        string? skipReason,
        CancellationTokenSource cancellationTokenSource,
        ParallelMode parallelMode,
        ExecutionScheduler scheduler,
        object?[] constructorArguments,
        FixtureMappingManager methodFixtureMappings)
    {
        await using var ctxt = new NamespaceParallelizationTestCaseRunnerContext(
            testCase,
            tests,
            explicitOption,
            messageBus,
            aggregator,
            displayName,
            skipReason,
            cancellationTokenSource,
            parallelMode,
            scheduler,
            constructorArguments,
            methodFixtureMappings
        );
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTest(
        NamespaceParallelizationTestCaseRunnerContext ctxt,
        IXunitTest test) =>
            XunitTestRunner.Instance.Run(
                test,
                ctxt.MessageBus,
                ctxt.ConstructorArguments,
                ctxt.ExplicitOption,
                ctxt.Aggregator.Clone(),
                ctxt.CancellationTokenSource,
                ctxt.ParallelMode,
                ctxt.Scheduler,
                ctxt.BeforeAfterTestAttributes,
                ctxt.CaseFixtureMappings
            );

    protected override async ValueTask<RunSummary> RunTestCaseInner(
        NamespaceParallelizationTestCaseRunnerContext ctxt,
        Exception? exception)
    {
        Guard.ArgumentNotNull(ctxt);

        var testCaseSummary = new RunSummary();
        var tasks = new List<Task<RunSummary>>();

        foreach (var test in ctxt.Tests)
            if (exception is not null)
                tasks.Add(Task.Run(async () => await FailTest(ctxt, test, exception), ctxt.CancellationTokenSource.Token));
            else
                tasks.Add(Task.Run(async () => await RunTest(ctxt, test), ctxt.CancellationTokenSource.Token));

        var testSummaries = await Task.WhenAll(tasks);
        foreach (var testSummary in testSummaries)
            testCaseSummary.Aggregate(testSummary);

        return testCaseSummary;
    }
}

public class NamespaceParallelizationTestCaseRunnerContext(
    IXunitTestCase testCase,
    IReadOnlyCollection<IXunitTest> tests,
    ExplicitOption explicitOption,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    string displayName,
    string? skipReason,
    CancellationTokenSource cancellationTokenSource,
    ParallelMode parallelMode,
    ExecutionScheduler scheduler,
    object?[] constructorArguments,
    FixtureMappingManager methodFixtureMappings) :
        XunitTestCaseRunnerBaseContext<IXunitTestCase, IXunitTest>(
            testCase,
            tests,
            explicitOption,
            messageBus,
            aggregator,
            displayName,
            skipReason,
            cancellationTokenSource,
            parallelMode,
            scheduler,
            constructorArguments,
            methodFixtureMappings
        )
{ }
