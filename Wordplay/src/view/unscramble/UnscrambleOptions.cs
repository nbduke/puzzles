using CommandLine;

namespace Wordplay.View.Unscramble
{
	class UnscrambleOptions
	{
		[Value(0, Required = true, MetaName = "letters", HelpText = "The scrambled letters")]
		public string Letters { get; set; }

		[Option('m', "minWordLength", Required = false, Default = -1,
			HelpText = "The minimum length of words to find (defaults to length of scrambled word")]
		public int MinWordLength { get; set; }
	}
}