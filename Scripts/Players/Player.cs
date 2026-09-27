using System.Collections.Generic;

public class Player
{
	// TODO: Might want to move these to a PlayerState class or something like that eventually, but for now, this works.
	public int Points { get; set; }
	public List<Tile> Hand { get; set; }

	// TODD: Have players be able to respond to match situations. This will probably be an interface.
}