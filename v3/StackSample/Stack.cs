using System;
using System.Collections.Generic;
using System.Linq;

namespace StackSample;

public class Stack<T>
{
    readonly List<T> elements = [];

    public int Count =>
        elements.Count;

    public bool Contains(T element) =>
        elements.Contains(element);

    public T Peek() =>
        Count switch
        {
            0 => throw new InvalidOperationException("empty stack"),
            _ => elements.Last(),
        };

    public T Pop()
    {
        T element = Peek();
        elements.RemoveAt(Count - 1);
        return element;
    }

    public void Push(T element) =>
        elements.Add(element);
}
