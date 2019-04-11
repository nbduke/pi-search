using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
using Tools.DataStructures;

namespace PiSearch.Core
{
	public struct ScanResults
	{
		public long DigitsRead;
		public WordStats WordStats;
	}

	public struct ProgressReport
	{
		public long DigitsRead;
		public int UniqueWordsFound;
		public int WordGroupsCreated;
	}

	public delegate void ProgressHandler(ProgressReport report);

	public class PiScanner
	{
		private PrefixTreeDictionary Dictionary;
		private WordGrouper WordGrouper;
		private Queue<WordBuilder> WordBuilders;
		private Counter<string> Words;

		public event ProgressHandler ProgressEvent = delegate { };

		public bool IsScanning { get; private set; }

		public PiScanner()
		{
			IsScanning = false;
			WordBuilders = new Queue<WordBuilder>();
			Words = new Counter<string>();
		}

		/// <summary>
		/// Scans the digits of pi identifying words and building word groups.
		/// </summary>
		/// <param name="digits">the stream of digits</param>
		/// <param name="dictionary">the dictionary of words to match against</param>
		/// <param name="groupStore">the location where word groups should be recorded</param>
		/// <param name="groupSeparation">the number of digits allowed between words of the same group</param>
		/// <param name="progressInterval">the number of digits to scan between progress reports</param>
		/// <param name="maxDigits">the maximum number of digits to scan (no limit by default)</param>
		/// <returns>a ScanResult object</returns>
		public Task<ScanResults> Scan(
			IEnumerable<char> digits,
			PrefixTreeDictionary dictionary,
			IGroupStore groupStore,
			long groupSeparation,
			long progressInterval = 10000,
			long? maxDigits = null
		)
		{
			Validate.IsNotNull(digits, "digits");
			Validate.IsNotNull(dictionary, "dictionary");

			Dictionary = dictionary;
			WordGrouper = new WordGrouper(groupSeparation, groupStore);
			WordBuilders.Clear();
			Words.Clear();

			return Task.Run(() =>
			{
				IsScanning = true;
				long digitIndex = 0;

				foreach (char digit in digits)
				{
					ExtendBuilders(digit, digitIndex);

					var dictionaryNode = Dictionary.FindNode(new string(digit, 1));
					if (dictionaryNode != null)
						WordBuilders.Enqueue(new WordBuilder(digitIndex, dictionaryNode));

					++digitIndex;

					CheckProgress(digitIndex, progressInterval);

					if (maxDigits.HasValue && digitIndex >= maxDigits)
						break;
				}

				ProcessRemainingBuilders();
				IsScanning = false;

				return new ScanResults() {
					DigitsRead = digitIndex,
					WordStats = new WordStats(Words)
				};
			});
		}

		private void ExtendBuilders(char digit, long digitIndex)
		{
			int numberToPop = 0;
			bool canPop = true;

			foreach (var builder in WordBuilders)
			{
				if (builder.TryExtend(digit, digitIndex))
					canPop = false;
				else if (canPop)
					++numberToPop;
			}

			for (int i = 0; i < numberToPop; ++i)
			{
				ProcessBuilder(WordBuilders.Dequeue());
			}
		}

		private void CheckProgress(long digitIndex, long progressInterval)
		{
			if (progressInterval > 0 && digitIndex % progressInterval == 0)
			{
				ProgressEvent(new ProgressReport()
				{
					DigitsRead = digitIndex,
					UniqueWordsFound = Words.UniqueCount,
					WordGroupsCreated = WordGrouper.GroupsCreated
				});
			}
		}

		private void ProcessRemainingBuilders()
		{
			foreach (var builder in WordBuilders)
			{
				ProcessBuilder(builder);
			}

			WordGrouper.CloseLastGroup();
		}

		private void ProcessBuilder(WordBuilder builder)
		{
			foreach (WordInstance word in builder.Words)
			{
				Words.Increment(word.Word);
				WordGrouper.AddWord(word);
			}
		}
	}
}