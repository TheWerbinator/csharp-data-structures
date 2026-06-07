namespace DataStructures.Algorithms.Selection;

/// <summary>
/// Find the minimum and maximum of an unordered sequence simultaneously
/// in ⌈3n/2⌉ - 2 comparisons — the optimal comparison count, beating the
/// naive 2n - 2 you'd get from running min and max separately.
/// <para>
/// Trick: process elements in pairs. Compare the two against each other
/// first (1 comparison), then compare the smaller against the running min
/// (1) and the larger against the running max (1) — 3 comparisons per
/// pair of 2 elements.
/// </para>
/// </summary>
public static class MinMax
{
    /// <summary>
    /// (Min, Max) of the sequence. Throws on empty input.
    /// </summary>
    public static (T Min, T Max) Find<T>(IReadOnlyList<T> values, IComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.Count == 0)
        {
            throw new ArgumentException("Sequence must not be empty", nameof(values));
        }
        comparer ??= Comparer<T>.Default;

        T min, max;
        int start;
        if (values.Count % 2 == 1)
        {
            min = max = values[0];
            start = 1;
        }
        else
        {
            if (comparer.Compare(values[0], values[1]) < 0)
            {
                (min, max) = (values[0], values[1]);
            }
            else
            {
                (min, max) = (values[1], values[0]);
            }
            start = 2;
        }

        for (var i = start; i + 1 < values.Count; i += 2)
        {
            T pairLow, pairHigh;
            if (comparer.Compare(values[i], values[i + 1]) < 0)
            {
                (pairLow, pairHigh) = (values[i], values[i + 1]);
            }
            else
            {
                (pairLow, pairHigh) = (values[i + 1], values[i]);
            }
            if (comparer.Compare(pairLow, min) < 0) min = pairLow;
            if (comparer.Compare(pairHigh, max) > 0) max = pairHigh;
        }
        return (min, max);
    }
}
