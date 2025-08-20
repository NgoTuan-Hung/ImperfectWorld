using System.Collections.Generic;

public static class ListExtension
{
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
}
