using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ListExtension
{
    private static readonly System.Random _rand = new();

    /// <summary>
    /// Working for most edge case, there are still some unexpected case like
    /// [0, 1] and find 0 would return 0;
    /// or List with zero element;
    /// </summary>
    /// <param name="list"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public static int CumulativeDistributionBinarySearch(
        this List<float> list,
        int start,
        int end,
        float searchValue
    )
    {
        if (start == end)
            return start;
        int mid = (start + end) / 2;

        if (searchValue <= list[mid])
        {
            end = mid;
            return CumulativeDistributionBinarySearch(list, start, end, searchValue);
        }
        else
        {
            start = mid + 1;
            return CumulativeDistributionBinarySearch(list, start, end, searchValue);
        }
    }

    public static T GetRandomElement<T>(this List<T> list)
        where T : class
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Returns 'numberToSample' unique random indices from the list using reservoir sampling.
    /// </summary>
    public static List<int> UniqueSample<T>(this List<T> list, int numberToSample)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (numberToSample < 0 || numberToSample > list.Count)
            throw new ArgumentOutOfRangeException(
                nameof(numberToSample),
                "numberToSample must be between 0 and the list count."
            );

        List<int> reservoir = new List<int>(numberToSample);

        // Fill the reservoir array with the first 'numberToSample' indices
        for (int i = 0; i < numberToSample; i++)
        {
            reservoir.Add(i);
        }

        // Replace elements with gradually decreasing probability
        for (int i = numberToSample; i < list.Count; i++)
        {
            int j = _rand.Next(i + 1); // random integer between 0 and i inclusive
            if (j < numberToSample)
            {
                reservoir[j] = i;
            }
        }

        return reservoir;
    }
}
