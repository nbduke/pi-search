using CommandLine;

namespace PiSearch.Scan
{
	class Options
	{
		[Option('p', "pi", Required = true,
			HelpText = "path to the file containing digits of pi")]
		public string Digits { get; set; }

		[Option('d', "dictionary", Required = true,
			HelpText = "path to the dictionary file")]
		public string Dictionary { get; set; }

		[Option('g', "groups-output", Required = false, Default = "groups.txt",
			HelpText = "the file where word groups will be written")]
		public string GroupsOutput { get; set; }

		[Option('t', "stats-output", Required = false, Default = "stats.txt",
			HelpText = "the file where word and group statistics will be written")]
		public string StatsOutput { get; set; }

		[Option('v', "verbose", Required = false,
			HelpText = "causes scanner to print progress reports as it runs")]
		public bool Verbose { get; set; }

		[Option('n', Required = false, Default = 1000,
			HelpText = "the number of word groups to sample in each category")]
		public int Samples { get; set; }

		[Option('l', "group-length", Required = false, Default = 6,
			HelpText = "the minimum length of a word group")]
		public int MinGroupLength { get; set; }

		[Option('s', "group-separation", Required = false, Default = 0,
			HelpText = "the maximum number of characters permitted between words to group them together")]
		public int GroupSeparation { get; set; }

		[Option('x', Required = false, Default = null,
			HelpText = "the maximum number of digits to scan")]
		public long? MaxDigits { get; set; }
	}
}