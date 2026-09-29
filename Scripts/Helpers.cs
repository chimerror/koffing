public static class Helpers
{
	public static int GetSafeIndex(int index, int numberOfItems)
	{
		if (index < 0)
		{
			return index + numberOfItems;
		}
		else if (index >= numberOfItems)
		{
			return index - numberOfItems;
		}
		else
		{
			return index;
		}
	}
}