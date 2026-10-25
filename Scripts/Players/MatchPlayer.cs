namespace Koffing.Players;

public class MatchPlayer
{
	public PlayerState State { get; set; }
	public IPlayerLogic PlayerLogic { get; set; }
}