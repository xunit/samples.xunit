using Xunit;

namespace RetryFactExample
{
    public class Samples
    {
        // We use a fixture to hold the run count, since it gets created only once for the
        // class no matter how many times a method is run.
        public class CounterFixture
        {
            public int RunCount;
        }

        // This uses [Fact], so the RunCount will only ever be one, and thus the test will fail.
        public class FactSample : IClassFixture<CounterFixture>
        {
            private readonly CounterFixture counter;

            public FactSample(CounterFixture counter)
            {
                this.counter = counter;

                counter.RunCount++;
            }

            [Fact]
            public void IWillFail()
            {
                Assert.Equal(2, counter.RunCount);
            }
        }

        // This uses [RetryFact], so it will run up to the number of retries. You can set a breakpoint
        // on the test method when running it to see that it is indeed called twice.
        public class RetryFactSample : IClassFixture<CounterFixture>
        {
            private readonly CounterFixture counter;

            public RetryFactSample(CounterFixture counter)
            {
                this.counter = counter;

                counter.RunCount++;
            }

            [RetryFact(MaxRetries = 2)]
            public void IWillPassTheSecondTime()
            {
                Assert.Equal(2, counter.RunCount);
            }
        }
    }
}
