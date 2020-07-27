using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestCase : TestMethodTestCase
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public ObservationTestCase() { }

        public ObservationTestCase(TestMethodDisplay defaultMethodDisplay, TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod)
            : base(defaultMethodDisplay, defaultMethodDisplayOptions, testMethod) { }

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