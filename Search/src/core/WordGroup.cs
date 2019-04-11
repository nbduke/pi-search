using System.Collections.Generic;

namespace PiSearch.Core
{
	public class WordGroup
	{
		private Interval _Location;
		private readonly List<string> _Words = new List<string>();

		public string Characters { get; private set; }

		public Interval Location
		{
			get { return _Location; }
		}

		public long Length
		{
			get { return _Location.Length; }
		}

		public IEnumerable<string> Words
		{
			get { return _Words; }
		}

		public int WordCount
		{
			get { return _Words.Count; }
		}

		public WordGroup(WordInstance firstWord)
		{
			Characters = firstWord.Word;
			_Location = firstWord.Location;
			_Words.Add(Characters);
		}

		public void AddWord(WordInstance instance)
		{
			long startIndex = instance.Location.StartIndex;
			long endIndex = instance.Location.EndIndex;
			string word = instance.Word;

			if (endIndex > _Location.EndIndex)
			{
				int overlap = (int)(_Location.EndIndex - startIndex);
				if (overlap >= 0)
					Characters += word.Substring(overlap);
				else
					Characters += new string('*', -overlap) + word;

				_Location.EndIndex = endIndex;
			}

			_Words.Add(word);
		}

		public override bool Equals(object obj)
		{
			return (obj is WordGroup other) && Location.Equals(other.Location);
		}

		public override int GetHashCode()
		{
			return Location.GetHashCode();
		}
	}
}