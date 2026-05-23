using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace AotRetryFact;

public class RetryTheoryTestCaseFactory : TheoryTestCaseFactory
{
    public int MaxRetries { get; set; }

    // This is called when pre-enumeration is disabled, so we get one test case for the whole data set
    protected override ICodeGenTestCase CreateDelayEnumeratedTestCase(ICodeGenTestMethod testMethod,
        string displayName,
        IReadOnlyCollection<Func<ICodeGenTestCase, ValueTask<IReadOnlyCollection<ICodeGenTest>>>> testFactories,
        string? displayNameSuffix = null) =>
            new RetryTestCase(
                Explicit,
                MaxRetries,
                SkipExceptions,
                SkipReason,
                SkipUnless,
                SkipWhen,
                testMethod.SourceFilePath,
                testMethod.SourceLineNumber,
                $"{displayName}{displayNameSuffix}",
                testFactories,
                testMethod,
                Timeout,
                testMethod.Traits,
                $"{UniqueIDGenerator.ForTestCase(testMethod.UniqueID, index: 0)}{displayNameSuffix}"
            );

    // This is called when pre-enumeration is enabled, to create a test case for the given data row
    protected override ICodeGenTestCase CreatePreEnumeratedTestCase(
        ICodeGenTestMethod testMethod,
        string displayName,
        ITheoryDataRow dataRow,
        Func<object?, ValueTask> methodInvoker,
        int testCaseIndex,
        string? displayNameSuffix = null,
        string? displayNameIndex = null)
    {
        var testDisplayName = GetTestDisplayName(dataRow, displayName, displayNameSuffix, displayNameIndex);
        var mergedTraits = MergeTraits(testMethod.Traits, dataRow.Traits);

        var skipReason = SkipReason;
        var skipUnless = SkipUnless;
        var skipWhen = SkipWhen;

        if (dataRow.Skip is not null)
        {
            skipReason = dataRow.Skip;
            skipUnless = dataRow.SkipUnless;
            skipWhen = dataRow.SkipWhen;
        }

        return new RetryTestCase(
            dataRow.Explicit ?? Explicit,
            MaxRetries,
            SkipExceptions,
            skipReason,
            skipUnless,
            skipWhen,
            testMethod.SourceFilePath,
            testMethod.SourceLineNumber,
            testDisplayName,
            [
                async testCase => [new CodeGenTest(
                    dataRow.Explicit ?? Explicit,
                    methodInvoker,
                    skipReason,
                    skipUnless,
                    skipWhen,
                    testCase,
                    testDisplayName,
                    dataRow.Label,
                    dataRow.Timeout ?? Timeout,
                    mergedTraits,
                    UniqueIDGenerator.ForTest(testCase.UniqueID, testIndex: 0)
                )]
            ],
            testMethod,
            dataRow.Timeout ?? Timeout,
            mergedTraits,
            $"{UniqueIDGenerator.ForTestCase(testMethod.UniqueID, testCaseIndex)}{displayNameSuffix}"
        );
    }
}
