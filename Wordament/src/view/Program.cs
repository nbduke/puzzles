using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tools.DataStructures;

namespace Wordament.View
{
	static class Program
	{
		public static PrefixTreeDictionary Dictionary = null;

		private static string tileScoresFilename = "../../resources/tile_scores.txt";
		private static string dictFilename = "../../resources/large_dict.txt";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// Load the set of images used to represent tiles
			if (!TileImageTable.LoadImages())
				return;

			// Load the tile scores from a file
			if (!TileScoreTable.BuildFromFile(tileScoresFilename))
				return;

			// Load the dictionary from a file
			if (!BuildDictionaryFromFile(dictFilename))
				return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new StartForm());
		}

		static bool BuildDictionaryFromFile(string filename)
		{
			Dictionary = new PrefixTreeDictionary();
			using (StreamReader inputFile = new StreamReader(filename))
			{
				while (!inputFile.EndOfStream)
				{
					string word = inputFile.ReadLine().Trim().ToLower();
					if (!string.IsNullOrEmpty(word))
						Dictionary.Add(word);
				}
			}

			return Dictionary.Count > 0;
		}
	}
}
