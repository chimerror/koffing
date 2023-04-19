using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
	public static List<T> Shuffle<T>(this RandomNumberGenerator rng, IEnumerable<T> originalList)
	{
		var list = new List<T>(originalList);
		var result = new List<T>();
		while (list.Count > 0)
		{
			var indexToRemove = rng.RandiRange(0, list.Count - 1);
			result.Add(list[indexToRemove]);
			list.RemoveAt(indexToRemove);
		}
		return result;
	}
}