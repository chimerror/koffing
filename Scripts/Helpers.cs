namespace Koffing;

/// <summary>
/// Useful helper functions.
/// </summary>
public static class Helpers
{
	/// <summary>
	/// Returns a "safe" index in the interval from 0 to the specified number of items minus one.
	/// </summary>
	/// <remarks>
	/// This function is not particularly robust in its fixes here, and will return invalid indices when given indices
	/// that are not in interval from negative <paramref name="numberOfItems"/> to one less than twice
	/// <paramref name="numberOfItems"/>.
	/// </remarks>
	/// <param name="index">The index to convert.</param>
	/// <param name="numberOfItems">
	/// The number of items in the array or list that safe indices are being generated for.
	/// </param>
	/// <returns><paramref name="index"/> converted to be within the "safe" range.</returns>
	public static int GetSafeIndex(int index, int numberOfItems)
	{
		// UNTESTED: We test indirectly by just doing the wall tests, but it might be worth testing it more fully.
		// TODO: If we do start testing this, we'll also probably have to add code to cover a few more cases such as
		// indices that need to be adjusted multiple times, and non-positive numberOfItems.
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