using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace RetryFactExample;

public class RetryTheoryDiscoverer : TheoryDiscoverer
{
    // This override is used when the theory data is serializable and the user has requested pre-enumeration.
    // It represents a single test case for a single row of data.
    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForDataRow(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        ITheoryAttribute theoryAttribute,
        ITheoryDataRow dataRow,
        object?[] testMethodArguments)
    {
        var maxRetries = (theoryAttribute as RetryTheoryAttribute)?.MaxRetries ?? 3;
        var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, theoryAttribute, testMethodArguments);
        var testCase = new RetryTestCase(
            maxRetries,
            details.ResolvedTestMethod,
            details.TestCaseDisplayName,
            details.UniqueID,
            details.Explicit,
            details.SkipReason,
            details.SkipType,
            details.SkipUnless,
            details.SkipWhen,
            testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
            testMethodArguments,
            timeout: details.Timeout
        );

        return new([testCase]);
    }

    // This override is used if pre-enumeration is disabled, or the theory data doesn't support serialization.
    // It represents a single test case which will result in multiple tests (one per row of data) at execution time.
    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForTheory(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        ITheoryAttribute theoryAttribute)
    {
        var maxRetries = (theoryAttribute as RetryTheoryAttribute)?.MaxRetries ?? 3;
        var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, theoryAttribute);
        var testCase =
            details.SkipReason is not null && details.SkipUnless is null && details.SkipWhen is null
                // Unconditionally skipped theory should yield a single XunitTestCase which will return a skipped result
                ? new XunitTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
                    timeout: details.Timeout
                )
                // Otherwise, return a test case which will enumerate the data later
                : new RetryDelayEnumeratedTestCase(
                    maxRetries,
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    theoryAttribute.SkipTestWithoutData,
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
                    timeout: details.Timeout
                );

        return new([testCase]);
    }
}
