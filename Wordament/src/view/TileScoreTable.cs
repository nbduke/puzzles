/*
 * TileScoreTable.cs
 * 
 * Nathan Duke
 * 7/29/13
 * WordamentApp
 * 
 * Contains the TileScoreTable static class.
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace Wordament.View
{
	/*
	 * This static class manages a table mapping tiles to their scores (used for auto-scoring in the Wordament
	 * App GUI). The table must be built by a call to BuildFromFile().
	 */
	static class TileScoreTable
	{
		/*
		 * The table mapping string representations of tiles to their integer scores.
		 */
		private static Dictionary<string, int> Table = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

		/*
		 * Opens the specified file and adds entries to Table. The function expects a text file in which each 
		 * line has the following format:
		 *      TILE_STRING SCORE
		 */
		public static bool BuildFromFile(string filename)
		{
			try
			{
				using (StreamReader sr = new StreamReader(File.OpenRead(filename)))
				{
					while (!sr.EndOfStream)
					{
						string line = sr.ReadLine().Trim();
						if (!string.IsNullOrEmpty(line))
						{
							string[] parts = line.Split(' ');
							if (parts.Length < 2)
							{
								Console.WriteLine("File could not be parsed. Format is TILE_STRING SCORE");
								return false;
							}

							Table.Add(parts[0], Int32.Parse(parts[1]));
						}
					}

					return true;
				}
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Could not find the file {0}.", filename);
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				Console.WriteLine("You do not have permission to read file {0}.", filename);
				return false;
			}
		}

		/*
		 * Looks up the score of the tile represented by rawTileString. If the string represents a suffix, 
		 * prefix, either/or, or digram tile, a separate category may be used to score it. If the string is 
		 * not a key in the table, 0 is returned.
		 */
		public static int LookupScore(string rawTileString)
		{
			if (string.IsNullOrEmpty(rawTileString))
				throw new ArgumentException("rawTileString must be a non-empty string.");

			try
			{
				return Table[rawTileString];
			}
			catch (KeyNotFoundException)
			{
				if (rawTileString.StartsWith("-"))
					return Table["suffix"];
				else if (rawTileString.EndsWith("-"))
					return Table["prefix"];
				else if (rawTileString.Contains("/"))
					return Table["either"];
				else if (rawTileString.Length > 1)
					return Table["digram"];
				else
					return 0;
			}
		}
	}
}
