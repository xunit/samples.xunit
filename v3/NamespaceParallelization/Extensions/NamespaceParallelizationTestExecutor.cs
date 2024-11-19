using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace NamespaceParallelization.Extensions;

public class NamespaceParallelizationTestExecutor(IXunitTestAssembly testAssembly) :
    XunitTestFrameworkExecutor(testAssembly)
{
    protected override ITestFrameworkDiscoverer CreateDiscoverer() =>
        new NamespaceParallelizationTestDiscoverer(TestAssembly);

    public override async ValueTask RunTestCases(
        IReadOnlyCollection<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions) =>
            await NamespaceParallelizationTestAssemblyRunner.Instance.Run(TestAssembly, testCases, executionMessageSink, executionOptions);
}
