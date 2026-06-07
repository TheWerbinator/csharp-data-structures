using DataStructures.Algorithms.DivideConquer;
using DataStructures.Algorithms.DynamicProgramming;
using DataStructures.Algorithms.Graphs;
using DataStructures.Algorithms.Greedy;
using DataStructures.Algorithms.Selection;
using Xunit;

namespace DataStructures.Tests;

public class FibonacciTests
{
    [Theory]
    [InlineData(0, 0L)]
    [InlineData(1, 1L)]
    [InlineData(2, 1L)]
    [InlineData(10, 55L)]
    [InlineData(20, 6765L)]
    public void AllThree_AgreeOnSmallInputs(int n, long expected)
    {
        Assert.Equal(expected, Fibonacci.Recursive(n));
        Assert.Equal(expected, Fibonacci.Memoized(n));
        Assert.Equal(expected, Fibonacci.Iterative(n));
    }

    [Fact]
    public void Memoized_HandlesLargeN_WithoutBlowingUp()
    {
        // Fib(70) = 190392490709135 — naive recursive would take hours here.
        Assert.Equal(190_392_490_709_135L, Fibonacci.Memoized(70));
        Assert.Equal(190_392_490_709_135L, Fibonacci.Iterative(70));
    }

    [Fact]
    public void Negative_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Fibonacci.Iterative(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Fibonacci.Memoized(-1));
    }
}

public class LongestPalindromicSubsequenceTests
{
    [Theory]
    [InlineData("", 0)]
    [InlineData("a", 1)]
    [InlineData("aa", 2)]
    [InlineData("ab", 1)]
    [InlineData("character", 5)]   // "carac"
    [InlineData("bbbab", 4)]       // "bbbb"
    [InlineData("agbdba", 5)]      // "abdba"
    public void Length_MatchesKnown(string input, int expected)
    {
        Assert.Equal(expected, LongestPalindromicSubsequence.Length(input));
    }

    [Theory]
    [InlineData("racecar")]
    [InlineData("character")]
    [InlineData("bbbab")]
    [InlineData("forgeeksskeegfor")]
    public void Witness_IsActuallyAPalindromeAndAValidSubsequence(string input)
    {
        var witness = LongestPalindromicSubsequence.Witness(input);
        Assert.Equal(LongestPalindromicSubsequence.Length(input), witness.Length);
        Assert.Equal(witness, new string(witness.Reverse().ToArray()));
        // Subsequence check: chars of witness appear in order in input.
        var j = 0;
        foreach (var c in input)
        {
            if (j < witness.Length && witness[j] == c)
            {
                j++;
            }
        }
        Assert.Equal(witness.Length, j);
    }
}

public class MaxSubarrayTests
{
    [Fact]
    public void Classic_CLRSExample()
    {
        // CLRS p.70: max-subarray of stock prices is +43 over indices 7..10.
        int[] input = [13, -3, -25, 20, -3, -16, -23, 18, 20, -7, 12, -5, -22, 15, -4, 7];
        var dc = MaxSubarray.DivideAndConquer(input);
        var k = MaxSubarray.Kadane(input);
        Assert.Equal(43, dc.Sum);
        Assert.Equal(43, k.Sum);
        Assert.Equal((7, 10), (dc.Low, dc.High));
        Assert.Equal((7, 10), (k.Low, k.High));
    }

    [Fact]
    public void AllNegative_PicksSingleLargestElement()
    {
        int[] input = [-5, -2, -8, -1, -4];
        var r = MaxSubarray.Kadane(input);
        Assert.Equal(-1, r.Sum);
        Assert.Equal(3, r.Low);
        Assert.Equal(3, r.High);
    }

    [Fact]
    public void SingleElement_IsTheAnswer()
    {
        int[] input = [42];
        Assert.Equal(42, MaxSubarray.Kadane(input).Sum);
        Assert.Equal(42, MaxSubarray.DivideAndConquer(input).Sum);
    }

    [Fact]
    public void Empty_Throws()
    {
        Assert.Throws<ArgumentException>(() => MaxSubarray.Kadane(Array.Empty<int>()));
        Assert.Throws<ArgumentException>(() => MaxSubarray.DivideAndConquer(Array.Empty<int>()));
    }
}

