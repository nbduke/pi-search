using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PiSearch.Core;

namespace Test
{
	[TestClass]
	public class WordGrouperUnitTests
	{
		#region AddWord
		[TestMethod]
		public void AddWord_WordIsEmpty_NoGroupsAdded()
		{
			// Arrange
			var grouper = CreateWordGrouper();

			// Act
			grouper.AddWord(new WordInstance("", AnyIndex));

			// Assert
			Assert.AreEqual(0, FakeStore.GroupsAdded);
		}

		[TestMethod]
		public void AddWord_FirstWord_NoGroupsAdded()
		{
			// Arrange
			var grouper = CreateWordGrouper();

			// Act
			grouper.AddWord(new WordInstance(AnyWord, AnyIndex));

			// Assert
			Assert.AreEqual(0, FakeStore.GroupsAdded);
		}

		[TestMethod]
		public void AddWord_WordOverlapsFirstWord_NoGroupsAdded()
		{
			// Arrange
			var grouper = CreateWordGrouper();
			var firstWord = new WordInstance("dinner", AnyIndex);
			var secondWord = new WordInstance("nerd", AnyIndex + 3);

			// Act
			grouper.AddWord(firstWord);
			grouper.AddWord(secondWord);

			// Assert
			Assert.AreEqual(0, FakeStore.GroupsAdded);
		}

		[TestMethod]
		public void AddWord_WordIsWithinGroupSeparationOfFirstWord_NoGroupsAdded()
		{
			// Arrange
			var grouper = CreateWordGrouper();
			var firstWord = new WordInstance(AnyWord, AnyIndex);
			var secondWord = new WordInstance(AnyWord, firstWord.Location.EndIndex + AnyGroupSeparation);

			// Act
			grouper.AddWord(firstWord);
			grouper.AddWord(secondWord);

			// Assert
			Assert.AreEqual(0, FakeStore.GroupsAdded);
		}

		[TestMethod]
		public void AddWord_WordIsNotWithinGroupSeparationOfFirstWord_AddsOneGroup()
		{
			// Arrange
			var grouper = CreateWordGrouper();
			var firstWord = new WordInstance(AnyWord, AnyIndex);
			var secondWord = new WordInstance("another", firstWord.Location.EndIndex + AnyGroupSeparation + 1);

			// Act
			grouper.AddWord(firstWord);
			grouper.AddWord(secondWord);

			// Assert
			Assert.AreEqual(1, FakeStore.GroupsAdded);
			Assert.AreEqual(new WordGroup(firstWord), FakeStore.Groups[0]);
			Assert.AreEqual(1, grouper.GroupsCreated);
		}
		#endregion

		#region CloseLastGroup
		[TestMethod]
		public void CloseLastGroup_NoWordsHaveBeenAdded_NoGroupsAdded()
		{
			// Arrange
			var grouper = CreateWordGrouper();

			// Act
			grouper.CloseLastGroup();

			// Assert
			Assert.AreEqual(0, FakeStore.GroupsAdded);
		}

		[TestMethod]
		public void CloseLastGroup_WordHasBeenAdded_AddsOneGroup()
		{
			// Arrange
			var grouper = CreateWordGrouper();
			var firstWord = new WordInstance(AnyWord, AnyIndex);
			grouper.AddWord(firstWord);

			var expectedGroup = new WordGroup(firstWord);

			// Act
			grouper.CloseLastGroup();

			// Assert
			Assert.AreEqual(1, FakeStore.GroupsAdded);
			Assert.AreEqual(expectedGroup, FakeStore.Groups[0]);
			Assert.AreEqual(1, grouper.GroupsCreated);
		}
		#endregion

		#region Test several words
		[TestMethod]
		public void TestSeveralWords()
		{
			// Arrange
			var grouper = CreateWordGrouper(0);
			var startOfFirstGroup = new WordInstance("cat", AnyIndex);
			var overlappingFirst = new WordInstance("attest", startOfFirstGroup.Location.EndIndex - 2);
			var startOfSecondGroup = new WordInstance("fixate", AnyIndex + 100);
			var prefixOfSecondGroup = new WordInstance("fix", startOfSecondGroup.Location.StartIndex);
			var suffixOfSecondGroup = new WordInstance("ate", startOfSecondGroup.Location.StartIndex + 3);
			var adjacentToSecondGroup = new WordInstance("bar", startOfSecondGroup.Location.EndIndex);
			var startOfThirdGroup = new WordInstance("butternut", AnyIndex + 200);

			// Act
			grouper.AddWord(startOfFirstGroup);
			grouper.AddWord(overlappingFirst);
			grouper.AddWord(startOfSecondGroup);
			grouper.AddWord(prefixOfSecondGroup);
			grouper.AddWord(suffixOfSecondGroup);
			grouper.AddWord(adjacentToSecondGroup);
			grouper.AddWord(startOfThirdGroup);
			grouper.CloseLastGroup();

			// Assert
			var expectedGroups = new WordGroup[]
			{
				new WordGroup(new WordInstance("cattest", startOfFirstGroup.Location.StartIndex)),
				new WordGroup(new WordInstance("fixatebar", startOfSecondGroup.Location.StartIndex)),
				new WordGroup(new WordInstance("butternut", startOfThirdGroup.Location.StartIndex))
			};
			CollectionAssert.AreEqual(expectedGroups, FakeStore.Groups);
			Assert.AreEqual(3, grouper.GroupsCreated);
		}
		#endregion

		#region Helpers
		private const string AnyWord = "dinner";
		private const long AnyIndex = 1029;
		private const int AnyGroupSeparation = 3;
		private FakeGroupStore FakeStore = new FakeGroupStore();

		private WordGrouper CreateWordGrouper(int groupSeparation = AnyGroupSeparation)
		{
			return new WordGrouper(groupSeparation, FakeStore);
		}
		#endregion
	}
}