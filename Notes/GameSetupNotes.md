1. Deal starting scoring sticks
2. Assign Seat Wins
3. Set Round Wind
4. Display (Announce) Score
5. Build Wall
6. Break Wall
7. Deal Tiles
8. Set Up Dead Wall
9. Start hand

Four - 136 tiles [0..135] (34 per side, 17 stacks)
	East - 0 to 33
	North - 34 to 67
	West - 68 to 101
	South - 102 to 135

	Break Rolls (add die roll * 2 to starting index for break index):
		(1), 5, 9 - East (mod 1)
		2, 6, 10 - South (mod 2)
		3, 7, 11 - West (mod 3)
		4, 8, 12 - North (mod 0)

Three - 108 tiles [0..107] (36 per side, 18 stacks)
	East - 0 to 35
	West - 36 to 71
	South - 72 to 107

	Break Rolls (works the same):
		(1), 4, 7, 10 - East (mod 1)
		2, 5, 8, 11 - South (mod 2)
		3, 6, 9, 12 - West (mod 0)