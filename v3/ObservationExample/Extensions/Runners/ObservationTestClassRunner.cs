using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace ObservationExample;

public class ObservationTestClassRunner :
    TestClassRunner<ObservationTestClassRunnerContext, ObservationTestClass, ObservationTestMethod, ObservationTestCase>
{
    public static ObservationTestClassRunner Instance { get; } = new();

    protected override IReadOnlyCollection<ObservationTestCase> OrderTestCases(ObservationTestClassRunnerContext ctxt) =>
        [.. ctxt.TestCases.OrderBy(tc => tc.Order)];

    public async ValueTask<RunSummary> Run(
        Specification specification,
        ObservationTestClass testClass,
        IReadOnlyCollection<ObservationTestCase> testCases,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        await using var ctxt = new ObservationTestClassRunnerContext(specification, testClass, testCases, messageBus, aggregator, cancellationTokenSource);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTestMethod(
        ObservationTestClassRunnerContext ctxt,
        ObservationTestMethod? testMethod,
        IReadOnlyCollection<ObservationTestCase> testCases,
        object?[] constructorArguments)
    {
        ArgumentNullException.ThrowIfNull(testMethod);

        return ObservationTestMethodRunner.Instance.Run(ctxt.Specification, testMethod, testCases, ctxt.MessageBus, ctxt.Aggregator.Clone(), ctxt.CancellationTokenSource);
    }
}

public class ObservationTestClassRunnerContext(
    Specification specification,
    ObservationTestClass testClass,
    IReadOnlyCollection<ObservationTestCase> testCases,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource) :
        TestClassRunnerContext<ObservationTestClass, ObservationTestCase>(testClass, testCases, ExplicitOption.Off, messageBus, aggregator, cancellationTokenSource)
{
    public Specification Specification { get; } = specification;
}
