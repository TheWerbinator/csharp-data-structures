using System.Collections;

namespace DataStructures.LinkedList;

/// <summary>
/// A doubly linked list supporting O(1) insertion and removal at both ends
/// and O(1) reverse enumeration. The back-pointer is what distinguishes it
/// from <see cref="SinglyLinkedList{T}"/>: removing a known node is O(1)
/// rather than O(n), at the cost of one extra reference per node.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class DoublyLinkedList<T> : IEnumerable<T>
{
    private sealed class Node(T value)
    {
        public T Value = value;
        public Node? Prev;
        public Node? Next;
    }

    private Node? _head;
    private Node? _tail;

    /// <summary>Number of elements in the list. O(1).</summary>
    public int Count { get; private set; }

    /// <summary>Adds <paramref name="value"/> to the front. O(1).</summary>
    public void AddFirst(T value)
    {
        var node = new Node(value) { Next = _head };
        if (_head is not null)
        {
            _head.Prev = node;
        }
        _head = node;
        _tail ??= node;
        Count++;
    }

    /// <summary>Adds <paramref name="value"/> to the end. O(1).</summary>
    public void AddLast(T value)
    {
        var node = new Node(value) { Prev = _tail };
        if (_tail is not null)
        {
            _tail.Next = node;
        }
        _tail = node;
        _head ??= node;
        Count++;
    }

    /// <summary>Removes and returns the first element. Throws if empty. O(1).</summary>
    public T RemoveFirst()
    {
        if (_head is null)
        {
            throw new InvalidOperationException("List is empty.");
        }
        var value = _head.Value;
        _head = _head.Next;
        if (_head is null)
        {
            _tail = null;
        }
        else
        {
            _head.Prev = null;
        }
        Count--;
        return value;
    }

    /// <summary>Removes and returns the last element. Throws if empty. O(1).</summary>
    public T RemoveLast()
    {
        if (_tail is null)
        {
            throw new InvalidOperationException("List is empty.");
        }
        var value = _tail.Value;
        _tail = _tail.Prev;
        if (_tail is null)
        {
            _head = null;
        }
        else
        {
            _tail.Next = null;
        }
        Count--;
        return value;
    }

    /// <summary>Returns <see langword="true"/> if the list contains <paramref name="value"/>. O(n).</summary>
    public bool Contains(T value)
    {
        for (var current = _head; current is not null; current = current.Next)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, value))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>Enumerates from tail to head. O(n).</summary>
    public IEnumerable<T> EnumerateReverse()
    {
        for (var current = _tail; current is not null; current = current.Prev)
        {
            yield return current.Value;
        }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        for (var current = _head; current is not null; current = current.Next)
        {
            yield return current.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
