using CommandLine;

namespace CryptogramSolver.View
{
	class Options
	{
		[Value(0, Required = false, MetaName = "cryptogram",
			HelpText = "The cryptogram puzzle"
		)]
		public string Cryptogram { get; set; }

		[Option('f', "file", Required = false,
			HelpText = "The path to a file containing the cryptogram to solve"
		)]
		public string CryptogramFile { get; set; }

		[Option('d', "dictionary", Required = true, HelpText = "The dictionary file")]
		public string Dictionary { get; set; }
	}
}