A hand display probably should actually be called a "player display" and take in a player(state?)

Player needs to be updated to have the ability to keep track of the drawn tile and melds

Then along with that, Players should be able to (with the UI updating):

1. Receive a drawn tile
2. Make a meld
3. Discard a drawn tile

Player logic needs to make two types of choices:

1. Pick a tile to discard given their hand with a drawn tile
2. Make a call given a full game state (a bit worried about making it clear what exactly they are calling on)

Calls:

* Chii
	* Which chii to make
* Pon
	* Which Pon to make
* Kan
* Pei
	* Should I keep the MajSoul "kita" mistranslation? I'm inclined not to
* Riichi
	* Which tile to discard
		* thinking that if a proposed Riichi call isn't made, the match will then come back to the logic to make a
		  normal discard
* Ron
* Tsumo

(Doing the 9 terminals abortive draw call can be pushed off to later, these are the ones we _need_ to be able to make.)

Matches are created using the constructor, which take in an RNG, a player count, whether or not to use red fives, and
the number of rounds. A match has a set of MatchPlayers that have a state and a logic. In theory, when it comes to UI,
the Match Screen should just be able to assign a MatchPlayer to one of the four seats. That means we might want to
instead of having both the hand and discards on the match screen, create a "PlayerSeat" scene (for Human and CPU) that
the MatchScreen can assign players to.

After being created, the Match can deal out to players using the DealHands method. An initial goal here then, is to
create the PlayerSeat scene for the human player, and have the MatchScreen set up a Match and assign the player to East.
Then we just need to have code that takes the hand as written in the MatchState and displays it in the HandDisplay. I
have already added properties to the PlayerState that includes their hand, their drawn tile, and made melds.

This will also need the Pei Dora list...

Then moving on from this, the MatchPlayer would be responsible for being able to update its MatchState based on the 3
things mentioned in the very first list above. It could also make more sense to put this on the MatchState itself, but
we'll have a better idea as we implement things. (Actually, I probably do want it on the MatchState itself.)

As far as the ordering of how to tackle all of this, I think I will actually pull the current things in the Match Screen
back down to a PlayerSeat element, which will represent a PlayerState class. Then perhaps a few test buttons to be able
to do the top three things. Alternatively, this could be where we start going back and filling in the testing debt. This
now seems that the PlayerState and PlayerLogic are significantly separate enough in their goals that I have a way to
work.

One thing I'll note about the melds, is that if I want to simulate one of the tiles being rotated to represent a claimed
tile, I would need to probably update the MPSZ parser to allow for that. I am also thinking the same for having a
promoted kan, though I note that the LaTeX package expects that it will be a doubled tile. I'm thinking of slightly
changing their implementation to make something like "55'0"5s", where instead of that becoming a 5s, a rotated 5s, two
stacked rotated 0s, and a 5s that it would do a 5s, a rotated 5s, a rotated 0s stacked on that, and then the final 5s.
It strikes me as odd they didn't implement it like this considering red fives are otherwise supported. I can decide one
way or another if I want to accept the same parsing as the LaTeX package if there is no neighboring rotated tile, but I
wouldn't be against just letting it go. I think the big thing is taking a look at other clients and logs and judging how
they would represent that.