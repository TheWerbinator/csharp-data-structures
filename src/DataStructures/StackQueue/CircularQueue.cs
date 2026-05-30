using System.Collections;

namespace DataStructures.StackQueue;

/// <summary>
/// A FIFO queue backed by a circular (ring) buffer. Head and tail indices
/// wrap around the array with modular arithmetic, so dequeue is O(1) with no
/// element shifting — the naive "array queue that shifts on dequeue" is O(n)
/// per dequeue and is the bug this design avoids. Grows by doubling when full.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class CircularQueue<T> : IEnumerable<T>
{
    private T[] _items;
    private int _head;
    private int _tail;

    /// <summary>Creates a queue with an optional initial capacity.</summary>
    public CircularQueue(int capacity = 4)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        _items = new T[capacity];
    }

    /// <summary>Number of elements. O(1).</summary>
    public int Count { get; private set; }

    /// <summary>Adds <paramref name="value"/> to the back. Amortized O(1).</summary>
    public void Enqueue(T value)
    {
        if (Count == _items.Length)
        {
            Grow();
        }
        _items[_tail] = value;
        _tail = (_tail + 1) % _items.Length;
        Count++;
    }

    /// <summary>Removes and returns the front. Throws if empty. O(1).</summary>
    public T Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Queue is empty.");
        }
        var value = _items[_head];
        _items[_head] = default!;  // release reference for the GC
        _head = (_head + 1) % _items.Length;
        Count--;
        return value;
    }

    /// <summary>Returns the front without removing it. Throws if empty. O(1).</summary>
    public T Peek()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Queue is empty.");
        }
        return _items[_head];
    }

    private void Grow()
    {
        var grown = new T[_items.Length * 2];
        // Unroll the ring into linear order starting at _head.
        for (var i = 0; i < Count; i++)
        {
            grown[i] = _items[(_head + i) % _items.Length];
        }
        _items = grown;
        _head = 0;
        _tail = Count;
    }

    /// <summary>Enumerates from front to back (FIFO order). O(n).</summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return _items[(_head + i) % _items.Length];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
