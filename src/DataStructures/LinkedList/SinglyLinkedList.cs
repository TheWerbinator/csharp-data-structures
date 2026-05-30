using System.Collections;

namespace DataStructures.LinkedList;

/// <summary>
/// A singly linked list with O(1) head insertion and O(1) tail insertion
/// (a tail pointer is maintained). Implements <see cref="IEnumerable{T}"/>
/// so it composes with LINQ and <c>foreach</c>.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class SinglyLinkedList<T> : IEnumerable<T>
{
    private sealed class Node(T value)
    {
        public T Value = value;
        public Node? Next;
    }

    private Node? _head;
    private Node? _tail;

    /// <summary>Number of elements in the list. O(1).</summary>
    public int Count { get; private set; }

    /// <summary>Adds <paramref name="value"/> to the front of the list. O(1).</summary>
    public void AddFirst(T value)
    {
        var node = new Node(value) { Next = _head };
        _head = node;
        _tail ??= node;
        Count++;
    }

    /// <summary>Adds <paramref name="value"/> to the end of the list. O(1).</summary>
    public void AddLast(T value)
    {
        var node = new Node(value);
        if (_tail is null)
        {
            _head = _tail = node;
        }
        else
        {
            _tail.Next = node;
            _tail = node;
        }
        Count++;
    }

    /// <summary>
    /// Removes the first element equal to <paramref name="value"/>.
    /// Returns <see langword="true"/> if an element was removed. O(n).
    /// </summary>
    public bool Remove(T value)
    {
        Node? previous = null;
        for (var current = _head; current is not null; current = current.Next)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, value))
            {
                if (previous is null)
                {
                    _head = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }
                if (ReferenceEquals(current, _tail))
                {
                    _tail = previous;
                }
                Count--;
                return true;
            }
            previous = current;
        }
        return false;
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

    /// <summary>Removes all elements. O(1) — the chain is left for the GC.</summary>
    public void Clear()
    {
        _head = _tail = null;
        Count = 0;
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
