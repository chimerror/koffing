public class Orphan : Wait
{
	public Orphan() : base()
	{
	}

	public Orphan(Tile tile) : base([tile])
	{
	}

	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 11;
	}
}