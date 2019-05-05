using CommandLine;

namespace Wordplay.View
{
	class ProgramOptions
	{
		[Option('d', "dictionary", Required = true, HelpText = "The dictionary to use")]
		public string DictionaryFile { get; set; }

		[Value(0, MetaName = "game", Required = true,
			HelpText = "The word game to play: either transform or unscramble")]
		public string Game { get; set; }
	}
}