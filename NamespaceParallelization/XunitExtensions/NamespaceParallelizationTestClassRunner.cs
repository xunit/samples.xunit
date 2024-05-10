using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NamespaceParallelization.XunitExtensions;

public class NamespaceParallelizationTestClassRunner(ITestClass testClass,
                                                     IReflectionTypeInfo @class,
                                                     IEnumerable<IXunitTestCase> testCases,
                                                     IMessageSink diagnosticMessageSink,
                                                     IMessageBus messageBus,
                                                     ITestCaseOrderer testCaseOrderer,
                                                     ExceptionAggregator aggregator,
                                                     CancellationTokenSource cancellationTokenSource,
                                                     IDictionary<Type, object> collectionFixtureMappings)
    : XunitTestClassRunner(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
{
    // Don't allow users to do class fixtures, so we skip this entirely
    protected override Task AfterTestClassStartingAsync()
        => Task.CompletedTask;

    protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod,
                                                           IReflectionMethodInfo method,
                                                           IEnumerable<IXunitTestCase> testCases,
                                                           object[] constructorArguments)
        => new NamespaceParallelizationTestMethodRunner(testMethod, Class, method, testCases, DiagnosticMessageSink, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments).RunAsync();

    // Override so we can run everything in parallel (and skip test case ordering as it's irrelevant)
    protected override async Task<RunSummary> RunTestMethodsAsync()
    {
        var testClassSummary = new RunSummary();
        var constructorArguments = CreateTestClassConstructorArguments();
        var tasks = new List<Task<RunSummary>>();

        foreach (var method in TestCases.GroupBy(tc => tc.TestMethod, TestMethodComparer.Instance))
            tasks.Add(Task.Run(() => RunTestMethodAsync(method.Key, (IReflectionMethodInfo)method.Key.Method, method, constructorArguments), CancellationTokenSource.Token));

        var testCaseSummaries = await Task.WhenAll(tasks);
        foreach (var testCaseSummary in testCaseSummaries)
            testClassSummary.Aggregate(testCaseSummary);

        return testClassSummary;
    }
}
