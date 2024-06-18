using System;
using Xunit;

namespace StackSample.Tests;

// The tests in this file are structured using nested classes, in a "context" style. This means
// that each nested class is a context, meaning: there is a shared setup that puts the tests
// into the correct environment.
//
// Expectations List:
//
//   Context: Empty Stack
//     Verify that Count is 0
//     Call Pop, Verify InvalidOperationException is thrown
//     Call Peek, Verify InvalidOperationException is thrown
//     Call Contains, Verify that it returns false
//
//   Context: Create a Stack, Push a single value
//     Verify that Count is 1
//     Call Pop, Verify Count is 0
//     Call Peek, Verify Count is 1
//     Call Pop, Verify Pop returns the pushed integer
//     Call Peek, Verify Peek returns the pushed integer
//
//   Context: Create a Stack, Push multiple values
//     Push 3 ints, verify that Count is 3
//     Push 3 ints 10, 20, 30, Call Pop three times, verify that they are removed 30, 20, 10
//     Create a Stack, Push(10), Push(20), Push(30), call Contains(20), verify that it returns true
//
//   Context: Create a Stack<string> to test a different type
//     Push("Help"), call Pop, verify that what is returned from Pop equals "Help"

public class StackTests
{
    public class EmptyStack
    {
        Stack<int> stack;

        public EmptyStack() =>
            stack = new();

        [Fact]
        public void Count_ShouldReturnZero()
        {
            var count = stack.Count;

            Assert.Equal(0, count);
        }

        [Fact]
        public void Contains_ShouldReturnFalse()
        {
            var contains = stack.Contains(10);

            Assert.False(contains);
        }

        [Fact]
        public void Pop_ShouldThrowInvalidOperationException()
        {
            var exception = Record.Exception(() => stack.Pop());

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void Peek_ShouldThrowInvalidOperationException()
        {
            var exception = Record.Exception(() => stack.Peek());

            Assert.IsType<InvalidOperationException>(exception);
        }
    }

    public class StackWithOneElement
    {
        const int PushedValue = 42;

        readonly Stack<int> stack;

        public StackWithOneElement()
        {
            stack = new();
            stack.Push(PushedValue);
        }

        [Fact]
        public void Count_ShouldBeOne()
        {
            var count = stack.Count;

            Assert.Equal(1, count);
        }

        [Fact]
        public void Pop_CountShouldBeZero()
        {
            stack.Pop();

            var count = stack.Count;

            Assert.Equal(0, count);
        }

        [Fact]
        public void Peek_CountShouldBeOne()
        {
            stack.Peek();

            var count = stack.Count;

            Assert.Equal(1, count);
        }

        [Fact]
        public void Pop_ShouldReturnPushedValue()
        {
            var actual = stack.Pop();

            Assert.Equal(PushedValue, actual);
        }

        [Fact]
        public void Peek_ShouldReturnPushedValue()
        {
            var actual = stack.Peek();

            Assert.Equal(PushedValue, actual);
        }
    }

    public class StackWithMultipleValues
    {
        const int FirstPushedValue = 42;
        const int SecondPushedValue = 21;
        const int ThirdPushedValue = 11;

        readonly Stack<int> stack;

        public StackWithMultipleValues()
        {
            stack = new();
            stack.Push(FirstPushedValue);
            stack.Push(SecondPushedValue);
            stack.Push(ThirdPushedValue);
        }

        [Fact]
        public void Count_ShouldBeThree()
        {
            var count = stack.Count;

            Assert.Equal(3, count);
        }

        [Fact]
        public void Pop_VerifyLifoOrder()
        {
            Assert.Equal(ThirdPushedValue, stack.Pop());
            Assert.Equal(SecondPushedValue, stack.Pop());
            Assert.Equal(FirstPushedValue, stack.Pop());
        }

        [Fact]
        public void Peek_ReturnsLastPushedValue()
        {
            var actual = stack.Peek();

            Assert.Equal(ThirdPushedValue, actual);
        }

        [Fact]
        public void Contains_ReturnsTrue()
        {
            var contains = stack.Contains(SecondPushedValue);

            Assert.True(contains);
        }
    }

    public class StackWithStrings
    {
        [Fact]
        public void Pop_ShouldReturnPushedValue()
        {
            var stack = new Stack<string>();
            stack.Push("Help");

            string actual = stack.Pop();

            Assert.Equal("Help", actual);
        }
    }
}
