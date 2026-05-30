using System.Collections;

namespace DataStructures.StackQueue;

/// <summary>
/// A LIFO stack backed by a singly linked chain. Unlike
/// <see cref="ArrayStack{T}"/> it never resizes — every push is true O(1)
/// with no amortized doubling — at the cost of one allocation per element
/// and worse cache locality.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class LinkedStack<T> : IEnumerable<T>
{
    private sealed class Node(T value, Node? next)
    {
        public readonly T Value = value;
        public readonly Node? Next = next;
    }

    private Node? _top;

    /// <summary>Number of elements. O(1).</summary>
    public int Count { get; private set; }

    /// <summary>Pushes <paramref name="value"/> onto the top. O(1), one allocation.</summary>
    public void Push(T value)
    {
        _top = new Node(value, _top);
        Count++;
    }

    /// <summary>Removes and returns the top. Throws if empty. O(1).</summary>
    public T Pop()
    {
        if (_top is null)
        {
            throw new InvalidOperationException("Stack is empty.");
        }
        var value = _top.Value;
        _top = _top.Next;
        Count--;
        return value;
    }

    /// <summary>Returns the top without removing it. Throws if empty. O(1).</summary>
    public T Peek()
    {
        if (_top is null)
        {
            throw new InvalidOperationException("Stack is empty.");
        }
        return _top.Value;
    }

    /// <summary>Enumerates from top to bottom (LIFO order). O(n).</summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (var current = _top; current is not null; current = current.Next)
        {
            yield return current.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
