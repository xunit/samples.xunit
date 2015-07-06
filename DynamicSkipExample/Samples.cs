using Xunit;

namespace DynamicSkipExample
{
    public class Samples
    {
        [SkippableFact]
        public void Passing() { }

        [SkippableFact(Skip = "I never feel like it")]
        public void StaticallySkipped() { }

        [SkippableFact]
        public void DynamicallySkipped()
        {
            // You could hide this behind something like "Assert.Skip", by bring in the assertion
            // library as source and extending the Assert class.
            throw new SkipTestException("I don't feel like it right now, ask again later");
        }

        [SkippableFact]
        public void Failing()
        {
            Assert.True(false);
        }
    }
}
