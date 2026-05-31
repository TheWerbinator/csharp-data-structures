namespace DataStructures.Sorting;

/// <summary>
/// Comparison sorts implemented in place or via auxiliary buffers, each as a
/// static generic method over <see cref="IList{T}"/>. These exist to show the
/// mechanics; production code should call <see cref="List{T}.Sort()"/> or
/// <c>Array.Sort</c> (an introsort) which beats all of these in practice.
/// </summary>
public static class Sorts
{
    /// <summary>
    /// Merge sort. Stable, O(n log n) time, O(n) auxiliary space. Good when
    /// stability matters or data is in a linked structure.
    /// </summary>
    public static void MergeSort<T>(IList<T> list, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;
        if (list.Count < 2)
        {
            return;
        }
        var buffer = new T[list.Count];
        MergeSortRange(list, buffer, 0, list.Count - 1, comparer);
    }

    private static void MergeSortRange<T>(IList<T> list, T[] buffer, int low, int high, IComparer<T> comparer)
    {
        if (low >= high)
        {
            return;
        }
        var mid = low + ((high - low) >> 1);
        MergeSortRange(list, buffer, low, mid, comparer);
        MergeSortRange(list, buffer, mid + 1, high, comparer);
        Merge(list, buffer, low, mid, high, comparer);
    }

    private static void Merge<T>(IList<T> list, T[] buffer, int low, int mid, int high, IComparer<T> comparer)
    {
        for (var i = low; i <= high; i++)
        {
            buffer[i] = list[i];
        }
        int left = low, right = mid + 1;
        for (var i = low; i <= high; i++)
        {
            if (left > mid)
            {
                list[i] = buffer[right++];
            }
            else if (right > high)
            {
                list[i] = buffer[left++];
            }
            // <= keeps the sort stable: equal elements from the left half win.
            else if (comparer.Compare(buffer[left], buffer[right]) <= 0)
            {
                list[i] = buffer[left++];
            }
            else
            {
                list[i] = buffer[right++];
            }
        }
    }

    /// <summary>
    /// Quicksort with median-of-three pivot selection. Average O(n log n),
    /// worst case O(n²) (mitigated, not eliminated, by the pivot choice).
    /// In place, not stable.
    /// </summary>
    public static void QuickSort<T>(IList<T> list, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;
        QuickSortRange(list, 0, list.Count - 1, comparer);
    }

    private static void QuickSortRange<T>(IList<T> list, int low, int high, IComparer<T> comparer)
    {
        while (low < high)
        {
            var pivot = Partition(list, low, high, comparer);
            // Recurse into the smaller side, loop on the larger — bounds stack
            // depth to O(log n) even on adversarial input.
            if (pivot - low < high - pivot)
            {
                QuickSortRange(list, low, pivot - 1, comparer);
                low = pivot + 1;
            }
            else
            {
                QuickSortRange(list, pivot + 1, high, comparer);
                high = pivot - 1;
            }
        }
    }

    private static int Partition<T>(IList<T> list, int low, int high, IComparer<T> comparer)
    {
        var mid = low + ((high - low) >> 1);
        // Median-of-three: after these swaps list[low] <= list[mid] <= list[high].
        if (comparer.Compare(list[mid], list[low]) < 0) Swap(list, low, mid);
        if (comparer.Compare(list[high], list[low]) < 0) Swap(list, low, high);
        if (comparer.Compare(list[high], list[mid]) < 0) Swap(list, mid, high);
        // Use the median as pivot: move it to `high`, then Lomuto-partition.
        Swap(list, mid, high);
        var pivot = list[high];

        var store = low;
        for (var i = low; i < high; i++)
        {
            if (comparer.Compare(list[i], pivot) < 0)
            {
                Swap(list, i, store);
                store++;
            }
        }
        Swap(list, store, high);
        return store;
    }

    /// <summary>
    /// Heap sort. O(n log n) worst case, in place, not stable. Builds a binary
    /// max-heap in the array then repeatedly extracts the max to the end.
    /// </summary>
    public static void HeapSort<T>(IList<T> list, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;
        var n = list.Count;
        for (var i = n / 2 - 1; i >= 0; i--)
        {
            SiftDown(list, i, n, comparer);
        }
        for (var end = n - 1; end > 0; end--)
        {
            Swap(list, 0, end);
            SiftDown(list, 0, end, comparer);
        }
    }

    private static void SiftDown<T>(IList<T> list, int root, int size, IComparer<T> comparer)
    {
        while (true)
        {
            var largest = root;
            var left = 2 * root + 1;
            var right = 2 * root + 2;
            if (left < size && comparer.Compare(list[left], list[largest]) > 0)
            {
                largest = left;
            }
            if (right < size && comparer.Compare(list[right], list[largest]) > 0)
            {
                largest = right;
            }
            if (largest == root)
            {
                return;
            }
            Swap(list, root, largest);
            root = largest;
        }
    }

    private static void Swap<T>(IList<T> list, int i, int j) => (list[i], list[j]) = (list[j], list[i]);
}
