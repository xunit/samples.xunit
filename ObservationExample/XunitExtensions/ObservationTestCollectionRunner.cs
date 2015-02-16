using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestCollectionRunner : TestCollectionRunner<ObservationTestCase>
    {
        readonly static RunSummary FailedSummary = new RunSummary { Total = 1, Failed = 1 };

        public ObservationTestCollectionRunner(ITestCollection testCollection,
                                               IEnumerable<ObservationTestCase> testCases,
                                               IMessageBus messageBus,
                                               ITestCaseOrderer testCaseOrderer,
                                               ExceptionAggregator aggregator,
                                               CancellationTokenSource cancellationTokenSource)
            : base(testCollection, testCases, messageBus, testCaseOrderer, aggregator, cancellationTokenSource) { }

        protected override async Task<RunSummary> RunTestClassAsync(ITestClass testClass,
                                                                    IReflectionTypeInfo @class,
                                                                    IEnumerable<ObservationTestCase> testCases)
        {
            var timer = new ExecutionTimer();
            var specification = Activator.CreateInstance(testClass.Class.ToRuntimeType()) as Specification;
            if (specification == null)
            {
                Aggregator.Add(new InvalidOperationException(String.Format("Test class {0} cannot be static, and must derive from Specification.", testClass.Class.Name)));
                return FailedSummary;
            }

            Aggregator.Run(specification.OnStart);
            if (Aggregator.HasExceptions)
                return FailedSummary;

            var result = await new ObservationTestClassRunner(specification, testClass, @class, testCases, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource).RunAsync();

            Aggregator.Run(specification.OnFinish);

            var disposable = specification as IDisposable;
            if (disposable != null)
                timer.Aggregate(disposable.Dispose);

            return result;
        }
    }
}
