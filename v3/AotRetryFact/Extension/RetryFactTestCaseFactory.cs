using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace AotRetryFact;

public class RetryFactTestCaseFactory : FactTestCaseFactory
{
    public int MaxRetries { get; set; }

    protected override async ValueTask<IReadOnlyCollection<ICodeGenTestCase>> GenerateTestCases(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ICodeGenTestMethod testMethod,
        DisposalTracker disposalTracker,
        string displayName) =>
            [new RetryTestCase(
                Explicit,
                MaxRetries,
                SkipExceptions,
                SkipReason,
                SkipUnless,
                SkipWhen,
                Guard.ArgumentNotNull(testMethod).SourceFilePath,
                testMethod.SourceLineNumber,
                displayName,
                [async testCase => [GenerateTest(testCase, MethodInvoker)]],
                testMethod,
                Timeout,
                testMethod.Traits,
                UniqueIDGenerator.ForTestCase(testMethod.UniqueID, 0)
            )];
}
