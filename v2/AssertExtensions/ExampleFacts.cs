using System;
using Xunit;
using Xunit.Extensions.AssertExtensions;

public class ExampleFacts
{
    public class BooleanFacts
    {
        [Fact]
        public void ShouldBeTrue() =>
            true.ShouldBeTrue();

        [Fact]
        public void ShouldBeFalse() =>
            false.ShouldBeFalse();

        [Fact]
        public void ShouldBeTrueWithMessage()
        {
            var ex = Record.Exception(() => false.ShouldBeTrue("should be true"));

            Assert.StartsWith("should be true", ex.Message);
        }

        [Fact]
        public void ShouldBeFalseWithMessage()
        {
            var ex = Record.Exception(() => true.ShouldBeFalse("should be false"));

            Assert.StartsWith("should be false", ex.Message);
        }
    }

    public class CollectionFacts
    {
        [Fact]
        public void ShouldBeEmpty() =>
            Array.Empty<int>().ShouldBeEmpty();

        [Fact]
        public void ShouldContain() =>
            new[] { 1, 2, 3 }.ShouldContain(2);

        [Fact]
        public void ShouldNotBeEmpty() =>
            new[] { 1, 2, 3 }.ShouldNotBeEmpty();

        [Fact]
        public void ShouldNotContain() =>
            new[] { 1, 2, 3 }.ShouldNotContain(5);
    }
}
