﻿#pragma warning disable IDE1006 // We violate the naming style here because we directly translate method names into test names

using System;
using Xunit;
using XunitExtensions;

[assembly: TestFramework("XunitExtensions.ObservationTestFramework", "ObservationExample")]

public class When_you_have_a_new_stack : Specification
{
    Stack<string> stack;

    protected override void Because() =>
        stack = new Stack<string>();

    [Observation]
    public void should_be_empty() =>
        Assert.True(stack.IsEmpty);

    [Observation]
    public void should_not_allow_you_to_call_Pop() =>
        Assert.Throws<InvalidOperationException>(() => stack.Pop());

    [Observation]
    public void should_not_allow_you_to_call_Peek() =>
        Assert.Throws<InvalidOperationException>(() => { string unused = stack.Peek; });
}

public class When_you_push_an_item_onto_the_stack : Specification
{
    Stack<string> stack;

    protected override void EstablishContext() =>
        stack = new Stack<string>();

    protected override void Because() =>
        stack.Push("first element");

    [Observation]
    public void should_not_be_empty() =>
        Assert.False(stack.IsEmpty);
}

public class When_you_push_null_onto_the_stack : Specification
{
    Stack<string> stack;

    protected override void EstablishContext() =>
        stack = new Stack<string>();

    protected override void Because() =>
        stack.Push(null);

    [Observation]
    public void should_not_be_empty() =>
        Assert.False(stack.IsEmpty);

    [Observation]
    public void should_return_null_when_calling_Peek() =>
        Assert.Null(stack.Peek);

    // Order dependent: calling Pop before any of the above tests would cause them to fail.
    // The default order is 0, so "Order = 1" means we run after all our peers.
    [Observation(Order = 1)]
    public void should_return_null_when_calling_Pop() =>
        Assert.Null(stack.Pop());
}

public class When_you_push_then_pop_a_value_from_the_stack : Specification
{
    Stack<string> stack;
    const string expected = "first element";
    string actual;

    protected override void EstablishContext() =>
        stack = new Stack<string>();

    protected override void Because()
    {
        stack.Push(expected);
        actual = stack.Pop();
    }

    [Observation]
    public void should_get_the_value_that_was_pushed() =>
        Assert.Equal(expected, actual);

    [Observation]
    public void should_be_empty_again() =>
        Assert.True(stack.IsEmpty);
}

public class When_you_push_an_item_on_the_stack_and_call_Peek : Specification
{
    Stack<string> stack;
    const string expected = "first element";
    string actual;

    protected override void EstablishContext() =>
        stack = new Stack<string>();

    protected override void Because()
    {
        stack.Push(expected);
        actual = stack.Peek;
    }

    [Observation]
    public void should_not_modify_the_stack() =>
        Assert.False(stack.IsEmpty);

    [Observation]
    public void should_return_the_last_item_pushed_onto_the_stack() =>
        Assert.Equal(expected, actual);

    [Observation]
    public void should_return_the_same_item_for_subsequent_Peek_calls()
    {
        Assert.Equal(actual, stack.Peek);
        Assert.Equal(actual, stack.Peek);
        Assert.Equal(actual, stack.Peek);
    }
}

public class When_you_push_several_items_onto_the_stack : Specification
{
    Stack<string> stack;
    const string firstElement = "firstElement";
    const string secondElement = "secondElement";
    const string thirdElement = "thirdElement";

    protected override void EstablishContext() =>
        stack = new Stack<string>();

    protected override void Because()
    {
        stack.Push(firstElement);
        stack.Push(secondElement);
        stack.Push(thirdElement);
    }

    // Every test here is order dependent. That means if you try to run an individual test, it'll fail because
    // the dependent previous tests haven't run.

    [Observation(Order = 1)]
    public void should_Pop_last_item_first()
    {
        Assert.Equal(thirdElement, stack.Pop());
    }

    [Observation(Order = 2)]
    public void should_Pop_second_item_second()
    {
        Assert.Equal(secondElement, stack.Pop());
    }

    [Observation(Order = 3)]
    public void should_Pop_first_item_last()
    {
        Assert.Equal(firstElement, stack.Pop());
    }
}

public class When_you_throw_an_exception_during_class_construction : Specification
{
    public When_you_throw_an_exception_during_class_construction() =>
        throw new Exception();

    [Observation]
    public void should_fail()
    {
        // This test should display as having failed, even without any assertions being called
    }
}

public class When_you_throw_an_exception_during_because_call : Specification
{
    protected override void Because() =>
        throw new Exception();

    [Observation]
    public void should_fail()
    {
        // This test should display as having failed, even without any assertions being called
    }
}
