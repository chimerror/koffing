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
		return 11;
	}
}