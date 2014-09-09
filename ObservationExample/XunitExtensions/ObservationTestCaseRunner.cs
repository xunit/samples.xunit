using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestCaseRunner : TestCaseRunner<ObservationTestCase>
    {
        readonly string displayName;
        readonly Specification specification;

        public ObservationTestCaseRunner(Specification specification,
                                         ObservationTestCase testCase,
                                         string displayName,
                                         IMessageBus messageBus,
                                         ExceptionAggregator aggregator,
                                         CancellationTokenSource cancellationTokenSource)
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
            this.specification = specification;
            this.displayName = displayName;
        }

        /// <inheritdoc/>
        protected override Task<RunSummary> RunTestAsync()
        {
            var timer = new ExecutionTimer();
            var TestClass = TestCase.TestMethod.TestClass.Class.ToRuntimeType();
            var TestMethod = TestCase.TestMethod.Method.ToRuntimeMethod();

            return new ObservationTestRunner(TestCase, specification, MessageBus, timer, TestClass, TestMethod, displayName, Aggregator, CancellationTokenSource).RunAsync();
        }
    }
}

