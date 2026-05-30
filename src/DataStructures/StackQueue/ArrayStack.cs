using System.Collections;

namespace DataStructures.StackQueue;

/// <summary>
/// A LIFO stack backed by a dynamically grown array, mirroring the strategy
/// <see cref="System.Collections.Generic.Stack{T}"/> uses. Amortized O(1)
/// push thanks to doubling; O(1) pop and peek.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class ArrayStack<T> : IEnumerable<T>
{
    private T[] _items;
    private int _count;

    /// <summary>Creates a stack with an optional initial capacity.</summary>
    public ArrayStack(int capacity = 4)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        _items = new T[capacity];
    }

    /// <summary>Number of elements. O(1).</summary>
    public int Count => _count;

    /// <summary>Pushes <paramref name="value"/> onto the top. Amortized O(1).</summary>
    public void Push(T value)
    {
        if (_count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length * 2);
        }
        _items[_count++] = value;
    }

    /// <summary>Removes and returns the top. Throws if empty. O(1).</summary>
    public T Pop()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }
        var value = _items[--_count];
        _items[_count] = default!;  // release reference for the GC
        return value;
    }

    /// <summary>Returns the top without removing it. Throws if empty. O(1).</summary>
    public T Peek()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }
        return _items[_count - 1];
    }

    /// <summary>Enumerates from top to bottom (LIFO order). O(n).</summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = _count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
