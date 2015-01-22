using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using Xunit.Serialization;

namespace RetryFactExample
{
    [Serializable]
    public class RetryTestCase : XunitTestCase
    {
        private int maxRetries;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", true)]
        public RetryTestCase() { }

        public RetryTestCase(TestMethodDisplay testMethodDisplay, ITestMethod testMethod, int maxRetries)
            : base(testMethodDisplay, testMethod, testMethodArguments: null)
        {
            this.maxRetries = maxRetries;
        }

        // This method is called by the xUnit test framework classes to run the test case. We will do the
        // loop here, forwarding on to the implementation in XunitTestCase to do the heavy lifting. We will
        // continue to re-run the test until the aggregator has an error (meaning that some internal error
        // condition happened), or the test runs without failure, or we've hit the maximum number of tries.
        public override async Task<RunSummary> RunAsync(IMessageBus messageBus,
                                                        object[] constructorArguments,
                                                        ExceptionAggregator aggregator,
                                                        CancellationTokenSource cancellationTokenSource)
        {
            var runCount = 0;

            while (true)
            {
                // This is really the only tricky bit: we need to capture and delay messages (since those will
                // contain run status) until we know we've decided to accept the final result;
                var delayedMessageBus = new DelayedMessageBus(messageBus);

                var summary = await base.RunAsync(delayedMessageBus, constructorArguments, aggregator, cancellationTokenSource);
                if (aggregator.HasExceptions || summary.Failed == 0 || ++runCount >= maxRetries)
                {
                    delayedMessageBus.Dispose();  // Sends all the delayed messages
                    return summary;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////
        // Serialization code

        // TODO: Can we do something about the doubled up code here?

        public override void GetData(XunitSerializationInfo data)
        {
            base.GetData(data);

            data.AddValue("MaxRetries", maxRetries);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("MaxRetries", maxRetries);
        }

        protected RetryTestCase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            maxRetries = info.GetInt32("MaxRetries");
        }

        public override void SetData(XunitSerializationInfo data)
        {
            base.SetData(data);

            maxRetries = (int)data.GetValue("MaxRetries", typeof(int));
        }
    }
}
