using System.Reflection;
using Xunit.v3;

namespace NamespaceParallelization.Extensions;

public class NamespaceParallelizationTestFramework(string? configFilePath) :
    TestFramework
{
    public override string TestFrameworkDisplayName => "Namespace Parallelization Framework";

    protected override ITestFrameworkDiscoverer CreateDiscoverer(Assembly assembly) =>
        new NamespaceParallelizationTestDiscoverer(new XunitTestAssembly(assembly, configFilePath));

    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly) =>
        new NamespaceParallelizationTestExecutor(new XunitTestAssembly(assembly, configFilePath));
}
