using Xunit.Abstractions;

namespace Xunit.Sdk;

public class AsyncDataDiscoverer : DataDiscoverer
{
    // Turn off discovery enumeration since we're assuming that anything which is async requires
    // runtime infrastructure to run properly.
    public override bool SupportsDiscoveryEnumeration(IAttributeInfo dataAttribute, IMethodInfo testMethod) => false;
}
