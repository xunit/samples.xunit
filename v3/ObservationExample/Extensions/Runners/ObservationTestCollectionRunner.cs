using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace ObservationExample;

public class ObservationTestCollectionRunner :
    TestCollectionRunner<ObservationTestCollectionRunnerContext, ObservationTestCollection, ObservationTestClass, ObservationTestCase>
{
    public static ObservationTestCollectionRunner Instance { get; } = new();

    protected override ValueTask<RunSummary> FailTestClass(ObservationTestCollectionRunnerContext ctxt, ObservationTestClass? testClass, IReadOnlyCollection<ObservationTestCase> testCases, Exception exception)
    {
        var result = XunitRunnerHelper.FailTestCases(
            Guard.ArgumentNotNull(ctxt).MessageBus,
            ctxt.CancellationTokenSource,
            testCases,
            exception,
            sendTestClassMessages: true,
            sendTestMethodMessages: true
        );

        return new(result);
    }

    protected override async ValueTask<RunSummary> RunTestClass(
        ObservationTestCollectionRunnerContext ctxt,
        ObservationTestClass? testClass,
        IReadOnlyCollection<ObservationTestCase> testCases)
    {
        ArgumentNullException.ThrowIfNull(testClass);

        object? testClassInstance = null;

        // We don't use the aggregator here because we're shortcutting everything to just return failure,
        // so the exception will already be reported and doesn't need to propagated.
        try
        {
            testClassInstance = Activator.CreateInstance(testClass.Class);
        }
        catch (Exception ex)
        {
            return await FailTestClass(ctxt, testClass, testCases, ex);
        }

        if (testClassInstance is not Specification specification)
            return await FailTestClass(ctxt, testClass, testCases, new TestPipelineException($"Test class {testClass.Class.FullName} cannot be static, and must derive from Specification."));

        try
        {
            specification.OnStart();
        }
        catch (Exception ex)
        {
            return await FailTestClass(ctxt, testClass, testCases, ex);
        }

        var result = await ObservationTestClassRunner.Instance.Run(specification, testClass, testCases, ctxt.MessageBus, ctxt.Aggregator.Clone(), ctxt.CancellationTokenSource);

        ctxt.Aggregator.Run(specification.OnFinish);

        if (specification is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else if (specification is IDisposable disposable)
            disposable.Dispose();

        return result;
    }

    public async ValueTask<RunSummary> Run(
        ObservationTestCollection testCollection,
        IReadOnlyCollection<ObservationTestCase> testCases,
        IMessageBus messageBus,
        ExceptionAggregator exceptionAggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        await using var ctxt = new ObservationTestCollectionRunnerContext(testCollection, testCases, messageBus, exceptionAggregator, cancellationTokenSource);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }
}

public class ObservationTestCollectionRunnerContext(
    ObservationTestCollection testCollection,
    IReadOnlyCollection<ObservationTestCase> testCases,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource) :
        TestCollectionRunnerContext<ObservationTestCollection, ObservationTestCase>(testCollection, testCases, ExplicitOption.Off, messageBus, aggregator, cancellationTokenSource)
{ }
