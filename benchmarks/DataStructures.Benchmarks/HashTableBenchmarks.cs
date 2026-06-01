using BenchmarkDotNet.Attributes;
using DataStructures.Hashing;

namespace DataStructures.Benchmarks;

/// <summary>
/// Compares the two collision strategies against the framework
/// <see cref="Dictionary{TKey,TValue}"/> on insert + lookup. Expectation:
/// open addressing beats chaining on lookup (cache locality), and both lose
/// to the framework dictionary (which is itself open-addressed and heavily
/// tuned). The gap quantifies "how much does a hand-rolled map cost?".
/// </summary>
[MemoryDiagnoser]
public class HashTableBenchmarks
{
    [Params(10_000, 100_000)]
    public int N { get; set; }

    private int[] _keys = [];

    [GlobalSetup]
    public void Setup()
    {
        var rng = new Random(42);
        _keys = new int[N];
        for (var i = 0; i < N; i++)
        {
            _keys[i] = rng.Next();
        }
    }

    [Benchmark(Baseline = true)]
    public int FrameworkDictionary()
    {
        var dict = new Dictionary<int, int>();
        foreach (var k in _keys)
        {
            dict[k] = k;
        }
        var hits = 0;
        foreach (var k in _keys)
        {
            if (dict.TryGetValue(k, out _))
            {
                hits++;
            }
        }
        return hits;
    }

    [Benchmark]
    public int OpenAddressing()
    {
        var table = new OpenAddressingHashTable<int, int>();
        foreach (var k in _keys)
        {
            table.Put(k, k);
        }
        var hits = 0;
        foreach (var k in _keys)
        {
            if (table.TryGet(k, out _))
            {
                hits++;
            }
        }
        return hits;
    }

    [Benchmark]
    public int Chaining()
    {
        var table = new ChainingHashTable<int, int>();
        foreach (var k in _keys)
        {
            table.Put(k, k);
        }
        var hits = 0;
        foreach (var k in _keys)
        {
            if (table.TryGet(k, out _))
            {
                hits++;
            }
        }
        return hits;
    }
}
