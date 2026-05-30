using System.Collections;

namespace DataStructures.LinkedList;

/// <summary>
/// A linked list that keeps its elements in ascending order on insertion.
/// Insertion is O(n) (it walks to the correct position), but enumeration is
/// always sorted with no separate sort step. Order is defined by an
/// <see cref="IComparer{T}"/> supplied at construction, defaulting to
/// <see cref="Comparer{T}.Default"/>.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class SortedLinkedList<T> : IEnumerable<T>
{
    private sealed class Node(T value)
    {
        public T Value = value;
        public Node? Next;
    }

    private readonly IComparer<T> _comparer;
    private Node? _head;

    /// <summary>Creates a list ordered by <see cref="Comparer{T}.Default"/>.</summary>
    public SortedLinkedList() : this(Comparer<T>.Default) { }

    /// <summary>Creates a list ordered by <paramref name="comparer"/>.</summary>
    public SortedLinkedList(IComparer<T> comparer) => _comparer = comparer;

    /// <summary>Number of elements in the list. O(1).</summary>
    public int Count { get; private set; }

    /// <summary>
    /// Inserts <paramref name="value"/> at the position that keeps the list
    /// sorted. Stable with respect to equal keys (new equal elements go after
    /// existing ones). O(n).
    /// </summary>
    public void Add(T value)
    {
        var node = new Node(value);
        if (_head is null || _comparer.Compare(value, _head.Value) < 0)
        {
            node.Next = _head;
            _head = node;
        }
        else
        {
            var current = _head;
            while (current.Next is not null && _comparer.Compare(current.Next.Value, value) <= 0)
            {
                current = current.Next;
            }
            node.Next = current.Next;
            current.Next = node;
        }
        Count++;
    }

    /// <summary>Removes the first element equal to <paramref name="value"/>. O(n).</summary>
    public bool Remove(T value)
    {
        Node? previous = null;
        for (var current = _head; current is not null; current = current.Next)
        {
            var cmp = _comparer.Compare(current.Value, value);
            if (cmp == 0)
            {
                if (previous is null)
                {
                    _head = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }
                Count--;
                return true;
            }
            // List is sorted: once we pass the target key, it can't appear later.
            if (cmp > 0)
            {
                return false;
            }
            previous = current;
        }
        return false;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the list contains <paramref name="value"/>.
    /// Stops early once it passes the target key. O(n) worst case.
    /// </summary>
    public bool Contains(T value)
    {
        for (var current = _head; current is not null; current = current.Next)
        {
            var cmp = _comparer.Compare(current.Value, value);
            if (cmp == 0)
            {
                return true;
            }
            if (cmp > 0)
            {
                return false;
            }
        }
        return false;
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
