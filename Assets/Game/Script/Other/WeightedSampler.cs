using System;
using System.Collections.Generic;

public static class WeightedSampler
{
    private static Random rng = new Random();

    public static List<int> SampleUnique(int nToPick, float[] weights)
    {
        int count = weights.Length;
        List<int> result = new List<int>(nToPick);
        HashSet<int> picked = new HashSet<int>();

        // Build prefix sum array
        double[] prefix = new double[count];
        double total = 0;
        for (int i = 0; i < count; i++)
        {
            total += weights[i];
            prefix[i] = total;
        }

        // Pick values
        while (result.Count < nToPick)
        {
            double r = rng.NextDouble() * total;

            // Binary search
            int idx = Array.BinarySearch(prefix, r);
            if (idx < 0)
                idx = ~idx;

            if (picked.Add(idx))
                result.Add(idx);
        }

        return result;
    }
}
