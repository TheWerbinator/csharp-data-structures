using DataStructures.Hashing;
using Xunit;

namespace DataStructures.Tests;

public class ChainingHashTableTests
{
    [Fact]
    public void Put_Get_RoundTrips()
    {
        var table = new ChainingHashTable<string, int>();
        table.Put("one", 1);
        table.Put("two", 2);
        Assert.True(table.TryGet("one", out var v));
        Assert.Equal(1, v);
        Assert.False(table.TryGet("missing", out _));
    }

    [Fact]
    public void Put_SameKey_Updates()
    {
        var table = new ChainingHashTable<string, int>();
        table.Put("k", 1);
        table.Put("k", 2);
        Assert.Equal(1, table.Count);
        Assert.True(table.TryGet("k", out var v));
        Assert.Equal(2, v);
    }

    [Fact]
    public void Remove_Works()
    {
        var table = new ChainingHashTable<string, int>();
        table.Put("k", 1);
        Assert.True(table.Remove("k"));
        Assert.False(table.Remove("k"));
        Assert.Equal(0, table.Count);
    }

    [Fact]
    public void GrowsUnderLoad_AllKeysSurvive()
    {
        var table = new ChainingHashTable<int, int>(capacity: 4);
        for (var i = 0; i < 1000; i++) table.Put(i, i * 10);
        Assert.Equal(1000, table.Count);
        for (var i = 0; i < 1000; i++)
        {
            Assert.True(table.TryGet(i, out var v));
            Assert.Equal(i * 10, v);
        }
    }
}

public class OpenAddressingHashTableTests
{
    [Fact]
    public void Put_Get_RoundTrips()
    {
        var table = new OpenAddressingHashTable<string, int>();
        table.Put("one", 1);
        table.Put("two", 2);
        Assert.True(table.TryGet("two", out var v));
        Assert.Equal(2, v);
        Assert.False(table.TryGet("missing", out _));
    }

    [Fact]
    public void Remove_Tombstone_DoesNotBreakProbeSequence()
    {
        // Force a collision chain, remove the middle, ensure the tail is still
        // findable through the tombstone.
        var table = new OpenAddressingHashTable<int, int>(capacity: 8);
        for (var i = 0; i < 6; i++) table.Put(i, i);
        Assert.True(table.Remove(2));
        for (var i = 0; i < 6; i++)
        {
            if (i == 2)
            {
                Assert.False(table.TryGet(i, out _));
            }
            else
            {
                Assert.True(table.TryGet(i, out var v));
                Assert.Equal(i, v);
            }
        }
    }

    [Fact]
    public void Reinsert_AfterRemove_ReusesTombstone()
    {
        var table = new OpenAddressingHashTable<int, int>(capacity: 8);
        table.Put(1, 10);
        table.Remove(1);
        table.Put(1, 20);
        Assert.Equal(1, table.Count);
        Assert.True(table.TryGet(1, out var v));
        Assert.Equal(20, v);
    }

    [Fact]
    public void GrowsUnderLoad_AllKeysSurvive()
    {
        var table = new OpenAddressingHashTable<int, string>(capacity: 4);
        for (var i = 0; i < 1000; i++) table.Put(i, $"v{i}");
        Assert.Equal(1000, table.Count);
        for (var i = 0; i < 1000; i++)
        {
            Assert.True(table.TryGet(i, out var v));
            Assert.Equal($"v{i}", v);
        }
    }
}
