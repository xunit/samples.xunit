using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NamespaceParallelization.XunitExtensions;

// Unfortunately we could not directly consume XunitTheoryTestCaseRunner because it was never designed for extensibility,
// so the simplest thing to do was copy all the code and just make the appropriate changes. Unfortunately it means that this
// looks like a lot of code, but the only code that really changed was RunTestAsync(), which switched from sequential to parallel.
public class NamespaceParallelizationTheoryTestCaseRunner(IXunitTestCase testCase,
                                                          string displayName,
                                                          string skipReason,
                                                          object[] constructorArguments,
                                                          IMessageSink diagnosticMessageSink,
                                                          IMessageBus messageBus,
                                                          ExceptionAggregator aggregator,
                                                          CancellationTokenSource cancellationTokenSource)
    : XunitTestCaseRunner(testCase, displayName, skipReason, constructorArguments, Array.Empty<object>(), messageBus, aggregator, cancellationTokenSource)
{
    readonly ExceptionAggregator cleanupAggregator = new();
    Exception? dataDiscoveryException;
    readonly List<XunitTestRunner> testRunners = new();
    readonly List<IDisposable> toDispose = new();

    // Copy/pasted from XunitTheoryTestCaseRunner
    protected override async Task AfterTestCaseStartingAsync()
    {
        await base.AfterTestCaseStartingAsync();

        try
        {
            var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute));

            foreach (var dataAttribute in dataAttributes)
            {
                var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                var args = discovererAttribute.GetConstructorArguments().Cast<string>().ToList();
                var discovererType = SerializationHelper.GetType(args[1], args[0]);
                if (discovererType == null)
                {
                    if (dataAttribute is IReflectionAttributeInfo reflectionAttribute)
                        Aggregator.Add(
                            new InvalidOperationException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    "Data discoverer specified for {0} on {1}.{2} does not exist.",
                                    reflectionAttribute.Attribute.GetType(),
                                    TestCase.TestMethod.TestClass.Class.Name,
                                    TestCase.TestMethod.Method.Name
                                )
                            )
                        );
                    else
                        Aggregator.Add(
                            new InvalidOperationException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    "A data discoverer specified on {0}.{1} does not exist.",
                                    TestCase.TestMethod.TestClass.Class.Name,
                                    TestCase.TestMethod.Method.Name
                                )
                            )
                        );

                    continue;
                }

                IDataDiscoverer discoverer;
                try
                {
                    discoverer = ExtensibilityPointFactory.GetDataDiscoverer(diagnosticMessageSink, discovererType);
                }
                catch (InvalidCastException)
                {
                    if (dataAttribute is IReflectionAttributeInfo reflectionAttribute)
                        Aggregator.Add(
                            new InvalidOperationException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    "Data discoverer specified for {0} on {1}.{2} does not implement IDataDiscoverer.",
                                    reflectionAttribute.Attribute.GetType(),
                                    TestCase.TestMethod.TestClass.Class.Name,
                                    TestCase.TestMethod.Method.Name
                                )
                            )
                        );
                    else
                        Aggregator.Add(
                            new InvalidOperationException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    "A data discoverer specified on {0}.{1} does not implement IDataDiscoverer.",
                                    TestCase.TestMethod.TestClass.Class.Name,
                                    TestCase.TestMethod.Method.Name
                                )
                            )
                        );

                    continue;
                }

                var data = discoverer.GetData(dataAttribute, TestCase.TestMethod.Method);
                if (data == null)
                {
                    Aggregator.Add(
                        new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "Test data returned null for {0}.{1}. Make sure it is statically initialized before this test method is called.",
                                TestCase.TestMethod.TestClass.Class.Name,
                                TestCase.TestMethod.Method.Name
                            )
                        )
                    );

                    continue;
                }

                foreach (var dataRow in data)
                {
                    toDispose.AddRange(dataRow.OfType<IDisposable>());

                    ITypeInfo[]? resolvedTypes = null;
                    var methodToRun = TestMethod;
                    var convertedDataRow = methodToRun.ResolveMethodArguments(dataRow);

                    if (methodToRun.IsGenericMethodDefinition)
                    {
                        resolvedTypes = TestCase.TestMethod.Method.ResolveGenericTypes(convertedDataRow);
                        methodToRun = methodToRun.MakeGenericMethod(resolvedTypes.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
                    }

                    var parameterTypes = methodToRun.GetParameters().Select(p => p.ParameterType).ToArray();
                    convertedDataRow = Reflector.ConvertArguments(convertedDataRow, parameterTypes);

                    var theoryDisplayName = TestCase.TestMethod.Method.GetDisplayNameWithArguments(DisplayName, convertedDataRow, resolvedTypes);
                    var test = CreateTest(TestCase, theoryDisplayName);
                    var skipReason = SkipReason ?? dataAttribute.GetNamedArgument<string>("Skip");
                    testRunners.Add(CreateTestRunner(test, MessageBus, TestClass, ConstructorArguments, methodToRun, convertedDataRow, skipReason, BeforeAfterAttributes, Aggregator, CancellationTokenSource));
                }
            }
        }
        catch (Exception ex)
        {
            // Stash the exception so we can surface it during RunTestAsync
            dataDiscoveryException = ex;

            while (dataDiscoveryException is AggregateException aggEx && aggEx.InnerException is not null)
                dataDiscoveryException = aggEx.InnerException;
        }
    }

    // Copy/pasted from XunitTheoryTestCaseRunner
    protected override Task BeforeTestCaseFinishedAsync()
    {
        Aggregator.Aggregate(cleanupAggregator);

        return base.BeforeTestCaseFinishedAsync();
    }

    // Overridden to run theory data rows in parallel instead of sequentially
    protected override async Task<RunSummary> RunTestAsync()
    {
        if (dataDiscoveryException != null)
            return RunTest_DataDiscoveryException();

        var testCaseSummary = new RunSummary();
        var tasks = new List<Task<RunSummary>>();

        foreach (var testRunner in testRunners)
            tasks.Add(Task.Run(testRunner.RunAsync));

        var testSummaries = await Task.WhenAll(tasks);
        foreach (var testSummary in testSummaries)
            testCaseSummary.Aggregate(testSummary);

        // Run the cleanup here so we can include cleanup time in the run summary,
        // but save any exceptions so we can surface them during the cleanup phase,
        // so they get properly reported as test case cleanup failures.
        var timer = new ExecutionTimer();
        foreach (var disposable in toDispose)
            timer.Aggregate(() => cleanupAggregator.Run(disposable.Dispose));

        testCaseSummary.Time += timer.Total;
        return testCaseSummary;
    }

    // Copy/pasted from XunitTheoryTestCaseRunner
    RunSummary RunTest_DataDiscoveryException()
    {
        var test = new XunitTest(TestCase, DisplayName);

        if (!MessageBus.QueueMessage(new TestStarting(test)))
            CancellationTokenSource.Cancel();
        else if (!MessageBus.QueueMessage(new TestFailed(test, 0, null, dataDiscoveryException)))
            CancellationTokenSource.Cancel();
        if (!MessageBus.QueueMessage(new TestFinished(test, 0, null)))
            CancellationTokenSource.Cancel();

        return new RunSummary { Total = 1, Failed = 1 };
    }
}
