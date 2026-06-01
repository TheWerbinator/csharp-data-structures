using DataStructures.Searching;
using DataStructures.Sorting;
using Xunit;

namespace DataStructures.Tests;

public class BinarySearchTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(5, 2)]
    [InlineData(9, 4)]
    [InlineData(4, -1)]
    [InlineData(10, -1)]
    public void Iterative_FindsIndexOrMinusOne(int target, int expected)
    {
        int[] sorted = [1, 3, 5, 7, 9];
        Assert.Equal(expected, BinarySearch.Iterative(sorted, target));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(5, 2)]
    [InlineData(9, 4)]
    [InlineData(4, -1)]
    public void Recursive_MatchesIterative(int target, int expected)
    {
        int[] sorted = [1, 3, 5, 7, 9];
        Assert.Equal(expected, BinarySearch.Recursive(sorted, target));
    }

    [Fact]
    public void EmptyList_ReturnsMinusOne()
    {
        Assert.Equal(-1, BinarySearch.Iterative(Array.Empty<int>(), 1));
        Assert.Equal(-1, BinarySearch.Recursive(Array.Empty<int>(), 1));
    }
}

public class SortingTests
{
    private static readonly int[] Unsorted = [5, 2, 9, 1, 5, 6, 3, 8, 7, 4, 0];
    private static readonly int[] Expected = [0, 1, 2, 3, 4, 5, 5, 6, 7, 8, 9];

    [Fact]
    public void MergeSort_Sorts()
    {
        var list = Unsorted.ToList();
        Sorts.MergeSort(list);
        Assert.Equal(Expected, list);
    }

    [Fact]
    public void QuickSort_Sorts()
    {
        var list = Unsorted.ToList();
        Sorts.QuickSort(list);
        Assert.Equal(Expected, list);
    }

    [Fact]
    public void HeapSort_Sorts()
    {
        var list = Unsorted.ToList();
        Sorts.HeapSort(list);
        Assert.Equal(Expected, list);
    }

    [Fact]
    public void QuickSort_HandlesAlreadySorted_NoStackOverflow()
    {
        // Adversarial-ish: already sorted, 10k elements. Median-of-three +
        // recurse-smaller-side should keep stack depth O(log n).
        var list = Enumerable.Range(0, 10_000).ToList();
        Sorts.QuickSort(list);
        Assert.Equal(Enumerable.Range(0, 10_000), list);
    }

    [Fact]
    public void MergeSort_IsStable()
    {
        // Sort pairs by Key only; equal keys must retain insertion order.
        var data = new List<(int Key, int Seq)>
        {
            (1, 0), (2, 1), (1, 2), (2, 3), (1, 4),
        };
        Sorts.MergeSort(data, Comparer<(int Key, int Seq)>.Create((a, b) => a.Key.CompareTo(b.Key)));
        Assert.Equal([(1, 0), (1, 2), (1, 4), (2, 1), (2, 3)], data);
    }

    [Fact]
    public void EmptyAndSingle_AreNoOps()
    {
        var empty = new List<int>();
        Sorts.QuickSort(empty);
        Assert.Empty(empty);

        var single = new List<int> { 42 };
        Sorts.HeapSort(single);
        Assert.Equal([42], single);
    }
}
