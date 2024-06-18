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

        public class RetryTheorySample : IClassFixture<CounterFixture>
        {
            private readonly CounterFixture counter;

            public RetryTheorySample(CounterFixture counter)
            {
                this.counter = counter;

                counter.RunCount++;
            }

            [RetryTheory(MaxRetries = 2)]
            [InlineData(2)]    // Will fail once then pass
            [InlineData(100)]  // Will fail twice
            public void TheoryMethod(int expectedCount)
            {
                // This test is a little sketchy, since we have a single shared counter for all
                // data items, so we use >= rather than Equal, and assume the test method will run
                // enough to make 2 pass but not enough to make 100 pass.
                Assert.True(counter.RunCount >= expectedCount);
            }
        }
    }
}
