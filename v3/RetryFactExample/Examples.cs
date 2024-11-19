using RetryFactExample;
using Xunit;

public class Examples
{
    // This uses [Fact], so the RunCount will only ever be one, and thus the test will fail.
    public class FactSample
    {
        int counter;

        // Each test gets its own class instance, so both of these tests will fail
        [Fact]
        public void IWillFail()
        {
            Assert.Equal(2, ++counter);
        }

        [Fact]
        public void IWillAlsoFail()
        {
            Assert.Equal(2, ++counter);
        }
    }

    // This uses [RetryFact], so it will run up to the number of retries. You can set a breakpoint
    // on the test method when running it to see that it is indeed called twice. We also print out
    // a diagnostic message every time we retry, to make it easier to see the retries.
    //
    // The retry reuses the same class instance, so we can just use the local counter to count
    // invocations.
    public class RetryFactSample
    {
        int counter;

        // Each test gets its own class instance, so both of these tests will fail once, then pass
        [RetryFact(MaxRetries = 2)]
        public void IWillPassTheSecondTime()
        {
            Assert.Equal(2, ++counter);
        }

        [RetryFact(MaxRetries = 2)]
        public void IWillAlsoPassTheSecondTime()
        {
            Assert.Equal(2, ++counter);
        }
    }

    // Note: this sample supports theory pre-enumeration on and off, so you can use xunit.runner.json
    // to enable or disable it, so you can trace through both paths to see how that works.
    public class RetryTheorySample
    {
        int counter;

        [RetryTheory(MaxRetries = 2)]
        [InlineData(2)]
        [InlineData(3)]
        public void TheoryMethod(int expectedCount)
        {
            ++counter;

            Assert.True(counter >= expectedCount, $"Run count was {counter}, expected at least {expectedCount}");
        }
    }
}
