using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using CommandLine;
using PiSearch.Core;
using Tools.DataStructures;

namespace PiSearch.Scan
{
	class Program
	{
		private const long TotalDigits = 1000000000;
		private const long ProgressInterval = TotalDigits / 100;

		static void Main(string[] args)
		{
			var parseResult = Parser.Default.ParseArguments<Options>(args);
			parseResult.WithParsed(RunScan);
		}

		private static void RunScan(Options options)
		{
			var dictionary = ParseDictionary(options.Dictionary);
			if (dictionary.Count == 0)
			{
				Console.WriteLine("The dictionary is empty.");
				return;
			}

			if (options.Verbose)
				Console.WriteLine($"Dictionary loaded with {dictionary.Count} words.");

			using (var digitsFile = new FileStream(options.Digits, FileMode.Open))
			{
				var digitEnumerator = new DigitEnumerator(digitsFile);
				var scanner = new PiScanner();
				var groupStore = new SortedGroupStore(
					options.Samples,
					options.MinGroupLength
				);

				if (options.Verbose)
					scanner.ProgressEvent += PrintProgressReport;

				scanner.Scan(
					digitEnumerator,
					dictionary,
					groupStore,
					options.GroupSeparation,
					ProgressInterval,
					options.MaxDigits
				).ContinueWith(scanTask =>
				{
					ScanResults results = scanTask.Result;
					StatsWriter.Write(results.WordStats, groupStore, options.StatsOutput);
					GroupsWriter.Write(groupStore, options.GroupsOutput);

					if (options.Verbose)
						PrintFinalReport(results, groupStore);
				}).Wait();
			}
		}

		private static PrefixTreeDictionary ParseDictionary(string filename)
		{
			var dictionary = new PrefixTreeDictionary();
			using (var file = new StreamReader(filename))
			{
				while (!file.EndOfStream)
				{
					string word = file.ReadLine().Trim().ToLower();
					if (word.Length > 0)
						dictionary.Add(word);
				}
			}

			return dictionary;
		}

		private static void PrintProgressReport(ProgressReport report)
		{
			double percentDigitsRead = report.DigitsRead / (double)ProgressInterval;
			Console.WriteLine(
				"{0:0}%\t {1} words, {2} groups",
				percentDigitsRead,
				report.UniqueWordsFound,
				report.WordGroupsCreated
			);
		}

		private static void PrintFinalReport(ScanResults results, SortedGroupStore groups)
		{
			Console.WriteLine($"Read {results.DigitsRead} digits");
			Console.WriteLine(
				$"Found {results.WordStats.TotalWordsFound} words, {results.WordStats.UniqueWordsFound} unique"
			);
			Console.WriteLine($"Created {groups.TotalGroupsAdded} word groups");
		}
	}
}
