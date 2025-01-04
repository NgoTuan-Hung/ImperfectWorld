public static class ArrayExtension
{
	public static int CumulativeDistributionBinarySearch(this float[] array, int start, int end, float searchValue)
	{
		if (start == end) return start;
		int mid = (start + end) / 2;
		
		if (searchValue <= array[mid])
		{
			end = mid;
			return CumulativeDistributionBinarySearch(array, start, end, searchValue);
		}
		else
		{
			start = mid + 1;
			return CumulativeDistributionBinarySearch(array, start, end, searchValue);	
		}
	}
}