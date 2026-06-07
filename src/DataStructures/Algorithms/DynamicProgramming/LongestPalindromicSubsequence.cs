namespace DataStructures.Algorithms.DynamicProgramming;

/// <summary>
/// Longest palindromic subsequence (LPS): given a string, find the longest
/// subsequence that reads the same forwards and backwards. Note this is
/// <b>subsequence</b>, not substring — characters need not be contiguous.
/// <para>
/// Standard O(n²) bottom-up DP. Builds the length table first, then walks
/// it back to reconstruct an actual palindrome witness.
/// </para>
/// </summary>
public static class LongestPalindromicSubsequence
{
    /// <summary>
    /// Returns the length of the longest palindromic subsequence of <paramref name="s"/>.
    /// O(n²) time, O(n²) space.
    /// </summary>
    public static int Length(string s)
    {
        ArgumentNullException.ThrowIfNull(s);
        if (s.Length == 0)
        {
            return 0;
        }
        var dp = BuildLengthTable(s);
        return dp[0, s.Length - 1];
    }

    /// <summary>
    /// Returns one (of possibly many) longest palindromic subsequences of
    /// <paramref name="s"/>. Reconstructs by walking the DP table backwards.
    /// </summary>
    public static string Witness(string s)
    {
        ArgumentNullException.ThrowIfNull(s);
        if (s.Length == 0)
        {
            return string.Empty;
        }
        var dp = BuildLengthTable(s);
        return Reconstruct(s, dp, 0, s.Length - 1);
    }

    private static int[,] BuildLengthTable(string s)
    {
        var n = s.Length;
        var dp = new int[n, n];
        for (var i = 0; i < n; i++)
        {
            dp[i, i] = 1;
        }
        // Fill by increasing substring length. dp[i, j] depends on dp[i+1, j-1],
        // dp[i+1, j], dp[i, j-1] — all already filled when we get here.
        for (var len = 2; len <= n; len++)
        {
            for (var i = 0; i + len - 1 < n; i++)
            {
                var j = i + len - 1;
                if (s[i] == s[j])
                {
                    dp[i, j] = (len == 2 ? 0 : dp[i + 1, j - 1]) + 2;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i + 1, j], dp[i, j - 1]);
                }
            }
        }
        return dp;
    }

    private static string Reconstruct(string s, int[,] dp, int i, int j)
    {
        if (i > j)
        {
            return string.Empty;
        }
        if (i == j)
        {
            return s[i].ToString();
        }
        if (s[i] == s[j])
        {
            return s[i] + Reconstruct(s, dp, i + 1, j - 1) + s[j];
        }
        return dp[i + 1, j] >= dp[i, j - 1]
            ? Reconstruct(s, dp, i + 1, j)
            : Reconstruct(s, dp, i, j - 1);
    }
}
