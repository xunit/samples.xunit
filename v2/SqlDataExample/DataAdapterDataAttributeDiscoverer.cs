using Xunit.Abstractions;
using Xunit.Sdk;

public class DataAdapterDataAttributeDiscoverer : DataDiscoverer
{
    public override bool SupportsDiscoveryEnumeration(IAttributeInfo dataAttribute, IMethodInfo testMethod)
        => dataAttribute.GetNamedArgument<bool>("EnableDiscoveryEnumeration");
}
