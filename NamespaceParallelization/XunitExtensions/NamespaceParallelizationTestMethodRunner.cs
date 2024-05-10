using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NamespaceParallelization.XunitExtensions;

#pragma warning disable CS9107  // Warning about captured parameters that might be available in base types, but in this case they're not

public class NamespaceParallelizationTestMethodRunner(ITestMethod testMethod,
                                                      IReflectionTypeInfo @class,
                                                      IReflectionMethodInfo method,
                                                      IEnumerable<IXunitTestCase> testCases,
                                                      IMessageSink diagnosticMessageSink,
                                                      IMessageBus messageBus,
                                                      ExceptionAggregator aggregator,
                                                      CancellationTokenSource cancellationTokenSource,
                                                      object[] constructorArguments)
    : XunitTestMethodRunner(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)
{
    // Override this so if we see XunitTheoryTestCase, we use a runner that will run all the individual data rows in parallel
    protected override Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
    {
        if (testCase is XunitTheoryTestCase theoryTestCase)
            return new NamespaceParallelizationTheoryTestCaseRunner(theoryTestCase, theoryTestCase.DisplayName, theoryTestCase.SkipReason, constructorArguments, diagnosticMessageSink, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource).RunAsync();
        else
            return testCase.RunAsync(diagnosticMessageSink, MessageBus, constructorArguments, new ExceptionAggregator(Aggregator), CancellationTokenSource);
    }

    // Override to run everything in parallel instead of sequentially
    protected override async Task<RunSummary> RunTestCasesAsync()
    {
        var testMethodSummary = new RunSummary();
        var tasks = new List<Task<RunSummary>>();

        foreach (var testCase in TestCases)
            tasks.Add(Task.Run(() => RunTestCaseAsync(testCase), CancellationTokenSource.Token));

        var testCaseSummaries = await Task.WhenAll(tasks);
        foreach (var testCaseSummary in testCaseSummaries)
            testMethodSummary.Aggregate(testCaseSummary);

        return testMethodSummary;
    }
}
