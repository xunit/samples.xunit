using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationExecutor : TestFrameworkExecutor<ObservationTestCase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservationExecutor"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the test assembly.</param>
        /// <param name="sourceInformationProvider">The source line number information provider.</param>
        public ObservationExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider)
            : base(assemblyName, sourceInformationProvider) { }

        protected override ITestFrameworkDiscoverer CreateDiscoverer()
        {
            return new ObservationDiscoverer(AssemblyInfo, SourceInformationProvider);
        }

        protected override async void RunTestCases(IEnumerable<ObservationTestCase> testCases, IMessageSink messageSink, ITestFrameworkOptions executionOptions)
        {
            var testAssembly = new TestAssembly(AssemblyInfo, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            using (var assemblyRunner = new ObservationAssemblyRunner(testAssembly, testCases, messageSink, executionOptions))
                await assemblyRunner.RunAsync();
        }
    }
}
