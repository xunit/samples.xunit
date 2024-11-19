using System.Collections.Generic;
using ObservationExample;
using Xunit.Sdk;

public class ObservationTest(ObservationTestCase testCase) :
    ITest
{
    public ObservationTestCase TestCase { get; } = testCase;

    ITestCase ITest.TestCase => TestCase;

    public string TestDisplayName { get; } = testCase.TestCaseDisplayName;

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits =>
        TestCase.Traits;

    public string UniqueID =>
        UniqueIDGenerator.ForTest(TestCase.UniqueID, 0);
}
