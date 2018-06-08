using System;
using System.IO;
using System.Linq;

using Tools.DataStructures;

namespace Wordament.Model
{
	public enum TileType
	{
		Normal,
		Prefix,
		Suffix,
		Alternating
	}

	/// <summary>
	/// Represents a single position on a Wordament grid, including its
	/// letters and score.
	/// </summary>
	public class Tile
	{
		public string Letters { get; private set; }
		public string AlternateLetters { get; private set; }
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
			AlternateLetters = string.Empty;
			Score = score;
			Location = location;
			Type = type;
		}

		public Tile(string letters, string alternateLetters, int score, GridCell location)
		{
			Letters = letters;
			AlternateLetters = alternateLetters;
			Score = score;
			Location = location;
			Type = TileType.Alternating;
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
				// Parse an alternating Tile
				string[] options = tileStr.Split('/');
				if (options.Length != 2)
					throw new InvalidDataException("String could not be parsed. The format for an alternating is STRING1/STRING2");
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
