using DataStructures.LinkedList;
using Xunit;

namespace DataStructures.Tests;

public class SinglyLinkedListTests
{
    [Fact]
    public void AddFirst_PrependsInReverseOrder()
    {
        var list = new SinglyLinkedList<int>();
        list.AddFirst(1);
        list.AddFirst(2);
        list.AddFirst(3);
        Assert.Equal([3, 2, 1], list);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void AddLast_AppendsInOrder()
    {
        var list = new SinglyLinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void Remove_Head_Middle_Tail()
    {
        var list = new SinglyLinkedList<int>();
        foreach (var n in new[] { 1, 2, 3, 4 }) list.AddLast(n);

        Assert.True(list.Remove(1));   // head
        Assert.True(list.Remove(3));   // middle
        Assert.True(list.Remove(4));   // tail
        Assert.Equal([2], list);
        Assert.False(list.Remove(99)); // absent
    }

    [Fact]
    public void Remove_Tail_ThenAddLast_StillWorks()
    {
        var list = new SinglyLinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        Assert.True(list.Remove(2));   // removes tail; tail pointer must update
        list.AddLast(3);
        Assert.Equal([1, 3], list);
    }

    [Fact]
    public void Contains_And_Clear()
    {
        var list = new SinglyLinkedList<string>();
        list.AddLast("a");
        Assert.True(list.Contains("a"));
        Assert.False(list.Contains("b"));
        list.Clear();
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }
}

public class DoublyLinkedListTests
{
    [Fact]
    public void RemoveFirst_And_RemoveLast()
    {
        var list = new DoublyLinkedList<int>();
        foreach (var n in new[] { 1, 2, 3 }) list.AddLast(n);
        Assert.Equal(1, list.RemoveFirst());
        Assert.Equal(3, list.RemoveLast());
        Assert.Equal([2], list);
    }

    [Fact]
    public void EnumerateReverse_YieldsTailToHead()
    {
        var list = new DoublyLinkedList<int>();
        foreach (var n in new[] { 1, 2, 3 }) list.AddLast(n);
        Assert.Equal([3, 2, 1], list.EnumerateReverse());
    }

    [Fact]
    public void RemoveFirst_OnEmpty_Throws()
    {
        var list = new DoublyLinkedList<int>();
        Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
    }

    [Fact]
    public void AddFirst_AddLast_Interleaved()
    {
        var list = new DoublyLinkedList<int>();
        list.AddLast(2);
        list.AddFirst(1);
        list.AddLast(3);
        Assert.Equal([1, 2, 3], list);
    }
}

public class SortedLinkedListTests
{
    [Fact]
    public void Add_KeepsAscendingOrder()
    {
        var list = new SortedLinkedList<int>();
        foreach (var n in new[] { 5, 1, 4, 2, 3 }) list.Add(n);
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void Add_AllowsDuplicates_StableAfterExisting()
    {
        var list = new SortedLinkedList<int>();
        foreach (var n in new[] { 2, 1, 2, 1 }) list.Add(n);
        Assert.Equal([1, 1, 2, 2], list);
        Assert.Equal(4, list.Count);
    }

    [Fact]
    public void Remove_And_Contains_ShortCircuit()
    {
        var list = new SortedLinkedList<int>();
        foreach (var n in new[] { 1, 3, 5 }) list.Add(n);
        Assert.False(list.Contains(4));   // between 3 and 5, stops early
        Assert.True(list.Remove(3));
        Assert.Equal([1, 5], list);
    }

    [Fact]
    public void CustomComparer_DescendingOrder()
    {
        var list = new SortedLinkedList<int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        foreach (var n in new[] { 1, 3, 2 }) list.Add(n);
        // Use new[] not [..] — a collection expression here target-types to
        // SortedLinkedList<int> via its Add method (default ascending
        // comparer), masking the descending-comparer behavior we want to test.
        Assert.Equal(new[] { 3, 2, 1 }, list);
    }
}
