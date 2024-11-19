using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace NamespaceParallelization.Extensions;

public class NamespaceParallelizationTestMethodRunner :
    XunitTestMethodRunnerBase<NamespaceParallelizationTestMethodRunnerContext, IXunitTestMethod, IXunitTestCase>
{
    public static NamespaceParallelizationTestMethodRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        IXunitTestMethod testMethod,
        IReadOnlyCollection<IXunitTestCase> testCases,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        object?[] constructorArguments)
    {
        await using var ctxt = new NamespaceParallelizationTestMethodRunnerContext(testMethod, testCases, explicitOption, messageBus, aggregator, cancellationTokenSource, constructorArguments);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override async ValueTask<RunSummary> RunTestCase(
        NamespaceParallelizationTestMethodRunnerContext ctxt,
        IXunitTestCase testCase)
    {
        // This code is mostly copied from XunitRunnerHelper.RunXunitTestCase, except that it delegates
        // to NamespaceParallelizationTestCaseRunner instead of XunitTestCaseRunner.
        var tests = await ctxt.Aggregator.RunAsync(testCase.CreateTests, []);

        if (ctxt.Aggregator.ToException() is Exception ex)
        {
            if (ex.Message.StartsWith(DynamicSkipToken.Value, StringComparison.Ordinal))
                return XunitRunnerHelper.SkipTestCases(
                    ctxt.MessageBus,
                    ctxt.CancellationTokenSource,
                    [testCase],
                    ex.Message.Substring(DynamicSkipToken.Value.Length),
                    sendTestCaseMessages: false
                );
            else
                return XunitRunnerHelper.FailTestCases(
                    ctxt.MessageBus,
                    ctxt.CancellationTokenSource,
                    [testCase],
                    ex,
                    sendTestCaseMessages: false
                );
        }

        return await NamespaceParallelizationTestCaseRunner.Instance.Run(
            testCase,
            tests,
            ctxt.MessageBus,
            ctxt.Aggregator,
            ctxt.CancellationTokenSource,
            testCase.TestCaseDisplayName,
            testCase.SkipReason,
            ctxt.ExplicitOption,
            ctxt.ConstructorArguments
        );
    }

    // Run everything in parallel
    protected override async ValueTask<RunSummary> RunTestCases(
        NamespaceParallelizationTestMethodRunnerContext ctxt,
        Exception? exception)
    {
        var testMethodSummary = new RunSummary();
        var tasks = new List<Task<RunSummary>>();

        foreach (var testCase in ctxt.TestCases)
            if (exception is not null)
                tasks.Add(Task.Run(async () => await FailTestCase(ctxt, testCase, exception), ctxt.CancellationTokenSource.Token));
            else
                tasks.Add(Task.Run(async () => await RunTestCase(ctxt, testCase), ctxt.CancellationTokenSource.Token));

        var testCaseSummaries = await Task.WhenAll(tasks);
        foreach (var testCaseSummary in testCaseSummaries)
            testMethodSummary.Aggregate(testCaseSummary);

        return testMethodSummary;
    }
}

public class NamespaceParallelizationTestMethodRunnerContext(
    IXunitTestMethod testMethod,
    IReadOnlyCollection<IXunitTestCase> testCases,
    ExplicitOption explicitOption,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    object?[] constructorArguments) :
        XunitTestMethodRunnerBaseContext<IXunitTestMethod, IXunitTestCase>(testMethod, testCases, explicitOption, messageBus, aggregator, cancellationTokenSource, constructorArguments)
{ }
