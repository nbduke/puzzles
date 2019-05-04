using CommandLine;

namespace Wordplay.View
{
	class BaseOptions
	{
		[Option('d', "dictionary", Required = true, HelpText = "The dictionary to use")]
		public string DictionaryFile { get; set; }
	}
}