/*
 * Tile.cs
 * 
 * Nathan Duke
 * 3/15/15
 * 
 * Contains the Tile class and the TileType enum. Tile represents a single position
 * on a Wordament grid, including its characters and score. TileType is used to
 * differentiate types of Wordament tiles, such as Prefix and Suffix.
 */

using System;
using System.IO;
using System.Linq;

using CommonTools.DataStructures;

namespace Wordament
{
	public enum TileType
	{
		Normal,
		Prefix,
		Suffix,
		EitherOr
	}

	public class Tile
	{
		public string Letters { get; private set; }
		public string OrLetters { get; private set; }
		public int Score { get; private set; }
		public GridCell Location { get; private set; }
		public TileType Type { get; private set; }

		public Tile(string letters, int score, GridCell location)
			: this(letters, score, location, TileType.Normal)
		{
		}

		public Tile(string letters, int score, GridCell location, TileType type)
		{
			Letters = letters;
			OrLetters = string.Empty;
			Score = score;
			Location = location;
			Type = type;
		}

		public Tile(string letters, string orLetters, int score, GridCell location)
		{
			Letters = letters;
			OrLetters = orLetters;
			Score = score;
			Location = location;
			Type = TileType.EitherOr;
		}

		public static Tile Parse(string tileStr, int score, GridCell location)
		{
			if (string.IsNullOrEmpty(tileStr))
				throw new ArgumentException("String representation of a tile must be a non-empty string.");

			if (tileStr.Contains('-'))
			{
				// Parse a prefix or suffix Tile
				if (tileStr.StartsWith("-"))
					return new Tile(tileStr.Substring(1), score, location, TileType.Suffix);
				else if (tileStr.EndsWith("-"))
					return new Tile(tileStr.Substring(0, tileStr.Length - 1), score, location, TileType.Prefix);
				else
					throw new InvalidDataException("String could not be parsed. The format for a prefix or suffix tile is PREFIX- or -SUFFIX");
			}
			else if (tileStr.Contains('/'))
			{
				// Parse an either/or Tile
				string[] options = tileStr.Split('/');
				if (options.Length != 2)
					throw new InvalidDataException("String could not be parsed. The format for an either-or tile is STRING1/STRING2");
				else
					return new Tile(options[0], options[1], score, location);
			}
			else
			{
				// Normal Tile
				return new Tile(tileStr, score, location);
			}
		}

		public static Tile Parse(string tileStr, string scoreStr, GridCell location)
		{
			int score;
			if (!Int32.TryParse(scoreStr, out score))
				throw new InvalidDataException("Score parameter is not convertible to a 32-bit signed integer.");

			return Parse(tileStr, score, location);
		}
	}
}
