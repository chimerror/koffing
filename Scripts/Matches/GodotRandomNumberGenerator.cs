using Godot;

public class GodotRandomNumberGenerator : IRandomNumberGenerator
{
	private readonly RandomNumberGenerator _rng = new();

	public GodotRandomNumberGenerator()
	{
	}

	public ulong Seed { get => _rng.Seed; set => _rng.Seed = value; }

	public void Randomize()
	{
		_rng.Randomize();
	}

	public int GetIntegerInRange(int from, int to)
	{
		return _rng.RandiRange(from, to);
	}
}