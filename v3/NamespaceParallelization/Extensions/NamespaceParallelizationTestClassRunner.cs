using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace NamespaceParallelization.Extensions;

public class NamespaceParallelizationTestClassRunner :
    XunitTestClassRunnerBase<NamespaceParallelizationTestClassRunnerContext, IXunitTestClass, IXunitTestMethod, IXunitTestCase>
{
    public static NamespaceParallelizationTestClassRunner Instance { get; } = new();

    protected override ValueTask<object?[]> CreateTestClassConstructorArguments(NamespaceParallelizationTestClassRunnerContext ctxt)
    {
        if (ctxt.StartupObject is null)
            return new([]);

        return new([ctxt.StartupObject]);
    }

    public async ValueTask<RunSummary> Run(
        object? startupObject,
        IXunitTestClass testClass,
        IReadOnlyCollection<IXunitTestCase> testCases,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        await using var ctxt = new NamespaceParallelizationTestClassRunnerContext(startupObject, testClass, testCases, explicitOption, messageBus, aggregator, cancellationTokenSource);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTestMethod(
        NamespaceParallelizationTestClassRunnerContext ctxt,
        IXunitTestMethod? testMethod,
        IReadOnlyCollection<IXunitTestCase> testCases,
        object?[] constructorArguments)
    {
        if (testMethod is null)
            return new(XunitRunnerHelper.FailTestCases(ctxt.MessageBus, ctxt.CancellationTokenSource, testCases, "Test case '{0}' must be backed by a test method"));

        return NamespaceParallelizationTestMethodRunner.Instance.Run(testMethod, testCases, ctxt.ExplicitOption, ctxt.MessageBus, ctxt.Aggregator.Clone(), ctxt.CancellationTokenSource, constructorArguments);
    }

    // Run everything in parallel
    protected override async ValueTask<RunSummary> RunTestMethods(
        NamespaceParallelizationTestClassRunnerContext ctxt,
        Exception? exception)
    {
        object?[] constructorArguments;

        if (exception is not null)
            constructorArguments = [];
        else
        {
            constructorArguments = await CreateTestClassConstructorArguments(ctxt);
            exception = ctxt.Aggregator.ToException();
            ctxt.Aggregator.Clear();
        }

        var tasks = new List<Task<RunSummary>>();
        var testClassSummary = new RunSummary();

        foreach (var method in ctxt.TestCases.GroupBy(tc => tc.TestMethod, TestMethodComparer.Instance))
        {
            var testMethod = method.Key as IXunitTestMethod;
            var testCases = method.CastOrToReadOnlyCollection();

            if (exception is not null)
                tasks.Add(Task.Run(async () => await FailTestMethod(ctxt, testMethod, testCases, constructorArguments, exception), ctxt.CancellationTokenSource.Token));
            else
                tasks.Add(Task.Run(async () => await RunTestMethod(ctxt, testMethod, testCases, constructorArguments), ctxt.CancellationTokenSource.Token));
        }

        var testCaseSummaries = await Task.WhenAll(tasks);
        foreach (var testCaseSummary in testCaseSummaries)
            testClassSummary.Aggregate(testCaseSummary);

        return testClassSummary;
    }
}

public class NamespaceParallelizationTestClassRunnerContext(
    object? startupObject,
    IXunitTestClass testClass,
    IReadOnlyCollection<IXunitTestCase> testCases,
    ExplicitOption explicitOption,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource) :
        XunitTestClassRunnerBaseContext<IXunitTestClass, IXunitTestCase>(testClass, testCases, explicitOption, messageBus, DefaultTestCaseOrderer.Instance, aggregator, cancellationTokenSource, new("Unused"))
{
    public object? StartupObject { get; } = startupObject;
}
