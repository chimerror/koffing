using Koffing.Matches;
using Koffing.Calls;
using System.Collections.Generic;

namespace Koffing.Players;

public interface IPlayerLogic
{
	/// <summary>
	/// Prompts the player to choose a tile in their hand to discard.
	/// </summary>
	/// <param name="match">The current match state for the player logic to make a decision on.</param>
	/// <param name="playerIndex">The index of the player the logic should make a decision for.</param>
	/// <returns>
	/// The index (in <see cref="PlayerState.Hand"/>) to discard, or <c>-1</c> to discard the drawn tile in
	/// <see cref="PlayerState.DrawnTile"/>.
	/// </returns>
	int MakeDiscard(Match match, int playerIndex);

	/// <summary>
	/// Prompts the player to make a call.
	/// </summary>
	/// <param name="match">The current match state for the player logic to make a decision on.</param>
	/// <param name="playerIndex">The index of the player the logic should make a decision for.</param>
	/// <param name="legalCalls">
	/// An <see cref="IEnumerable{T}"/> of possible legal <see cref="Call"/>s.
	/// </param>
	/// <returns>A <see cref="Call"/> to make, or <see langword="null"/> to make no call.</returns>
	/// <remarks>
	/// Player logic is not strictly limited to making legal calls, though choosing to make an illegal call can result
	/// in penalization.
	/// </remarks>
	Call MakeCall(Match match, int playerIndex, IEnumerable<Call> legalCalls);
}