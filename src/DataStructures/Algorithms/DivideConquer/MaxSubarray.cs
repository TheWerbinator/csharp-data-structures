namespace DataStructures.Algorithms.DivideConquer;

/// <summary>
/// Maximum contiguous-subarray-sum, the way CLRS chapter 4 teaches it
/// (divide-and-conquer, O(n log n)) and the way you'd actually ship it
/// (Kadane's, O(n)). Both return the same result; Kadane wins in practice.
/// </summary>
public static class MaxSubarray
{
    /// <summary>
    /// Inclusive index range and the sum it produces.
    /// </summary>
    public readonly record struct Result(int Low, int High, long Sum);

    /// <summary>
    /// CLRS divide-and-conquer. O(n log n) time, O(log n) stack depth.
    /// Educational — the cross-the-midpoint step is the trick that makes
    /// the recurrence work; the standalone halves cover the cases where
    /// the max subarray lies entirely on one side.
    /// </summary>
    public static Result DivideAndConquer(IReadOnlyList<int> array)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (array.Count == 0)
        {
            throw new ArgumentException("Array must not be empty", nameof(array));
        }
        return Solve(array, 0, array.Count - 1);
    }

    private static Result Solve(IReadOnlyList<int> a, int low, int high)
    {
        if (low == high)
        {
            return new Result(low, high, a[low]);
        }
        var mid = low + ((high - low) >> 1);
        var left = Solve(a, low, mid);
        var right = Solve(a, mid + 1, high);
        var cross = MaxCrossing(a, low, mid, high);
        if (left.Sum >= right.Sum && left.Sum >= cross.Sum) return left;
        if (right.Sum >= cross.Sum) return right;
        return cross;
    }

    private static Result MaxCrossing(IReadOnlyList<int> a, int low, int mid, int high)
    {
        long leftSum = long.MinValue, runningSum = 0;
        var maxLeft = mid;
        for (var i = mid; i >= low; i--)
        {
            runningSum += a[i];
            if (runningSum > leftSum)
            {
                leftSum = runningSum;
                maxLeft = i;
            }
        }
        long rightSum = long.MinValue;
        runningSum = 0;
        var maxRight = mid + 1;
        for (var j = mid + 1; j <= high; j++)
        {
            runningSum += a[j];
            if (runningSum > rightSum)
            {
                rightSum = runningSum;
                maxRight = j;
            }
        }
        return new Result(maxLeft, maxRight, leftSum + rightSum);
    }

    /// <summary>
    /// Kadane's algorithm. Single linear pass that tracks the best
    /// subarray ending at the current index. O(n) time, O(1) space.
    /// </summary>
    public static Result Kadane(IReadOnlyList<int> array)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (array.Count == 0)
        {
            throw new ArgumentException("Array must not be empty", nameof(array));
        }
        long bestSum = array[0], currentSum = array[0];
        int bestLow = 0, bestHigh = 0, currentLow = 0;
        for (var i = 1; i < array.Count; i++)
        {
            // Either extend the current subarray, or start a new one at i.
            if (currentSum + array[i] >= array[i])
            {
                currentSum += array[i];
            }
            else
            {
                currentLow = i;
                currentSum = array[i];
            }
            if (currentSum > bestSum)
            {
                bestSum = currentSum;
                bestLow = currentLow;
                bestHigh = i;
            }
        }
        return new Result(bestLow, bestHigh, bestSum);
    }
}
