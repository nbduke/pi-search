using System.Collections.Generic;
using System.Linq;

using PiSearch.Core;
using Tools.DataStructures;

namespace PiSearch.Scan
{
	struct GroupStats
	{
		public WordGroup Group;
		public double AverageWordLength;
		public int MaxWordLength;
	}

	/// <summary>
	/// A named list of word groups sorted according to some metric.
	/// </summary>
	public class SortedGroupList
	{
		public readonly string Name;
		public readonly IEnumerable<WordGroup> Groups;

		public SortedGroupList(string name, IEnumerable<WordGroup> groups)
		{
			Name = name;
			Groups = groups;
		}
	}

	/// <summary>
	/// Sorts word groups along the metrics of length, word count,
	/// average word length, and maximum word length.
	/// </summary>
	public class SortedGroupStore : IGroupStore
	{
		public int TotalGroupsAdded { get; private set; } = 0;
		public long TotalLengthOfGroups { get; private set; } = 0;
		public long TotalWordsWithinGroups { get; private set; } = 0;
		public readonly int Samples;

		private readonly int MinGroupLength;
		private readonly Heap<WordGroup> _Longest;
		private readonly Heap<WordGroup> _MostWords;
		private readonly Heap<GroupStats> _LongestAverageWords;
		private readonly Heap<GroupStats> _LongestMaxWords;

		public double AverageGroupLength
		{
			get { return TotalLengthOfGroups / (double)TotalGroupsAdded; }
		}

		public double AverageWordsPerGroup
		{
			get { return TotalWordsWithinGroups / (double)TotalGroupsAdded; }
		}

		public SortedGroupList Longest
		{
			get
			{
				return new SortedGroupList(
					"Longest Groups",
					_Longest.OrderByDescending(g => g.Length)
				);
			}
		}

		public SortedGroupList MostWords
		{
			get
			{
				return new SortedGroupList(
					"Groups with Most Words",
					_MostWords.OrderByDescending(g => g.WordCount)
				);
			}
		}

		public SortedGroupList LongestAverageWords
		{
			get
			{
				return new SortedGroupList(
					"Groups with Longest Words on Average",
					_LongestAverageWords.OrderByDescending(g => g.AverageWordLength)
					.Select(g => g.Group)
				);
			}
		}

		public SortedGroupList LongestMaxWords
		{
			get
			{
				return new SortedGroupList(
					"Groups with Longest Overall Words",
					_LongestMaxWords.OrderByDescending(g => g.MaxWordLength)
					.Select(g => g.Group)
				);
			}
		}

		/// <summary>
		/// Creates a SortedGroupStore.
		/// </summary>
		/// <param name="n">the number of groups to keep at the top and bottom of each
		/// sorted list</param>
		/// <param name="minGroupLength">the minimum length a group must have to be kept</param>
		public SortedGroupStore(int n, int minGroupLength = 1)
		{
			Samples = n;
			MinGroupLength = minGroupLength;
			_Longest = new Heap<WordGroup>(g => g.Length, n);
			_MostWords = new Heap<WordGroup>(g => g.WordCount, n);
			_LongestAverageWords = new Heap<GroupStats>(g => g.AverageWordLength, n);
			_LongestMaxWords = new Heap<GroupStats>(g => g.MaxWordLength, n);
		}

		public void AddGroup(WordGroup group)
		{
			if (group.Length >= MinGroupLength)
			{
				++TotalGroupsAdded;
				TotalLengthOfGroups += group.Length;
				TotalWordsWithinGroups += group.WordCount;

				GroupStats stats = new GroupStats()
				{
					Group = group,
					AverageWordLength = group.Words.Sum(w => w.Length) / (double)group.WordCount,
					MaxWordLength = group.Words.Max(w => w.Length)
				};
				_LongestAverageWords.Push(stats);
				_LongestMaxWords.Push(stats);
				_Longest.Push(group);
				_MostWords.Push(group);
			}
		}
	}
}