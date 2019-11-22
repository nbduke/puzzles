using System;

using CommandLine;

namespace Wordplay.View
{
	class Program
	{
		public static void Main(string[] args)
		{
			Parser.Default.ParseArguments<ProgramOptions>(args).WithParsed(options =>
			{
				string game = options.Game.Trim().ToLower();
				if (game == "transform")
					Transform.Transform.Run(options.DictionaryFile);
				else if (game == "unscramble")
					Unscramble.Unscramble.Run(options.DictionaryFile);
				else
					Console.WriteLine("You must supply either 'transform' or 'unscramble' as an argument.");
			});
		}
	}
}
