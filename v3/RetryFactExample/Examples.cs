using RetryFactExample;
using Xunit;

public class Examples
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
        readonly CounterFixture counter;

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
    // on the test method when running it to see that it is indeed called twice. We also print out
    // a diagnostic message every time we retry, to make it easier to see the retries.
    public class RetryFactSample : IClassFixture<CounterFixture>
    {
        readonly CounterFixture counter;

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
        readonly CounterFixture counter;

        public RetryTheorySample(CounterFixture counter)
        {
            this.counter = counter;

            counter.RunCount++;
        }

        [RetryTheory(MaxRetries = 2)]
        [InlineData(2)]  // Will fail once then pass
        [InlineData(5)]  // Will fail twice
        public void TheoryMethod(int expectedCount)
        {
            // This test uses >= rather than ==, because depending on ordering, this test
            // will run between 3 and 4 times (depending on which data row runs first), so
            // choosing 5 ensures we'll always fail.
            Assert.True(counter.RunCount >= expectedCount);
        }
    }
}
