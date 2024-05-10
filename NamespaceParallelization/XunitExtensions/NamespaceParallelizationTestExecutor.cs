using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NamespaceParallelization.XunitExtensions;

public class NamespaceParallelizationTestExecutor(AssemblyName assemblyName,
                                                  ISourceInformationProvider sourceInformationProvider,
                                                  IMessageSink diagnosticMessageSink)
    : XunitTestFrameworkExecutor(assemblyName, sourceInformationProvider, diagnosticMessageSink)
{
    protected override ITestFrameworkDiscoverer CreateDiscoverer()
        => new NamespaceParallelizationTestDiscoverer(AssemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

    protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases,
                                               IMessageSink executionMessageSink,
                                               ITestFrameworkExecutionOptions executionOptions)
    {
        using var assemblyRunner = new NamespaceParallelizationTestAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions);
        await assemblyRunner.RunAsync();
    }
}
