using System;
using System.Collections.Generic;

class Stack<T>
{
    readonly LinkedList<T> elements = new LinkedList<T>();

    public bool IsEmpty
    {
        get { return elements.Count == 0; }
    }

    public void Push(T element)
    {
        elements.AddFirst(element);
    }

    public T Peek
    {
        get
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("cannot top an empty stack");

            return elements.First.Value;
        }
    }

    public T Pop()
    {
        T top = Peek;
        elements.RemoveFirst();

        return top;
    }
}