namespace DataStructures.Searching;

/// <summary>
/// Binary search over a sorted <see cref="IReadOnlyList{T}"/>. Both an
/// iterative and a recursive implementation are provided — they return the
/// same result; the iterative one avoids the call-stack growth of the
/// recursive one (though at log2(n) depth that's never a real concern).
/// </summary>
public static class BinarySearch
{
    /// <summary>
    /// Returns the index of <paramref name="target"/> in the sorted list, or
    /// -1 if absent. Uses <paramref name="comparer"/> or
    /// <see cref="Comparer{T}.Default"/>. O(log n).
    /// </summary>
    /// <remarks>
    /// The midpoint is computed as <c>low + (high - low) / 2</c> rather than
    /// <c>(low + high) / 2</c> to avoid integer overflow on very large lists —
    /// the classic bug Jon Bentley documented in nearly every textbook
    /// implementation.
    /// </remarks>
    public static int Iterative<T>(IReadOnlyList<T> sorted, T target, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;
        var low = 0;
        var high = sorted.Count - 1;
        while (low <= high)
        {
            var mid = low + ((high - low) >> 1);
            var cmp = comparer.Compare(sorted[mid], target);
            if (cmp == 0)
            {
                return mid;
            }
            if (cmp < 0)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }
        return -1;
    }

    /// <summary>
    /// Recursive binary search. Same contract as <see cref="Iterative{T}"/>.
    /// O(log n) time, O(log n) stack depth.
    /// </summary>
    public static int Recursive<T>(IReadOnlyList<T> sorted, T target, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;
        return Search(sorted, target, 0, sorted.Count - 1, comparer);
    }

    private static int Search<T>(IReadOnlyList<T> sorted, T target, int low, int high, IComparer<T> comparer)
    {
        if (low > high)
        {
            return -1;
        }
        var mid = low + ((high - low) >> 1);
        var cmp = comparer.Compare(sorted[mid], target);
        return cmp switch
        {
            0 => mid,
            < 0 => Search(sorted, target, mid + 1, high, comparer),
            > 0 => Search(sorted, target, low, mid - 1, comparer),
        };
    }
}
