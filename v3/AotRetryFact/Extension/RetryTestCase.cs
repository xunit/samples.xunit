using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace AotRetryFact;

public class RetryTestCase(
    bool @explicit,
    int maxRetries,
    Type[]? skipExceptions,
    string? skipReason,
    Func<bool>? skipUnless,
    Func<bool>? skipWhen,
    string? sourceFilePath,
    int? sourceLineNumber,
    string testCaseDisplayName,
    IReadOnlyCollection<Func<ICodeGenTestCase, ValueTask<IReadOnlyCollection<ICodeGenTest>>>> testFactories,
    ICodeGenTestMethod testMethod,
    int timeout,
    IReadOnlyDictionary<string, IReadOnlyCollection<string>> traits,
    string uniqueID) :
        CodeGenTestCaseBase(
            @explicit,
            skipExceptions,
            skipReason,
            skipUnless,
            skipWhen,
            sourceFilePath,
            sourceLineNumber,
            testCaseDisplayName,
            testMethod,
            timeout,
            traits,
            uniqueID
        ), ISelfExecutingCodeGenTestCase
{
    public int MaxRetries { get; private set; } = maxRetries;

    // When the test is a RetryFact, you should end up with a singular test factory which returns
    // a singular test. This will also be true for RetryTheory when pre-enumeration is enabled.
    // When pre-enumeration is disabled, then the test factories here will end up generating one
    // test per data row.
    public override async ValueTask<IReadOnlyCollection<ICodeGenTest>> CreateTests()
    {
        var result = new List<ICodeGenTest>();

        foreach (var testFactory in testFactories)
            result.AddRange(await testFactory(this));

        return result;
    }

    // We implement ISelfExecutingCodeGenTestCase so we can use our custom test runner, which does
    // the message capture & retry logic
    public async ValueTask<RunSummary> Run(
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        FixtureMappingManager methodFixtureMappings,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        var tests = await CreateTests();

        return await RetryTestCaseRunner.Instance.Run(
            this,
            tests,
            explicitOption,
            messageBus,
            aggregator.Clone(),
            TestCaseDisplayName,
            SkipReason,
            cancellationTokenSource,
            methodFixtureMappings
        );
    }
}
