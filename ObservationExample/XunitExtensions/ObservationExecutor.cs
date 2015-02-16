using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationExecutor : TestFrameworkExecutor<ObservationTestCase>
    {
        public ObservationExecutor(AssemblyName assemblyName,
                                   ISourceInformationProvider sourceInformationProvider,
                                   IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink) { }

        protected override ITestFrameworkDiscoverer CreateDiscoverer()
        {
            return new ObservationDiscoverer(AssemblyInfo, SourceInformationProvider, DiagnosticMessageSink);
        }

        protected override async void RunTestCases(IEnumerable<ObservationTestCase> testCases,
                                                   IMessageSink executionMessageSink,
                                                   ITestFrameworkExecutionOptions executionOptions)
        {
            var testAssembly = new TestAssembly(AssemblyInfo, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            using (var assemblyRunner = new ObservationAssemblyRunner(testAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions))
                await assemblyRunner.RunAsync();
        }
    }
}
