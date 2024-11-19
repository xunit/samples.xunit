using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xunit.v3;

namespace NamespaceParallelization.Extensions;

public sealed class NamespaceParallelizationTestDiscoverer(IXunitTestAssembly testAssembly) :
    XunitTestFrameworkDiscoverer(testAssembly)
{
    readonly ConcurrentDictionary<string, IXunitTestCollection> testCollectionsByNamespace = new();

    // Rather than relying on decorations, we set up so that each namespace is in its own test collection. That
    // way when it comes time to create the collection fixtures, we will be creating the shared state object for
    // that namespace.
    protected override ValueTask<IXunitTestClass> CreateTestClass(Type @class)
    {
        var @namespace = @class.Namespace ?? "<no namespace>";
        var testCollection = testCollectionsByNamespace.GetOrAdd(@namespace, ns => new XunitTestCollection(TestAssembly, collectionDefinition: null, disableParallelization: false, displayName: ns));

        return new(new XunitTestClass(@class, testCollection));
    }
}
