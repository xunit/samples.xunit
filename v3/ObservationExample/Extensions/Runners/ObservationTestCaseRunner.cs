using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace ObservationExample;

public class ObservationTestCaseRunner :
    TestCaseRunner<ObservationTestCaseRunnerContext, ObservationTestCase, ObservationTest>
{
    public static ObservationTestCaseRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        Specification specification,
        ObservationTestCase testCase,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        await using var ctxt = new ObservationTestCaseRunnerContext(specification, testCase, messageBus, aggregator, cancellationTokenSource);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTest(
        ObservationTestCaseRunnerContext ctxt,
        ObservationTest test) =>
            ObservationTestRunner.Instance.Run(ctxt.Specification, test, ctxt.MessageBus, null, ctxt.Aggregator.Clone(), ctxt.CancellationTokenSource);
}

public class ObservationTestCaseRunnerContext(
    Specification specification,
    ObservationTestCase testCase,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource) :
        TestCaseRunnerContext<ObservationTestCase, ObservationTest>(testCase, ExplicitOption.Off, messageBus, aggregator, cancellationTokenSource)
{
    public Specification Specification { get; } = specification;

    public override IReadOnlyCollection<ObservationTest> Tests =>
        [new ObservationTest(TestCase)];
}
