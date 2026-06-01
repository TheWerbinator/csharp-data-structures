using DataStructures.Hashing;
using DataStructures.Sorting;
using DataStructures.Trees;
using FsCheck;
using FsCheck.Xunit;

namespace DataStructures.Tests;

/// <summary>
/// Property-based tests: instead of hand-picked cases, FsCheck generates
/// hundreds of random inputs and checks an invariant holds for all of them.
/// These catch edge cases (empty, single, duplicates, negatives, ordering)
/// that example-based tests miss.
/// </summary>
public class PropertyTests
{
    [Property]
    public bool MergeSort_MatchesFrameworkSort(int[] input)
    {
        var mine = input.ToList();
        Sorts.MergeSort(mine);
        var reference = input.OrderBy(x => x).ToList();
        return mine.SequenceEqual(reference);
    }

    [Property]
    public bool QuickSort_MatchesFrameworkSort(int[] input)
    {
        var mine = input.ToList();
        Sorts.QuickSort(mine);
        return mine.SequenceEqual(input.OrderBy(x => x));
    }

    [Property]
    public bool HeapSort_MatchesFrameworkSort(int[] input)
    {
        var mine = input.ToList();
        Sorts.HeapSort(mine);
        return mine.SequenceEqual(input.OrderBy(x => x));
    }

    [Property]
    public bool BstInOrderTraversal_IsAlwaysSorted(int[] input)
    {
        var tree = new BinarySearchTree<int>();
        foreach (var n in input)
        {
            tree.Insert(n);
        }
        var traversal = tree.ToList();
        // In-order traversal must equal the distinct, sorted input (BST is a set).
        var expected = input.Distinct().OrderBy(x => x).ToList();
        return traversal.SequenceEqual(expected);
    }

    [Property]
    public bool BstCount_EqualsDistinctInputCount(int[] input)
    {
        var tree = new BinarySearchTree<int>();
        foreach (var n in input)
        {
            tree.Insert(n);
        }
        return tree.Count == input.Distinct().Count();
    }

    [Property]
    public bool OpenAddressing_RoundTripsAnyKeyValueMap(NonNull<string>[] keys, int[] values)
    {
        // Pair up keys/values, dedup keys keeping the last value (map semantics).
        var n = Math.Min(keys.Length, values.Length);
        var map = new Dictionary<string, int>();
        for (var i = 0; i < n; i++)
        {
            map[keys[i].Get] = values[i];
        }

        var table = new OpenAddressingHashTable<string, int>();
        foreach (var (k, v) in map)
        {
            table.Put(k, v);
        }
        if (table.Count != map.Count)
        {
            return false;
        }
        return map.All(kv => table.TryGet(kv.Key, out var got) && got == kv.Value);
    }

    [Property]
    public bool Chaining_RoundTripsAnyIntMap(int[] keys, int[] values)
    {
        var n = Math.Min(keys.Length, values.Length);
        var map = new Dictionary<int, int>();
        for (var i = 0; i < n; i++)
        {
            map[keys[i]] = values[i];
        }

        var table = new ChainingHashTable<int, int>();
        foreach (var (k, v) in map)
        {
            table.Put(k, v);
        }
        if (table.Count != map.Count)
        {
            return false;
        }
        return map.All(kv => table.TryGet(kv.Key, out var got) && got == kv.Value);
    }
}
