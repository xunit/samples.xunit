using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NamespaceParallelization.XunitExtensions;

public class NamespaceParallelizationTestFramework(IMessageSink diagnosticMessageSink)
    : TestFramework(diagnosticMessageSink)
{
    protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
        => new NamespaceParallelizationTestDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        => new NamespaceParallelizationTestExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
}
