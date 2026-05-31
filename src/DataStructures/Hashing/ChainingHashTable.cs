using System.Collections;

namespace DataStructures.Hashing;

/// <summary>
/// A hash table that resolves collisions by separate chaining: each bucket
/// holds a linked list of entries that hash to it. Simple, tolerates load
/// factors above 1, and degrades gracefully — but every lookup that collides
/// chases pointers, hurting cache locality compared to open addressing.
/// </summary>
/// <typeparam name="TKey">Key type.</typeparam>
/// <typeparam name="TValue">Value type.</typeparam>
public sealed class ChainingHashTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private sealed class Entry(TKey key, TValue value)
    {
        public readonly TKey Key = key;
        public TValue Value = value;
        public Entry? Next;
    }

    private const double MaxLoadFactor = 0.75;

    private readonly IEqualityComparer<TKey> _comparer;
    private Entry?[] _buckets;

    /// <summary>Creates a table with an optional initial bucket count and comparer.</summary>
    public ChainingHashTable(int capacity = 16, IEqualityComparer<TKey>? comparer = null)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
        _buckets = new Entry?[capacity];
    }

    /// <summary>Number of key/value pairs. O(1).</summary>
    public int Count { get; private set; }

    private int BucketIndex(TKey key, int bucketCount)
    {
        // Mask off the sign bit so the hash is non-negative before modulo.
        return (_comparer.GetHashCode(key) & 0x7FFFFFFF) % bucketCount;
    }

    /// <summary>
    /// Inserts or updates the value for <paramref name="key"/>. Amortized O(1).
    /// </summary>
    public void Put(TKey key, TValue value)
    {
        var index = BucketIndex(key, _buckets.Length);
        for (var entry = _buckets[index]; entry is not null; entry = entry.Next)
        {
            if (_comparer.Equals(entry.Key, key))
            {
                entry.Value = value;
                return;
            }
        }
        _buckets[index] = new Entry(key, value) { Next = _buckets[index] };
        Count++;
        if ((double)Count / _buckets.Length > MaxLoadFactor)
        {
            Resize();
        }
    }

    /// <summary>
    /// Gets the value for <paramref name="key"/>. Returns <see langword="false"/>
    /// if absent. Amortized O(1).
    /// </summary>
    public bool TryGet(TKey key, out TValue value)
    {
        var index = BucketIndex(key, _buckets.Length);
        for (var entry = _buckets[index]; entry is not null; entry = entry.Next)
        {
            if (_comparer.Equals(entry.Key, key))
            {
                value = entry.Value;
                return true;
            }
        }
        value = default!;
        return false;
    }

    /// <summary>Removes <paramref name="key"/>. Returns whether it was present. Amortized O(1).</summary>
    public bool Remove(TKey key)
    {
        var index = BucketIndex(key, _buckets.Length);
        Entry? previous = null;
        for (var entry = _buckets[index]; entry is not null; entry = entry.Next)
        {
            if (_comparer.Equals(entry.Key, key))
            {
                if (previous is null)
                {
                    _buckets[index] = entry.Next;
                }
                else
                {
                    previous.Next = entry.Next;
                }
                Count--;
                return true;
            }
            previous = entry;
        }
        return false;
    }

    private void Resize()
    {
        var grown = new Entry?[_buckets.Length * 2];
        foreach (var head in _buckets)
        {
            for (var entry = head; entry is not null;)
            {
                var next = entry.Next;
                var index = BucketIndex(entry.Key, grown.Length);
                entry.Next = grown[index];
                grown[index] = entry;
                entry = next;
            }
        }
        _buckets = grown;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var head in _buckets)
        {
            for (var entry = head; entry is not null; entry = entry.Next)
            {
                yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
