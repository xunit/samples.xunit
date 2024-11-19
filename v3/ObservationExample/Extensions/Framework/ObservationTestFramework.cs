using System.Reflection;
using Xunit.v3;

namespace ObservationExample;

public class ObservationTestFramework : TestFramework
{
    public override string TestFrameworkDisplayName =>
        "Observation Framework";

    protected override ITestFrameworkDiscoverer CreateDiscoverer(Assembly assembly) =>
        new ObservationDiscoverer(new ObservationTestAssembly(assembly));

    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly) =>
        new ObservationExecutor(new ObservationTestAssembly(assembly));
}
