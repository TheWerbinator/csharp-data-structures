using System.Collections;

namespace DataStructures.Trees;

/// <summary>
/// An unbalanced binary search tree. Insert, search, and delete are O(h)
/// where h is the height — O(log n) on random input, O(n) on already-sorted
/// input (the tree degenerates into a linked list). For guaranteed log-time
/// operations use a self-balancing tree; this one is here to show the core
/// BST mechanics, including the three-case delete.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class BinarySearchTree<T> : IEnumerable<T>
{
    private sealed class Node(T value)
    {
        public T Value = value;
        public Node? Left;
        public Node? Right;
    }

    private readonly IComparer<T> _comparer;
    private Node? _root;

    /// <summary>Creates a tree ordered by <see cref="Comparer{T}.Default"/>.</summary>
    public BinarySearchTree() : this(Comparer<T>.Default) { }

    /// <summary>Creates a tree ordered by <paramref name="comparer"/>.</summary>
    public BinarySearchTree(IComparer<T> comparer) => _comparer = comparer;

    /// <summary>Number of nodes. O(1).</summary>
    public int Count { get; private set; }

    /// <summary>
    /// Inserts <paramref name="value"/>. Duplicate keys are ignored (set
    /// semantics) and reported by the return value. O(h).
    /// </summary>
    public bool Insert(T value)
    {
        if (_root is null)
        {
            _root = new Node(value);
            Count++;
            return true;
        }

        var current = _root;
        while (true)
        {
            var cmp = _comparer.Compare(value, current.Value);
            if (cmp == 0)
            {
                return false;  // duplicate
            }
            if (cmp < 0)
            {
                if (current.Left is null)
                {
                    current.Left = new Node(value);
                    Count++;
                    return true;
                }
                current = current.Left;
            }
            else
            {
                if (current.Right is null)
                {
                    current.Right = new Node(value);
                    Count++;
                    return true;
                }
                current = current.Right;
            }
        }
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="value"/> is present. O(h).</summary>
    public bool Contains(T value)
    {
        var current = _root;
        while (current is not null)
        {
            var cmp = _comparer.Compare(value, current.Value);
            if (cmp == 0)
            {
                return true;
            }
            current = cmp < 0 ? current.Left : current.Right;
        }
        return false;
    }

    /// <summary>
    /// Removes <paramref name="value"/> if present. Handles the three delete
    /// cases: leaf (detach), one child (splice), two children (replace with
    /// in-order successor). O(h).
    /// </summary>
    public bool Remove(T value)
    {
        var countBefore = Count;
        _root = RemoveNode(_root, value);
        return Count != countBefore;
    }

    private Node? RemoveNode(Node? node, T value)
    {
        if (node is null)
        {
            return null;
        }
        var cmp = _comparer.Compare(value, node.Value);
        if (cmp < 0)
        {
            node.Left = RemoveNode(node.Left, value);
        }
        else if (cmp > 0)
        {
            node.Right = RemoveNode(node.Right, value);
        }
        else
        {
            // Case 1 & 2: zero or one child — decrement here and splice out.
            if (node.Left is null)
            {
                Count--;
                return node.Right;
            }
            if (node.Right is null)
            {
                Count--;
                return node.Left;
            }
            // Case 3: two children. Copy the in-order successor's value up,
            // then delete the successor node from the right subtree — that
            // recursive call owns the Count decrement, so we don't touch it here.
            var successor = Min(node.Right);
            node.Value = successor;
            node.Right = RemoveNode(node.Right, successor);
        }
        return node;
    }

    private static T Min(Node node)
    {
        while (node.Left is not null)
        {
            node = node.Left;
        }
        return node.Value;
    }

    /// <summary>Enumerates elements in ascending (in-order) order. O(n).</summary>
    public IEnumerator<T> GetEnumerator()
    {
        // Explicit stack instead of recursion so the iterator stays lazy and
        // avoids building an intermediate list.
        var stack = new Stack<Node>();
        var current = _root;
        while (current is not null || stack.Count > 0)
        {
            while (current is not null)
            {
                stack.Push(current);
                current = current.Left;
            }
            current = stack.Pop();
            yield return current.Value;
            current = current.Right;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
