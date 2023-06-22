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

        public ObservationTestCase(TestMethodDisplay defaultMethodDisplay, TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod, int order)
            : base(defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
        {
            Order = order;
        }

        protected override void Initialize()
        {
            base.Initialize();

            DisplayName = string.Format("{0}, it {1}", TestMethod.TestClass.Class.Name, TestMethod.Method.Name).Replace('_', ' ');
        }

        public int Order { get; set; }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            Order = data.GetValue<int>(nameof(Order));
        }

        public Task<RunSummary> RunAsync(Specification specification,
                                         IMessageBus messageBus,
                                         ExceptionAggregator aggregator,
                                         CancellationTokenSource cancellationTokenSource)
        {
            return new ObservationTestCaseRunner(specification, this, DisplayName, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue(nameof(Order), Order);
        }
    }
}
