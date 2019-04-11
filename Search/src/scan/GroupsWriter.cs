using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using PiSearch.Core;

namespace PiSearch.Scan
{
	class GroupsWriter
	{
		private const string SectionHeader =
@"============================================================
				{0} {1}
============================================================";

		public static void Write(SortedGroupStore groupStore, string filename)
		{
			string extension = filename.Substring(filename.LastIndexOf('.'));
			if (extension == ".csv")
				WriteCSVFile(groupStore, filename);
			else
				WriteCategorizedGroups(groupStore, filename);
		}

		private static void WriteCSVFile(SortedGroupStore groupStore, string filename)
		{
			using (var output = new StreamWriter(filename))
			{
				HashSet<WordGroup> allGroups = new HashSet<WordGroup>(
					groupStore.Longest.Groups
					.Concat(groupStore.MostWords.Groups)
					.Concat(groupStore.LongestAverageWords.Groups)
					.Concat(groupStore.LongestMaxWords.Groups)
				);

				foreach (var group in allGroups)
				{
					WriteGroup(output, group);
				}
			}
		}

		private static void WriteGroup(StreamWriter output, WordGroup group)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat(
				"{0},{1},{2},{3},",
				group.Location.StartIndex,
				group.Location.EndIndex,
				group.WordCount,
				group.Characters
			);

			builder.Append("[");
			foreach (string word in group.Words)
			{
				builder.Append(word);
				builder.Append(' ');
			}
			builder.Length--;
			builder.Append(']');

			output.WriteLine(builder.ToString());
		}

		private static void WriteCategorizedGroups(SortedGroupStore groupStore, string filename)
		{
			using (var output = new StreamWriter(filename))
			{
				WriteSection(
					output,
					groupStore.Longest,
					groupStore.Samples
				);
				WriteSection(
					output,
					groupStore.MostWords,
					groupStore.Samples
				);
				WriteSection(
					output,
					groupStore.LongestAverageWords,
					groupStore.Samples
				);
				WriteSection(
					output,
					groupStore.LongestMaxWords,
					groupStore.Samples
				);
			}
		}

		private static void WriteSection(
			StreamWriter output,
			SortedGroupList section,
			int samples
		)
		{
			output.WriteLine(string.Format(SectionHeader, samples, section.Name));
			foreach (var group in section.Groups)
			{
				WriteGroup(output, group);
			}
			output.WriteLine();
		}
	}
}