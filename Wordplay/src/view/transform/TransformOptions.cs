using CommandLine;

namespace Wordplay.View.Transform
{
	class TransformOptions
	{
		[Value(0, Required = true, MetaName = "start", HelpText = "The starting word")]
		public string Start { get; set; }

		[Value(1, Required = true, MetaName = "end", HelpText = "The ending word")]
		public string End { get; set; }

		[Option('s', "substitutions", Required = false, Default = false,
			HelpText = "Only allow character substitutions, not insertions or deletions")]
		public bool SubstitutionsOnly { get; set; }
	}
}