namespace DataStructures.Algorithms.Graphs;

/// <summary>
/// Breadth-first search over an unweighted graph represented as an
/// adjacency list. Computes shortest-path distance (in edges) from a
/// source vertex to every reachable vertex, and the predecessor map
/// needed to reconstruct a shortest path.
/// </summary>
/// <typeparam name="TVertex">Vertex identifier — must be equatable.</typeparam>
public static class BreadthFirstSearch<TVertex>
    where TVertex : notnull
{
    /// <summary>
    /// Result of a BFS traversal.
    /// </summary>
    /// <param name="Distance">
    /// Distance in edges from <c>source</c>. Reachable vertices map to a
    /// non-negative int; unreachable vertices are absent.
    /// </param>
    /// <param name="Predecessor">
    /// For each reachable vertex other than the source, the previous
    /// vertex on a shortest path from <c>source</c>. The source has no
    /// entry — that's the terminating signal during path reconstruction
    /// (rather than a sentinel value, which would collide with vertex 0
    /// when <typeparamref name="TVertex"/> is <see cref="int"/>).
    /// </param>
    public readonly record struct Result(
        IReadOnlyDictionary<TVertex, int> Distance,
        IReadOnlyDictionary<TVertex, TVertex> Predecessor);

    /// <summary>
    /// Run BFS from <paramref name="source"/> over the graph encoded by
    /// <paramref name="adjacency"/>. O(V + E) time and space.
    /// </summary>
    public static Result Run(IReadOnlyDictionary<TVertex, IReadOnlyList<TVertex>> adjacency, TVertex source)
    {
        ArgumentNullException.ThrowIfNull(adjacency);
        ArgumentNullException.ThrowIfNull(source);
        if (!adjacency.ContainsKey(source))
        {
            throw new ArgumentException($"Source vertex {source} is not in the graph", nameof(source));
        }

        var distance = new Dictionary<TVertex, int> { [source] = 0 };
        var predecessor = new Dictionary<TVertex, TVertex>();
        var queue = new Queue<TVertex>();
        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            if (!adjacency.TryGetValue(u, out var neighbors))
            {
                continue;
            }
            foreach (var v in neighbors)
            {
                if (distance.ContainsKey(v))
                {
                    continue;
                }
                distance[v] = distance[u] + 1;
                predecessor[v] = u;
                queue.Enqueue(v);
            }
        }

        return new Result(distance, predecessor);
    }

    /// <summary>
    /// Reconstruct a shortest path from <c>source</c> to
    /// <paramref name="target"/> using a BFS result. Returns an empty list
    /// if no path exists.
    /// </summary>
    public static IReadOnlyList<TVertex> PathTo(Result result, TVertex target)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (!result.Distance.ContainsKey(target))
        {
            return Array.Empty<TVertex>();
        }
        var path = new List<TVertex> { target };
        var current = target;
        while (result.Predecessor.TryGetValue(current, out var prev))
        {
            path.Add(prev);
            current = prev;
        }
        path.Reverse();
        return path;
    }
}
