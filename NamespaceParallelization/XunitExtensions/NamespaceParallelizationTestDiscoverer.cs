using System.Collections.Concurrent;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NamespaceParallelization.XunitExtensions;

public sealed class NamespaceParallelizationTestDiscoverer(IAssemblyInfo assemblyInfo,
                                                           ISourceInformationProvider sourceInformationProvider,
                                                           IMessageSink diagnosticMessageSink)
    : XunitTestFrameworkDiscoverer(assemblyInfo, sourceInformationProvider, diagnosticMessageSink)
{
    ITestAssembly? testAssembly;
    readonly ConcurrentDictionary<string, ITestCollection> testCollectionsByNamespace = new();

    // Rather than relying on decorations, we set up so that each namespace is in its own test collection. That
    // way when it comes time to create the collection fixtures, we will be creating the shared state object for
    // that namespace.
    protected override ITestClass CreateTestClass(ITypeInfo @class)
    {
        testAssembly ??= new TestAssembly(AssemblyInfo);
        var namespaceIdx = @class.Name.LastIndexOf('.');
        var @namespace = namespaceIdx < 0 ? string.Empty : @class.Name.Substring(0, namespaceIdx);
        var testCollection = testCollectionsByNamespace.GetOrAdd(@namespace, ns => new TestCollection(testAssembly, null, ns));

        return new TestClass(testCollection, @class);
    }
}
