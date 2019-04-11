using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.DataStructures;

namespace PiSearch.Core
{
	public struct WordInstance
	{
		public string Word;
		public Interval Location;

		public WordInstance(string word, long startIndex)
		{
			Word = word;
			Location = new Interval(startIndex, startIndex + word.Length);
		}
	}

	public class WordBuilder
	{
		private readonly long StartIndex;
		private readonly List<WordInstance> Instances;
		private string Characters;
		private IPrefixTreeNode DictionaryNode;

		public WordBuilder(long startIndex, IPrefixTreeNode dictionaryNode)
		{
			Validate.IsTrue(startIndex >= 0, "startIndex must not be negative");
			Validate.IsNotNull(dictionaryNode, "dictionaryNode");

			StartIndex = startIndex;
			Instances = new List<WordInstance>();
			DictionaryNode = dictionaryNode;
			Characters = new string(dictionaryNode.Character, 1);

			if (dictionaryNode.IsEndOfWord)
				SaveInstance();
		}

		public IEnumerable<WordInstance> Words
		{
			get { return Instances; }
		}

		public bool TryExtend(char nextCharacter, long index)
		{
			if (DictionaryNode != null)
			{
				DictionaryNode = DictionaryNode.GetChild(nextCharacter);
				if (DictionaryNode != null)
				{
					Characters += nextCharacter;
					if (DictionaryNode.IsEndOfWord)
						SaveInstance();

					return true;
				}
			}

			return false;
		}

		private void SaveInstance()
		{
			Instances.Add(new WordInstance(
				Characters,
				StartIndex
			));
		}
	}
}