using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace ObservationExample;

public class ObservationTestAssemblyRunner :
    TestAssemblyRunner<ObservationTestAssemblyRunnerContext, ObservationTestAssembly, ObservationTestCollection, ObservationTestCase>
{
    public static ObservationTestAssemblyRunner Instance { get; } = new();

    protected override ValueTask<string> GetTestFrameworkDisplayName(ObservationTestAssemblyRunnerContext ctxt) =>
        new("Observation Framework");

    public async ValueTask<RunSummary> Run(
        ObservationTestAssembly testAssembly,
        IReadOnlyCollection<ObservationTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions,
        CancellationToken cancellationToken)
    {
        await using var ctxt = new ObservationTestAssemblyRunnerContext(testAssembly, testCases, executionMessageSink, executionOptions, cancellationToken);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTestCollection(
        ObservationTestAssemblyRunnerContext ctxt,
        ObservationTestCollection testCollection,
        IReadOnlyCollection<ObservationTestCase> testCases) =>
            ObservationTestCollectionRunner.Instance.Run(
                testCollection,
                testCases,
                ctxt.MessageBus,
                ctxt.Aggregator.Clone(),
                ctxt.CancellationTokenSource
            );
}

public class ObservationTestAssemblyRunnerContext(
    ObservationTestAssembly testAssembly,
    IReadOnlyCollection<ObservationTestCase> testCases,
    IMessageSink executionMessageSink,
    ITestFrameworkExecutionOptions executionOptions,
    CancellationToken cancellationToken) :
        TestAssemblyRunnerContext<ObservationTestAssembly, ObservationTestCase>(testAssembly, testCases, executionMessageSink, executionOptions, cancellationToken)
{ }