public class BreadthFirstSearchTests
{
    [Fact]
    public void DistancesAreCorrectOnSimpleGraph()
    {
        // 0 -- 1 -- 2
        //      |
        //      3
        var g = new Dictionary<int, IReadOnlyList<int>>
        {
            [0] = [1],
            [1] = [0, 2, 3],
            [2] = [1],
            [3] = [1],
        };
        var r = BreadthFirstSearch<int>.Run(g, 0);
        Assert.Equal(0, r.Distance[0]);
        Assert.Equal(1, r.Distance[1]);
        Assert.Equal(2, r.Distance[2]);
        Assert.Equal(2, r.Distance[3]);
    }

    [Fact]
    public void PathReconstruction_GivesShortestPath()
    {
        var g = new Dictionary<int, IReadOnlyList<int>>
        {
            [0] = [1, 2],
            [1] = [0, 3],
            [2] = [0, 3],
            [3] = [1, 2, 4],
            [4] = [3],
        };
        var r = BreadthFirstSearch<int>.Run(g, 0);
        var path = BreadthFirstSearch<int>.PathTo(r, 4);
        // 0 → (1 or 2) → 3 → 4 is the shortest route — 3 edges, 4 vertices.
        Assert.Equal(4, path.Count);
        Assert.Equal(0, path[0]);
        Assert.Equal(4, path[^1]);
        Assert.Equal(3, r.Distance[4]);
    }

    [Fact]
    public void UnreachableVertex_HasNoDistanceAndEmptyPath()
    {
        var g = new Dictionary<int, IReadOnlyList<int>>
        {
            [0] = [1],
            [1] = [0],
            [2] = [],
        };
        var r = BreadthFirstSearch<int>.Run(g, 0);
        Assert.False(r.Distance.ContainsKey(2));
        Assert.Empty(BreadthFirstSearch<int>.PathTo(r, 2));
    }
}

public class FurthestInFutureCacheTests
{
    [Fact]
    public void ClassicBeladyExample_GivesOptimalMissCount()
    {
        // <a, b, c, d, a, b, e, a, b, c, d, e> with cache size 3.
        // Bélády gives 7 misses (each new symbol's first appearance + the
        // forced re-fetches when the working set exceeds cache size).
        string[] requests = ["a", "b", "c", "d", "a", "b", "e", "a", "b", "c", "d", "e"];
        Assert.Equal(7, FurthestInFutureCache<string>.CountMisses(requests, 3));
    }

    [Fact]
    public void AllInCache_ZeroMissesAfterWarmup()
    {
        int[] requests = [1, 2, 3, 1, 2, 3, 1, 2, 3];
        // 3 warmup misses, then everything hits.
        Assert.Equal(3, FurthestInFutureCache<int>.CountMisses(requests, 3));
    }

    [Fact]
    public void CacheSizeOne_DegeneratesToMissOnEveryChange()
    {
        int[] requests = [1, 2, 1, 2, 1];
        Assert.Equal(5, FurthestInFutureCache<int>.CountMisses(requests, 1));
    }
}

public class MinMaxTests
{
    [Fact]
    public void OddCount()
    {
        int[] values = [3, 1, 4, 1, 5, 9, 2, 6, 5];
        Assert.Equal((1, 9), MinMax.Find(values));
    }

    [Fact]
    public void EvenCount()
    {
        int[] values = [3, 1, 4, 1, 5, 9, 2, 6];
        Assert.Equal((1, 9), MinMax.Find(values));
    }

    [Fact]
    public void SingleElement_IsBothMinAndMax()
    {
        Assert.Equal((42, 42), MinMax.Find(new[] { 42 }));
    }

    [Fact]
    public void AllEqual_IsBoth()
    {
        Assert.Equal((7, 7), MinMax.Find(new[] { 7, 7, 7, 7 }));
    }

    [Fact]
    public void MatchesFrameworkResults_OnRandomInput()
    {
        // Differential check against Enumerable.Min/Max for a few sizes.
        // FsCheck would be the natural fit but a few hand seeds is plenty here.
        int[][] cases =
        [
            [5, 2, 9, 1, 5, 6, 3, 8, 7, 4, 0],
            [-3, -1, -4, -1, -5, -9, -2, -6],
            [int.MaxValue, int.MinValue, 0],
        ];
        foreach (var arr in cases)
        {
            var (min, max) = MinMax.Find(arr);
            Assert.Equal(arr.Min(), min);
            Assert.Equal(arr.Max(), max);
        }
    }

    [Fact]
    public void Empty_Throws()
    {
        Assert.Throws<ArgumentException>(() => MinMax.Find(Array.Empty<int>()));
    }
}
