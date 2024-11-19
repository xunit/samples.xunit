using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace ObservationExample;

public class ObservationTestMethodRunner :
    TestMethodRunner<ObservationTestMethodRunnerContext, ObservationTestMethod, ObservationTestCase>
{
    public static ObservationTestMethodRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        Specification specification,
        ObservationTestMethod testMethod,
        IReadOnlyCollection<ObservationTestCase> testCases,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        await using var ctxt = new ObservationTestMethodRunnerContext(specification, testMethod, testCases, messageBus, aggregator, cancellationTokenSource);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTestCase(
        ObservationTestMethodRunnerContext ctxt,
        ObservationTestCase testCase) =>
           ObservationTestCaseRunner.Instance.Run(ctxt.Specification, testCase, ctxt.MessageBus, ctxt.Aggregator.Clone(), ctxt.CancellationTokenSource);
}

public class ObservationTestMethodRunnerContext(
    Specification specification,
    ObservationTestMethod testMethod,
    IReadOnlyCollection<ObservationTestCase> testCases,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource) :
        TestMethodRunnerContext<ObservationTestMethod, ObservationTestCase>(testMethod, testCases, ExplicitOption.Off, messageBus, aggregator, cancellationTokenSource)
{
    public Specification Specification { get; } = specification;
}
