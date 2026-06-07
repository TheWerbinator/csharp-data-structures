namespace DataStructures.Algorithms.Greedy;

/// <summary>
/// Bélády's optimal page-replacement (a.k.a. furthest-in-future, OPT, MIN).
/// On a cache miss with a full cache, evict the element whose next use lies
/// furthest in the future — or that never comes again. Provably minimizes
/// the total miss count for a known request sequence.
/// <para>
/// This is offline — it requires the full request sequence up front, which
/// real systems don't have. It's the theoretical lower bound that online
/// policies (LRU, CLOCK, ARC) are measured against.
/// </para>
/// </summary>
public static class FurthestInFutureCache<T>
    where T : notnull
{
    /// <summary>
    /// Simulate Bélády's policy on a request stream and return the count of
    /// cache misses. O(n · k) time where n is requests, k is cache size.
    /// </summary>
    public static int CountMisses(IReadOnlyList<T> requests, int cacheSize)
    {
        ArgumentNullException.ThrowIfNull(requests);
        if (cacheSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(cacheSize), "Cache size must be at least 1");
        }
        var cache = new HashSet<T>();
        var misses = 0;
        for (var i = 0; i < requests.Count; i++)
        {
            var req = requests[i];
            if (cache.Contains(req))
            {
                continue;
            }
            misses++;
            if (cache.Count < cacheSize)
            {
                cache.Add(req);
                continue;
            }
            EvictFurthest(cache, requests, i);
            cache.Add(req);
        }
        return misses;
    }

    private static void EvictFurthest(HashSet<T> cache, IReadOnlyList<T> requests, int currentIndex)
    {
        T? victim = default;
        var furthest = -1;
        var hasVictim = false;
        foreach (var item in cache)
        {
            var nextUse = NextUseIndex(requests, item, currentIndex + 1);
            // Never used again → evict this immediately; nothing beats infinity.
            if (nextUse == -1)
            {
                cache.Remove(item);
                return;
            }
            if (nextUse > furthest)
            {
                furthest = nextUse;
                victim = item;
                hasVictim = true;
            }
        }
        if (hasVictim)
        {
            cache.Remove(victim!);
        }
    }

    private static int NextUseIndex(IReadOnlyList<T> requests, T item, int from)
    {
        for (var i = from; i < requests.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(requests[i], item))
            {
                return i;
            }
        }
        return -1;
    }
}
