using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestCase : TestMethodTestCase
    {
        [Obsolete("For de-serialization purposes only", error: true)]
        public ObservationTestCase() { }

        public ObservationTestCase(TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod)
            : base(defaultMethodDisplay, testMethod) { }

        protected override void Initialize()
        {
            base.Initialize();

            DisplayName = String.Format("{0}, it {1}", TestMethod.TestClass.Class.Name, TestMethod.Method.Name).Replace('_', ' ');
        }

        public Task<RunSummary> RunAsync(Specification specification,
                                         IMessageBus messageBus,
                                         ExceptionAggregator aggregator,
                                         CancellationTokenSource cancellationTokenSource)
        {
            return new ObservationTestCaseRunner(specification, this, DisplayName, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }
    }
}