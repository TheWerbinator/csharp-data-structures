namespace DataStructures.Algorithms.DynamicProgramming;

/// <summary>
/// Three Fibonacci implementations side by side — the canonical example of
/// why memoization matters. <see cref="Recursive"/> is O(2^n) and chokes
/// past n ≈ 35; <see cref="Memoized"/> and <see cref="Iterative"/> are both
/// O(n) but the iterative version avoids the recursion overhead and the
/// dictionary allocation.
/// </summary>
public static class Fibonacci
{
    /// <summary>
    /// Naive double-recursion. Included for comparison; do not call with
    /// n &gt; 35 unless you want to wait.
    /// </summary>
    public static long Recursive(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n must be non-negative");
        }
        if (n <= 1)
        {
            return n;
        }
        return Recursive(n - 1) + Recursive(n - 2);
    }

    /// <summary>
    /// Top-down DP. Caches subproblem results on the way up. O(n) time,
    /// O(n) space, O(n) stack depth.
    /// </summary>
    public static long Memoized(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n must be non-negative");
        }
        var cache = new long[n + 1];
        Array.Fill(cache, -1L);
        return MemoFib(n, cache);
    }

    private static long MemoFib(int n, long[] cache)
    {
        if (n <= 1)
        {
            return n;
        }
        if (cache[n] >= 0)
        {
            return cache[n];
        }
        return cache[n] = MemoFib(n - 1, cache) + MemoFib(n - 2, cache);
    }

    /// <summary>
    /// Bottom-up DP with two-variable rolling state. O(n) time, O(1) space.
    /// The version you'd actually ship.
    /// </summary>
    public static long Iterative(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n must be non-negative");
        }
        if (n <= 1)
        {
            return n;
        }
        long prev = 0, curr = 1;
        for (var i = 2; i <= n; i++)
        {
            (prev, curr) = (curr, prev + curr);
        }
        return curr;
    }
}
