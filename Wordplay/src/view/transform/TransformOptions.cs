using CommandLine;

namespace Wordplay.View.Transform
{
	[Verb("transform", HelpText = "Transform one word into a different word one letter at a time")]
	class TransformOptions : BaseOptions
	{
		[Value(0, Required = true, HelpText = "The starting word")]
		public string Start { get; set; }

		[Value(1, Required = true, HelpText = "The ending word")]
		public string End { get; set; }

		[Option('s', "substitutions", Required = false, Default = false,
			HelpText = "Only allow character substitutions, not insertions or deletions")]
		public bool SubstitutionsOnly { get; set; }
	}
}