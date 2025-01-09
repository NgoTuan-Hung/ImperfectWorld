using System.Collections.Generic;

public static class ListExtension
{
	public static int CumulativeDistributionBinarySearch(this List<float> list, int start, int end, float searchValue)
	{
		if (start == end) return start;
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