using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestMethodRunner : TestMethodRunner<ObservationTestCase>
    {
        readonly Specification specification;

        public ObservationTestMethodRunner(Specification specification,
                                           ITestMethod testMethod,
                                           IReflectionTypeInfo @class,
                                           IReflectionMethodInfo method,
                                           IEnumerable<ObservationTestCase> testCases,
                                           IMessageBus messageBus,
                                           ExceptionAggregator aggregator,
                                           CancellationTokenSource cancellationTokenSource)
            : base(testMethod, @class, method, testCases, messageBus, aggregator, cancellationTokenSource)
        {
            this.specification = specification;
        }

        protected override Task<RunSummary> RunTestCaseAsync(ObservationTestCase testCase)
        {
            return testCase.RunAsync(specification, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource);
        }
    }
}
