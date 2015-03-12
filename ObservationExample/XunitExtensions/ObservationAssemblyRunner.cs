using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationAssemblyRunner : TestAssemblyRunner<ObservationTestCase>
    {
        public ObservationAssemblyRunner(ITestAssembly testAssembly,
                                         IEnumerable<ObservationTestCase> testCases,
                                         IMessageSink diagnosticMessageSink,
                                         IMessageSink executionMessageSink,
                                         ITestFrameworkExecutionOptions executionOptions)
            : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
        {
            TestCaseOrderer = new ObservationTestCaseOrderer();
        }

        protected override string GetTestFrameworkDisplayName()
        {
            return "Observation Framework";
        }

        protected override string GetTestFrameworkEnvironment()
        {
            return String.Format("{0}-bit .NET {1}", IntPtr.Size * 8, Environment.Version);
        }

        protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus,
                                                                   ITestCollection testCollection,
                                                                   IEnumerable<ObservationTestCase> testCases,
                                                                   CancellationTokenSource cancellationTokenSource)
        {
            return new ObservationTestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource).RunAsync();
        }
    }
}
