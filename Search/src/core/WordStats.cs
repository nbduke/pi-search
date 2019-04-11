using System.Collections.Generic;
using System.Linq;

using Tools.DataStructures;

namespace PiSearch.Core
{
	public class WordStats
	{
		public const int Samples = 10;

		public readonly Counter<string> Histogram;
		public readonly int TotalWordsFound;
		public readonly int TotalLengthOfWords;
		public readonly IEnumerable<string> Longest;
		public readonly IEnumerable<string> Shortest;
		public readonly IEnumerable<string> MostCommon;
		public readonly IEnumerable<string> LeastCommon;

		public int UniqueWordsFound
		{
			get { return Histogram.UniqueCount; }
		}

		public double AverageWordLength
		{
			get { return TotalLengthOfWords / (double)UniqueWordsFound; }
		}

		public double AverageWordFrequency
		{
			get { return TotalWordsFound / (double)UniqueWordsFound; }
		}

		internal WordStats(Counter<string> allWords)
		{
			Histogram = allWords;
			Longest = allWords
				.Select(p => p.Key)
				.OrderByDescending(w => w.Length)
				.Take(Samples);
			Shortest = allWords
				.Select(p => p.Key)
				.OrderBy(w => w.Length)
				.Take(Samples);
			MostCommon = allWords
				.OrderByDescending(pair => pair.Value)
				.Select(p => p.Key)
				.Take(Samples);
			LeastCommon = allWords
				.OrderBy(pair => pair.Value)
				.ThenByDescending(pair => pair.Key.Length)
				.Select(p => p.Key)
				.Take(Samples);
			TotalWordsFound = allWords.Sum;
			TotalLengthOfWords = allWords.Sum(pair => pair.Key.Length);
		}
	}
}