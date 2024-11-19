using System;
using System.Collections.Generic;
using System.Linq;

namespace ObservationExample;

// This is a copy of the Stack class from StackSample

public class Stack<T>
{
    readonly LinkedList<T> elements = [];

    public int Count =>
        elements.Count;

    public bool IsEmpty =>
        Count == 0;

    public bool Contains(T element) =>
        elements.Contains(element);

    public T Peek() =>
        Count switch
        {
            0 => throw new InvalidOperationException("empty stack"),
            _ => elements.First(),
        };

    public T Pop()
    {
        T element = Peek();
        elements.RemoveFirst();
        return element;
    }

    public void Push(T element) =>
        elements.AddFirst(element);
}
