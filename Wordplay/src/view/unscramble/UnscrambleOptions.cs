using CommandLine;

namespace Wordplay.View.Unscramble
{
	[Verb("unscramble", HelpText = "Unscramble a string of letters to find words")]
	class UnscrambleOptions : BaseOptions
	{
		[Value(0, Required = true, HelpText = "The scrambled letters")]
		public string Letters { get; set; }

		[Option('m', "minWordLength", Required = false,
			HelpText = "The minimum length of words to find (defaults to length of scrambled word")]
		public int MinWordLength { get; set; }
	}
}