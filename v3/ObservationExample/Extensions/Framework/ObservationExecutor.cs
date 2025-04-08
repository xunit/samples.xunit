using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace ObservationExample;

// We use ITestCase as our test case type, because we will see both ObservationTestCase objects
// as well as ExecutionErrorTestCase. ITestCase is the common denominator. We will end up dispatching
// the test cases appropriately in ObservationTestMethodRunner.
public class ObservationExecutor(ObservationTestAssembly testAssembly) :
    TestFrameworkExecutor<ITestCase>(testAssembly)
{
    public new ObservationTestAssembly TestAssembly { get; } = testAssembly;

    protected override ITestFrameworkDiscoverer CreateDiscoverer() =>
        new ObservationDiscoverer(TestAssembly);

    public override async ValueTask RunTestCases(
        IReadOnlyCollection<ITestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions,
        CancellationToken cancellationToken) =>
            await ObservationTestAssemblyRunner.Instance.Run(TestAssembly, testCases.Cast<ObservationTestCase>().ToArray(), executionMessageSink, executionOptions, cancellationToken);
}
