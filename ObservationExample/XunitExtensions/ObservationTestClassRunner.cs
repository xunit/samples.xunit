using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestClassRunner : TestClassRunner<ObservationTestCase>
    {
        readonly Specification specification;

        public ObservationTestClassRunner(Specification specification, ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<ObservationTestCase> testCases, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testClass, @class, testCases, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            this.specification = specification;
        }

        protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<ObservationTestCase> testCases, object[] constructorArguments)
        {
            return new ObservationTestMethodRunner(specification, testMethod, Class, method, testCases, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource).RunAsync();
        }
    }
}
