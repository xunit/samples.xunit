using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestInvoker : TestInvoker<ObservationTestCase>
    {
        private readonly Specification specification;

        public ObservationTestInvoker(Specification specification,
                                      ITest test,
                                      IMessageBus messageBus,
                                      Type testClass,
                                      MethodInfo testMethod,
                                      ExceptionAggregator aggregator,
                                      CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, null, testMethod, null, aggregator, cancellationTokenSource)
        {
            this.specification = specification;
        }

        public new Task<decimal> RunAsync()
        {
            return Aggregator.RunAsync(async () =>
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    if (!CancellationTokenSource.IsCancellationRequested)
                    {
                        if (!Aggregator.HasExceptions)
                            await Timer.AggregateAsync(() => InvokeTestMethodAsync(specification));
                    }
                }

                return Timer.Total;
            });
        }
    }
}
