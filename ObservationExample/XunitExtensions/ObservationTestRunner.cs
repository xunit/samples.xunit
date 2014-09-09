using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestRunner : TestRunner<ObservationTestCase>
    {
        readonly Specification specification;
        readonly ExecutionTimer timer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="XunitTestRunner"/> class.
        /// </summary>
        /// <param name="testCase">The test case that this invocation belongs to.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="testClass">The test class that the test method belongs to.</param>
        /// <param name="testMethod">The test method that will be invoked.</param>
        /// <param name="displayName">The display name for this test invocation.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public ObservationTestRunner(ObservationTestCase testCase,
                                     Specification specification,
                                     IMessageBus messageBus,
                                     ExecutionTimer timer,
                                     Type testClass,
                                     MethodInfo testMethod,
                                     string displayName,
                                     ExceptionAggregator aggregator,
                                     CancellationTokenSource cancellationTokenSource)
            : base(testCase, messageBus, testClass, null, testMethod, null, displayName, null, aggregator, cancellationTokenSource)
        {
            this.specification = specification;
            this.timer = timer;

            TestCase = testCase;
        }

        /// <inheritdoc/>
        protected override Task<decimal> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            return new ObservationTestInvoker(specification, TestCase, MessageBus, TestClass, TestMethod, DisplayName, aggregator, CancellationTokenSource).RunAsync();
        }
    }
}

