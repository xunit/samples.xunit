using Xunit;

namespace DynamicSkipExample
{
    public class Samples
    {
        public class Facts
        {
            [Fact]
            public void NonSkippableFact() { }

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

        public class Theories
        {
            [SkippableTheory(Skip = "I never feel like it")]
#pragma warning disable xUnit1003 // Since this theory is skipped, it doesn't need data
#pragma warning disable xUnit1006 // Since this theory is skipped, it doesn't need parameters
            public void StaticallySkipped() { }
#pragma warning restore xUnit1006
#pragma warning restore xUnit1003

            [SkippableTheory]
            [InlineData(null)]
            [InlineData("Hello, world!")]
            public void DynamicallySkipped(string value)
            {
                if (value == null)
                    throw new SkipTestException("I don't like to run against null data.");

                Assert.StartsWith("foo", value);
            }
        }
    }
}
