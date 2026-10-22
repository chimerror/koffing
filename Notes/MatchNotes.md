A hand display probably should actually be called a "player display" and take in a player(state?)

Player needs to be updated to have the ability to keep track of the drawn tile and melds

Then along with that, Players should be able to (with the UI updating):

1. Receive a drawn tile
2. Make a meld
3. Discard a drawn tile

Player logic needs to make two types of choices:

1. Pick a tile to discard given their hand with a drawn tile
2. Make a call given a full game state (a bit worried about making it clear what exactly they are calling on)