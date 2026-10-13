# The Koffing Mahjong Engine

The Koffing Mahjong Engine is an engine for playing riichi mahjong written in C#. This initial version is targeted
towards the Godot game engine, but the intention is to avoid Godot-specific code in the main parts of the library,
towards the aim of eventually being able to use the engine in other game engines such as Unity.

As a C#-based engine, Koffing makes large use of LINQ, generally representing hands as collections of tiles and
providing functions to generate other collections of objects that group the tiles into classes with more semantic
meanings. Thus, the library class hierarchy serves to encapsulate the rules and strategy of mahjong, freeing library
clients from being overly concerned with the details.

As an engine, it is _very_ early in the initial implementation of this and thus very much a work in progress. Despite
that, the engine also has a strong focus on ensuring all code is rigorously tested to adhere to the rules of mahjong.
We favor being correct more than we favor, say, performance and speed of development. On that last point, we
strictly reject LLM use in our development. All code has been written completely by hand, and we intend to
continue that.

We hope that eventually Koffing can be a very useful open-source library for game developers who wish to write mahjong
clients and CPU players, but also so that mahjong players can analyze their own games. We love the game of mahjong and
we want to be able to have more people enjoy it, and this is our way of doing so.