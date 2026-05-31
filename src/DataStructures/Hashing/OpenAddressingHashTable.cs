using System.Collections;

namespace DataStructures.Hashing;

/// <summary>
/// A hash table that resolves collisions by open addressing with linear
/// probing: on collision it scans forward to the next free slot. All entries
/// live in one contiguous array, so lookups have excellent cache locality —
/// the reason <see cref="System.Collections.Generic.Dictionary{TKey,TValue}"/>
/// and most modern hash maps prefer open addressing over chaining.
///
/// Deletion uses tombstones (a removed marker) so probe sequences for other
/// keys aren't broken. Tombstones are reclaimed on resize.
/// </summary>
/// <typeparam name="TKey">Key type.</typeparam>
/// <typeparam name="TValue">Value type.</typeparam>
public sealed class OpenAddressingHashTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private enum SlotState : byte { Empty, Occupied, Tombstone }

    private readonly IEqualityComparer<TKey> _comparer;
    private TKey[] _keys;
    private TValue[] _values;
    private SlotState[] _states;
    private int _occupied;   // live entries
    private int _used;       // occupied + tombstones

    private const double MaxLoadFactor = 0.7;

    /// <summary>Creates a table with an optional initial capacity and comparer.</summary>
    public OpenAddressingHashTable(int capacity = 16, IEqualityComparer<TKey>? comparer = null)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
        capacity = NextPowerOfTwo(capacity);
        _keys = new TKey[capacity];
        _values = new TValue[capacity];
        _states = new SlotState[capacity];
    }

    /// <summary>Number of live key/value pairs. O(1).</summary>
    public int Count => _occupied;

    private int Mask => _keys.Length - 1;

    private int HashIndex(TKey key) => _comparer.GetHashCode(key) & 0x7FFFFFFF & Mask;

    /// <summary>Inserts or updates the value for <paramref name="key"/>. Amortized O(1).</summary>
    public void Put(TKey key, TValue value)
    {
        // Resize on the *used* slots (incl. tombstones) to bound probe length.
        if ((double)(_used + 1) / _keys.Length > MaxLoadFactor)
        {
            Resize();
        }

        var index = HashIndex(key);
        var firstTombstone = -1;
        while (_states[index] != SlotState.Empty)
        {
            if (_states[index] == SlotState.Occupied && _comparer.Equals(_keys[index], key))
            {
                _values[index] = value;  // update existing
                return;
            }
            if (_states[index] == SlotState.Tombstone && firstTombstone < 0)
            {
                firstTombstone = index;
            }
            index = (index + 1) & Mask;
        }

        // Reuse the earliest tombstone in the probe sequence if there was one.
        var target = firstTombstone >= 0 ? firstTombstone : index;
        if (_states[target] != SlotState.Tombstone)
        {
            _used++;  // consuming a fresh Empty slot
        }
        _keys[target] = key;
        _values[target] = value;
        _states[target] = SlotState.Occupied;
        _occupied++;
    }

    /// <summary>Gets the value for <paramref name="key"/>, or <see langword="false"/> if absent. Amortized O(1).</summary>
    public bool TryGet(TKey key, out TValue value)
    {
        var index = HashIndex(key);
        while (_states[index] != SlotState.Empty)
        {
            if (_states[index] == SlotState.Occupied && _comparer.Equals(_keys[index], key))
            {
                value = _values[index];
                return true;
            }
            index = (index + 1) & Mask;
        }
        value = default!;
        return false;
    }

    /// <summary>Removes <paramref name="key"/> via tombstone. Returns whether it was present. Amortized O(1).</summary>
    public bool Remove(TKey key)
    {
        var index = HashIndex(key);
        while (_states[index] != SlotState.Empty)
        {
            if (_states[index] == SlotState.Occupied && _comparer.Equals(_keys[index], key))
            {
                _states[index] = SlotState.Tombstone;
                _keys[index] = default!;
                _values[index] = default!;
                _occupied--;
                return true;
            }
            index = (index + 1) & Mask;
        }
        return false;
    }

    private void Resize()
    {
        var oldKeys = _keys;
        var oldValues = _values;
        var oldStates = _states;

        var newCapacity = _keys.Length * 2;
        _keys = new TKey[newCapacity];
        _values = new TValue[newCapacity];
        _states = new SlotState[newCapacity];
        _occupied = 0;
        _used = 0;

        for (var i = 0; i < oldStates.Length; i++)
        {
            if (oldStates[i] == SlotState.Occupied)
            {
                Put(oldKeys[i], oldValues[i]);  // rehash; tombstones dropped
            }
        }
    }

    private static int NextPowerOfTwo(int n)
    {
        var power = 1;
        while (power < n)
        {
            power <<= 1;
        }
        return power;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (var i = 0; i < _states.Length; i++)
        {
            if (_states[i] == SlotState.Occupied)
            {
                yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
