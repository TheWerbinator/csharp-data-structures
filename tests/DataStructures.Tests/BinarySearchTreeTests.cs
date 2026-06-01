using DataStructures.Trees;
using Xunit;

namespace DataStructures.Tests;

public class BinarySearchTreeTests
{
    [Fact]
    public void Insert_RejectsDuplicates()
    {
        var tree = new BinarySearchTree<int>();
        Assert.True(tree.Insert(5));
        Assert.False(tree.Insert(5));
        Assert.Equal(1, tree.Count);
    }

    [Fact]
    public void InOrderTraversal_IsSorted()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var n in new[] { 5, 3, 8, 1, 4, 7, 9, 2, 6 }) tree.Insert(n);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9], tree);
    }

    [Fact]
    public void Contains_FindsPresentAndAbsent()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var n in new[] { 5, 3, 8 }) tree.Insert(n);
        Assert.True(tree.Contains(3));
        Assert.False(tree.Contains(4));
    }

    [Fact]
    public void Remove_LeafNode()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var n in new[] { 5, 3, 8 }) tree.Insert(n);
        Assert.True(tree.Remove(3));     // leaf
        Assert.Equal([5, 8], tree);
        Assert.Equal(2, tree.Count);
    }

    [Fact]
    public void Remove_NodeWithOneChild()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var n in new[] { 5, 3, 2 }) tree.Insert(n);
        Assert.True(tree.Remove(3));     // one child (2)
        Assert.Equal([2, 5], tree);
    }

    [Fact]
    public void Remove_NodeWithTwoChildren_UsesInOrderSuccessor()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var n in new[] { 5, 3, 8, 6, 9, 7 }) tree.Insert(n);
        Assert.True(tree.Remove(8));     // two children; successor is 9... no, 6's subtree
        Assert.Equal([3, 5, 6, 7, 9], tree);
        Assert.Equal(5, tree.Count);
    }

    [Fact]
    public void Remove_Root_Repeatedly_DrainsTree()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var n in new[] { 5, 3, 8, 1, 4 }) tree.Insert(n);
        Assert.True(tree.Remove(5));
        Assert.True(tree.Remove(3));
        Assert.True(tree.Remove(8));
        Assert.Equal([1, 4], tree);
    }

    [Fact]
    public void Remove_AbsentValue_ReturnsFalse()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);
        Assert.False(tree.Remove(99));
        Assert.Equal(1, tree.Count);
    }
}
