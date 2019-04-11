using System.Collections.Generic;
using System.IO;
using System.Text;

using PiSearch.Core;
using Tools.DataStructures;

namespace PiSearch.Scan
{
	class StatsWriter
	{
		private const string Form =
@"============================================================
					WORD STATISTICS
============================================================
Total words found:			{0}
Unique words found:			{1}
Total length of words:		{2}
Average word length:		{3:0}
Average word frequency:		{4:0}

					{5}
					{6}
					{7}
					{8}


============================================================
					GROUP STATISTICS
============================================================
Total groups created:		{9}
Total length of groups:		{10}
Total words within groups:	{11}
Average group length:		{12:0}
Average words per group:	{13:0}
";

		public static void Write(
			WordStats wordStats,
			SortedGroupStore groups,
			string filename
		)
		{
			using (var output = new StreamWriter(filename))
			{
				output.Write(FillForm(wordStats, groups));
			}
		}

		private static string FillForm(WordStats wordStats, SortedGroupStore groups)
		{
			return string.Format(
				Form,
				wordStats.TotalWordsFound,
				wordStats.UniqueWordsFound,
				wordStats.TotalLengthOfWords,
				wordStats.AverageWordLength,
				wordStats.AverageWordFrequency,
				GetWordList(wordStats.Longest, "Longest Words"),
				GetWordList(wordStats.Shortest, "Shortest Words"),
				GetFrequencyWordList(wordStats.MostCommon, wordStats.Histogram, "Most Common Words"),
				GetFrequencyWordList(wordStats.LeastCommon, wordStats.Histogram, "Least Common Words"),
				groups.TotalGroupsAdded,
				groups.TotalLengthOfGroups,
				groups.TotalWordsWithinGroups,
				groups.AverageGroupLength,
				groups.AverageWordsPerGroup
			);
		}

		private static string GetWordList(IEnumerable<string> list, string name)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine($"{WordStats.Samples} {name}");

			int index = 1;
			foreach (string word in list)
			{
				builder.AppendLine($"{index++}.\t{word}");
			}

			return builder.ToString();
		}

		private static string GetFrequencyWordList(
			IEnumerable<string> list,
			Counter<string> histogram,
			string name
		)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine($"{WordStats.Samples} {name}");

			int index = 1;
			foreach (string word in list)
			{
				int count = histogram[word];
				string occurrence = "occurrence";
				if (count > 1)
					occurrence += 's';
				builder.AppendLine($"{index++}.\t{word}\t\t({count} {occurrence})");
			}

			return builder.ToString();
		}
	}
}