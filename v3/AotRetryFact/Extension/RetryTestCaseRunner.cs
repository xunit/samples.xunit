using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace AotRetryFact;

// We need a custom runner so we can use a custom context to do our retry logic
public class RetryTestCaseRunner : CodeGenTestCaseRunnerBase<RetryTestCaseRunnerContext, RetryTestCase, ICodeGenTest>
{
    public static RetryTestCaseRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        RetryTestCase testCase,
        IReadOnlyCollection<ICodeGenTest> tests,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        string displayName,
        string? skipReason,
        CancellationTokenSource cancellationTokenSource,
        FixtureMappingManager methodFixtureMappings)
    {
        await using var ctxt = new RetryTestCaseRunnerContext(
            testCase,
            tests,
            explicitOption,
            messageBus,
            aggregator,
            displayName,
            skipReason,
            cancellationTokenSource,
            methodFixtureMappings
        );
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }
}
