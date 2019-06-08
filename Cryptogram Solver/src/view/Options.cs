using CommandLine;

namespace CryptogramSolver.View
{
	class Options
	{
		[Value(0, Required = true, MetaName = "cryptogram",
			HelpText = "The cryptogram puzzle"
		)]
		public string Cryptogram { get; set; }

		[Option('d', "dictionary", Required = true, HelpText = "The dictionary file")]
		public string Dictionary { get; set; }
	}
}