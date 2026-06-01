using BenchmarkDotNet.Attributes;
using DataStructures.Sorting;

namespace DataStructures.Benchmarks;

/// <summary>
/// Compares the from-scratch sorts against the framework's introsort
/// (<see cref="List{T}.Sort()"/>). The framework wins — that's the point.
/// The interesting number is *how much* it wins by, and how the from-scratch
/// O(n log n) sorts compare to each other.
/// </summary>
[MemoryDiagnoser]
public class SortingBenchmarks
{
    [Params(1_000, 100_000)]
    public int N { get; set; }

    private int[] _data = [];

    [GlobalSetup]
    public void Setup()
    {
        var rng = new Random(42);
        _data = new int[N];
        for (var i = 0; i < N; i++)
        {
            _data[i] = rng.Next();
        }
    }

    [Benchmark(Baseline = true)]
    public int[] FrameworkSort()
    {
        var copy = (int[])_data.Clone();
        Array.Sort(copy);
        return copy;
    }

    [Benchmark]
    public List<int> MergeSort()
    {
        var copy = _data.ToList();
        Sorts.MergeSort(copy);
        return copy;
    }

    [Benchmark]
    public List<int> QuickSort()
    {
        var copy = _data.ToList();
        Sorts.QuickSort(copy);
        return copy;
    }

    [Benchmark]
    public List<int> HeapSort()
    {
        var copy = _data.ToList();
        Sorts.HeapSort(copy);
        return copy;
    }
}
