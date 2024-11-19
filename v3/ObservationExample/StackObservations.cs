#pragma warning disable IDE1006  // We violate the naming style here because we directly translate method names into test names

using System;
using ObservationExample;
using Xunit;

[assembly: TestFramework(typeof(ObservationTestFramework))]

// The tests here are a rough translation of the StackTests class from StackSample,
// but restructured around the idea of a Specification being more clearly defined
// than the context concept used in the original tests.

public class When_you_have_a_new_stack : Specification
{
    Stack<int> stack = default!;

    protected override void EstablishContext() =>
        stack = new();

    [Observation]
    public void should_have_a_count_of_0() =>
        Assert.Equal(0, stack.Count);

    [Observation]
    public void should_be_empty() =>
        Assert.True(stack.IsEmpty);

    [Observation]
    public void should_not_allow_you_to_call_Pop() =>
        Assert.Throws<InvalidOperationException>(() => stack.Pop());

    [Observation]
    public void should_not_allow_you_to_call_Peek() =>
        Assert.Throws<InvalidOperationException>(() => stack.Peek());
}

public class When_you_push_an_item_onto_the_stack : Specification
{
    const int expected = 42;
    Stack<int> stack = default!;

    protected override void EstablishContext() =>
        stack = new();

    protected override void Because() =>
        stack.Push(expected);

    [Observation]
    public void should_have_a_count_of_1() =>
        Assert.Equal(1, stack.Count);

    [Observation]
    public void should_not_be_empty() =>
        Assert.False(stack.IsEmpty);
}

public class When_you_push_then_pop_a_value_from_the_stack : Specification
{
    int actual;
    const int expected = 42;
    Stack<int> stack = default!;

    protected override void EstablishContext() =>
        stack = new();

    protected override void Because()
    {
        stack.Push(expected);
        actual = stack.Pop();
    }

    [Observation]
    public void should_get_the_value_that_was_pushed() =>
        Assert.Equal(expected, actual);

    [Observation]
    public void should_have_a_count_of_0() =>
        Assert.Equal(0, stack.Count);

    [Observation]
    public void should_be_empty() =>
        Assert.True(stack.IsEmpty);
}

public class When_you_push_an_item_on_the_stack_and_call_Peek : Specification
{
    int actual;
    const int expected = 42;
    Stack<int> stack = default!;

    protected override void EstablishContext() =>
        stack = new();

    protected override void Because()
    {
        stack.Push(expected);
        actual = stack.Peek();
    }

    [Observation]
    public void should_have_a_count_of_1() =>
        Assert.Equal(1, stack.Count);

    [Observation]
    public void should_not_be_empty() =>
        Assert.False(stack.IsEmpty);

    [Observation]
    public void should_return_the_last_item_pushed_onto_the_stack() =>
        Assert.Equal(expected, actual);

    [Observation]
    public void should_return_the_same_item_for_subsequent_Peek_calls()
    {
        Assert.Equal(actual, stack.Peek());
        Assert.Equal(actual, stack.Peek());
        Assert.Equal(actual, stack.Peek());
    }
}

public class When_you_push_several_items_onto_the_stack : Specification
{
    const int element1 = 10;
    const int element2 = 20;
    const int element3 = 30;
    Stack<int> stack = default!;

    protected override void EstablishContext() =>
        stack = new();

    protected override void Because()
    {
        stack.Push(element1);
        stack.Push(element2);
        stack.Push(element3);
    }

    // Every test here is order dependent. That means if you try to run an individual test, it may fail because
    // any dependent previous tests haven't run. Run this class entirely or not at all. (This is an inherent and
    // intended design of specifications.)

    [Observation(Order = 1)]
    public void should_pop_last_item_first() =>
        Assert.Equal(element3, stack.Pop());

    [Observation(Order = 2)]
    public void should_pop_second_item_second() =>
        Assert.Equal(element2, stack.Pop());

    [Observation(Order = 3)]
    public void should_pop_first_item_last() =>
        Assert.Equal(element1, stack.Pop());
}

public class When_you_throw_an_exception_during_class_construction : Specification
{
    public When_you_throw_an_exception_during_class_construction() =>
        throw new Exception();

    [Observation]
    public void should_fail_the_test()
    {
        // This test should display as having failed, even without any assertions being called
    }
}

public class When_you_throw_an_exception_during_Because_call : Specification
{
    protected override void Because() =>
        throw new Exception();

    [Observation]
    public void should_fail_the_test()
    { }
}

public class When_you_throw_an_exception_during_EstablishContext_call : Specification
{
    protected override void EstablishContext() =>
        throw new Exception();

    [Observation]
    public void should_fail_the_test()
    { }
}

public class When_you_throw_an_exception_during_DestroyContext_call : Specification
{
    protected override void DestroyContext() =>
        throw new Exception();

    [Observation]
    public void should_surface_as_collection_cleanup_failure()
    { }
}
