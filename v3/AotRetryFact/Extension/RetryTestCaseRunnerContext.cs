using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace AotRetryFact;

// This overrides the RunTest method to implement our message collection & retry logic.
// It uses a delayed message bus which collects and holds onto any message generated
// during the test case execution, and will only send those messages along once we know
// either the test runs without failing, or that the failure count is equal to the maximum
// number of allowed retries.
public class RetryTestCaseRunnerContext(
    RetryTestCase testCase,
    IReadOnlyCollection<ICodeGenTest> tests,
    ExplicitOption explicitOption,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    string displayName,
    string? skipReason,
    CancellationTokenSource cancellationTokenSource,
    FixtureMappingManager methodFixtureMappings) :
        CodeGenTestCaseRunnerBaseContext<RetryTestCase, ICodeGenTest>(
            testCase,
            tests,
            explicitOption,
            messageBus,
            aggregator,
            displayName,
            skipReason,
            cancellationTokenSource,
            methodFixtureMappings
        )
{
    public override async ValueTask<RunSummary> RunTest(ICodeGenTest test)
    {
        var runCount = 0;
        var maxRetries = TestCase.MaxRetries;

        if (maxRetries < 1)
            maxRetries = 3;

        while (true)
        {
            var delayedMessageBus = new DelayedMessageBus(MessageBus);
            var aggregator = Aggregator.Clone();
            // There's nothing special about running our test objects, so we rely on the built-in CodeGenTest
            // class and the associated CodeGenTestRunner runner to run them
            var result = await CodeGenTestRunner.Instance.Run(test, delayedMessageBus, ExplicitOption, aggregator, CancellationTokenSource, CaseFixtureMappings);

            if (!(aggregator.HasExceptions || result.Failed != 0) || ++runCount >= maxRetries)
            {
                delayedMessageBus.Dispose();  // Sends all the delayed messages
                return result;
            }

            TestContext.Current.SendDiagnosticMessage("Execution of '{0}' failed (attempt #{1}), retrying...", test.TestDisplayName, runCount);
            Aggregator.Clear();
        }
    }
}
