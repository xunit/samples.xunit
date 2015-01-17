using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    [Serializable]
    public class ObservationTestCase : TestMethodTestCase
    {
        public ObservationTestCase(ITestMethod testMethod) : base(testMethod) { }

        /// <inheritdoc/>
        protected ObservationTestCase(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            DisplayName = String.Format("{0}, it {1}", TestMethod.TestClass.Class.Name, TestMethod.Method.Name).Replace('_', ' ');
        }

        /// <inheritdoc/>
        public Task<RunSummary> RunAsync(Specification specification, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            return new ObservationTestCaseRunner(specification, this, DisplayName, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }
    }
}