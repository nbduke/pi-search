using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PiSearch.Core;
using Tools.DataStructures;

namespace Test
{
	[TestClass]
	public class PiScannerComponentTests
	{
		[TestInitialize]
		public void BeforeEach()
		{
			Scanner = new PiScanner();
			FakeStore = new FakeGroupStore();
		}

		[TestMethod]
		public async Task NestedAndOverlappingWords()
		{
			// Arrange
			string digits = "fatherbald";
			var expectedWords = new string[]
			{
				"fat", "at", "the", "he", "her", "father", "herb", "herbal", "bald"
			};
			var expectedGroup = new WordGroup(new WordInstance(digits, 0));

			// Act
			var result = await Scanner.Scan(
				digits,
				CreateDictionary(expectedWords),
				FakeStore,
				AnyGroupSeparation
			);

			// Assert
			foreach (string word in expectedWords)
			{
				Assert.AreEqual(1, result.WordStats.Histogram[word]);
			}
			Assert.AreEqual(1, FakeStore.GroupsAdded);
			Assert.AreEqual(expectedGroup, FakeStore.Groups[0]);
			Assert.AreEqual(expectedWords.Length, FakeStore.Groups[0].WordCount);
		}

		[TestMethod]
		public async Task ThreeWordsInTwoGroups()
		{
			// Arrange
			string digits = "qtcatxdogpplhxybirdbnv";
			var expectedWords = new string[]
			{
				"cat", "dog", "bird"
			};
			var expectedGroups = new WordGroup[]
			{
				new WordGroup(new WordInstance("cat*dog", 2)),
				new WordGroup(new WordInstance("bird", 15))
			};

			// Act
			var result = await Scanner.Scan(
				digits,
				CreateDictionary(expectedWords),
				FakeStore,
				2
			);

			// Assert
			foreach (string word in expectedWords)
			{
				Assert.AreEqual(1, result.WordStats.Histogram[word]);
			}
			CollectionAssert.AreEqual(expectedGroups, FakeStore.Groups);
			Assert.AreEqual(2, FakeStore.Groups[0].WordCount);
		}

		[TestMethod]
		public async Task WordDuplicates()
		{
			// Arrange
			string digits = "yzhatbathatpcatsathat";
			var expectedWords = new string[]
			{
				"hat", "at", "bat", "bath", "that", "cat", "cats", "sat"
			};
			var expectedFrequencies = new int[]
			{
				3,      6,   1,      1,      2,      1,     1,      1
			};
			var expectedGroups = new WordGroup[]
			{
				new WordGroup(new WordInstance("hatbathat", 2)),
				new WordGroup(new WordInstance("catsathat", 12))
			};

			// Act
			var result = await Scanner.Scan(
				digits,
				CreateDictionary(expectedWords),
				FakeStore,
				AnyGroupSeparation
			);

			// Assert
			for (int i = 0; i < expectedWords.Length; ++i)
			{
				Assert.AreEqual(expectedFrequencies[i], result.WordStats.Histogram[expectedWords[i]]);
			}
			CollectionAssert.AreEqual(expectedGroups, FakeStore.Groups);
			Assert.AreEqual(expectedFrequencies.Sum(), FakeStore.Groups.Select(g => g.WordCount).Sum());
		}

		[TestMethod]
		public async Task PseudoRandomDigits()
		{
			// Arrange
			string digits = "mzxcvbsadfhkwrtouidybhpkzvxbaewrmbtsdfhg";
			var expectedWords = new string[]
			{
				"sad", "to", "bae"
			};
			var expectedGroups = new WordGroup[]
			{
				new WordGroup(new WordInstance("sad", 6)),
				new WordGroup(new WordInstance("to", 14)),
				new WordGroup(new WordInstance("bae", 27))
			};

			// Act
			var result = await Scanner.Scan(
				digits,
				CreateDictionary(expectedWords),
				FakeStore,
				AnyGroupSeparation
			);

			// Assert
			foreach (string word in expectedWords)
			{
				Assert.AreEqual(1, result.WordStats.Histogram[word]);
			}
			CollectionAssert.AreEqual(expectedGroups, FakeStore.Groups);
		}

		[TestMethod]
		public async Task TestProgressInterval()
		{
			var digits = "boyawnxonrvklifeq";
			var words = new string[]
			{
				"boy", "yaw", "yawn", "on", "life"
			};
			var expectedReports = new ProgressReport[]
			{
				new ProgressReport(){ DigitsRead = 4, UniqueWordsFound = 1, WordGroupsCreated = 0 },
				new ProgressReport(){ DigitsRead = 8, UniqueWordsFound = 3, WordGroupsCreated = 0 },
				new ProgressReport(){ DigitsRead = 12, UniqueWordsFound = 4, WordGroupsCreated = 1 },
				new ProgressReport(){ DigitsRead = 16, UniqueWordsFound = 4, WordGroupsCreated = 1 }
			};
			var expectedGroups = new WordGroup[]
			{
				new WordGroup(new WordInstance("boyawn", 0)),
				new WordGroup(new WordInstance("on", 7)),
				new WordGroup(new WordInstance("life", 12))
			};

			var progressReports = new List<ProgressReport>();
			Scanner.ProgressEvent += report => progressReports.Add(report);

			// Act
			var result = await Scanner.Scan(
				digits,
				CreateDictionary(words),
				FakeStore,
				AnyGroupSeparation,
				4
			);

			// Assert
			CollectionAssert.AreEqual(expectedReports, progressReports);
			foreach (var word in words)
			{
				Assert.AreEqual(1, result.WordStats.Histogram[word]);
			}
			CollectionAssert.AreEqual(expectedGroups, FakeStore.Groups);
		}

		private const long AnyGroupSeparation = 0;
		private PiScanner Scanner;
		private FakeGroupStore FakeStore;

		private PrefixTreeDictionary CreateDictionary(IEnumerable<string> words)
		{
			var dictionary = new PrefixTreeDictionary();
			foreach (string word in words)
			{
				dictionary.Add(word);
			}

			// Add some extra words
			dictionary.Add("test");
			dictionary.Add("foo");
			dictionary.Add("aaa");

			return dictionary;
		}
	}
}