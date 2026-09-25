public interface IRandomNumberGenerator
{
	ulong Seed { get; set; }
	void Randomize();
	int GetIntegerInRange(int from, int to);
}