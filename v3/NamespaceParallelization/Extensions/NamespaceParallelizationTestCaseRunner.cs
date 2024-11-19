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
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        string displayName,
        string? skipReason,
        ExplicitOption explicitOption,
        object?[] constructorArguments)
    {
        await using var ctxt = new NamespaceParallelizationTestCaseRunnerContext(testCase, tests, messageBus, aggregator, cancellationTokenSource, displayName, skipReason, explicitOption, constructorArguments);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTest(
        NamespaceParallelizationTestCaseRunnerContext ctxt,
        IXunitTest test) =>
            XunitTestRunner.Instance.Run(test, ctxt.MessageBus, ctxt.ConstructorArguments, ctxt.ExplicitOption, ctxt.Aggregator.Clone(), ctxt.CancellationTokenSource, ctxt.BeforeAfterTestAttributes);

    // Run everything in parallel
    protected override async ValueTask<RunSummary> RunTestCase(NamespaceParallelizationTestCaseRunnerContext ctxt, Exception? exception)
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
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    string displayName,
    string? skipReason,
    ExplicitOption explicitOption,
    object?[] constructorArguments) :
        XunitTestCaseRunnerBaseContext<IXunitTestCase, IXunitTest>(testCase, tests, messageBus, aggregator, cancellationTokenSource, displayName, skipReason, explicitOption, constructorArguments)
{ }
