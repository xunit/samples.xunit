using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace RetryFactExample;

public class RetryTestRunner :
    XunitTestRunnerBase<RetryTestRunnerContext, IXunitTest>
{
    public static RetryTestRunner Instance { get; } = new();

    protected override async ValueTask<TimeSpan> InvokeTest(
        RetryTestRunnerContext ctxt,
        object? testClassInstance)
    {
        var runCount = 0;
        var maxRetries = ctxt.MaxRetries;

        if (maxRetries < 1)
            maxRetries = 3;

        while (true)
        {
            // This is really the only tricky bit: we need to capture and delay messages (since those will
            // contain run status) until we know we've decided to accept the final result.
            var delayedMessageBus = new DelayedMessageBus(ctxt.MessageBus);

            var result = await base.InvokeTest(ctxt, testClassInstance);
            if (!ctxt.Aggregator.HasExceptions || ++runCount >= maxRetries)
            {
                delayedMessageBus.Dispose();  // Sends all the delayed messages
                return result;
            }

            TestContext.Current.SendDiagnosticMessage("Execution of '{0}' failed (attempt #{1}), retrying...", ctxt.Test.TestDisplayName, runCount);
            ctxt.Aggregator.Clear();
        }
    }

    public async ValueTask<RunSummary> Run(
        int maxRetries,
        IXunitTest test,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExplicitOption explicitOption,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes)
    {
        await using var ctxt = new RetryTestRunnerContext(
            maxRetries,
            test,
            messageBus,
            explicitOption,
            aggregator,
            cancellationTokenSource,
            beforeAfterAttributes,
            constructorArguments
        );
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }
}

public class RetryTestRunnerContext(
    int maxRetries,
    IXunitTest test,
    IMessageBus messageBus,
    ExplicitOption explicitOption,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterTestAttributes,
    object?[] constructorArguments) :
        XunitTestRunnerBaseContext<IXunitTest>(test, messageBus, explicitOption, aggregator, cancellationTokenSource, beforeAfterTestAttributes, constructorArguments)
{
    public int MaxRetries { get; } = maxRetries;
}
