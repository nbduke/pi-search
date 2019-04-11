using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PiSearch.Core;

namespace Test
{
	[TestClass]
	public class WordGroupUnitTests
	{
		#region Constructor
		[TestMethod]
		public void Constructor_WithAnyWordInstance_SetsCharactersToWordAndLocationToInterval()
		{
			// Arrange
			var instance = AnyFirstWord;

			// Act
			var group = new WordGroup(instance);

			// Assert
			Assert.AreEqual(instance.Word, group.Characters);
			Assert.AreEqual(instance.Location, group.Location);
		}

		[TestMethod]
		public void Constructor_WithAnyWordInstance_AddsWordAndSetsTotalLengthOfWords()
		{
			// Arrange
			var instance = AnyFirstWord;

			// Act
			var group = new WordGroup(instance);

			// Assert
			Assert.AreEqual(1, group.WordCount);
			Assert.AreEqual(instance.Word, group.Words.First());
		}
		#endregion

		#region AddWord
		[TestMethod]
		public void AddWord_WordIsPrefixOfFirstWord_AddsWordButDoesNotChangeLocationOrCharacters()
		{
			// Arrange
			var firstWord = AnyFirstWord;
			var nextWord = Prefix;
			var group = new WordGroup(firstWord);

			// Act
			group.AddWord(nextWord);

			// Assert
			var expectedWords = new string[] { "rabbit", "rabbi" };
			CollectionAssert.AreEqual(expectedWords, group.Words.ToList());
			Assert.AreEqual(firstWord.Word, group.Characters);
			Assert.AreEqual(firstWord.Location, group.Location);
		}

		[TestMethod]
		public void AddWord_WordIsSuffixOfFirstWord_AddsWordButDoesNotChangeLocationOrCharacters()
		{
			// Arrange
			var firstWord = AnyFirstWord;
			var nextWord = Suffix;
			var group = new WordGroup(firstWord);

			// Act
			group.AddWord(nextWord);

			// Assert
			var expectedWords = new string[] { "rabbit", "bit" };
			CollectionAssert.AreEqual(expectedWords, group.Words.ToList());
			Assert.AreEqual(firstWord.Word, group.Characters);
			Assert.AreEqual(firstWord.Location, group.Location);
		}

		[TestMethod]
		public void AddWord_WordOverlapsFirstWord_AddsWordAndUpdatesLocationAndCharacters()
		{
			// Arrange
			var firstWord = AnyFirstWord;
			var nextWord = Overlapping;
			var group = new WordGroup(firstWord);

			// Act
			group.AddWord(nextWord);

			// Assert
			var expectedWords = new string[] { "rabbit", "bitten" };
			var expectedLocation = new Interval(firstWord.Location.StartIndex, nextWord.Location.EndIndex);
			CollectionAssert.AreEqual(expectedWords, group.Words.ToList());
			Assert.AreEqual("rabbitten", group.Characters);
			Assert.AreEqual(expectedLocation, group.Location);
		}

		[TestMethod]
		public void AddWord_WordIsAdjacentToFirstWord_AddsWordAndUpdatesLocationAndCharacters()
		{
			// Arrange
			var firstWord = AnyFirstWord;
			var nextWord = Adjacent;
			var group = new WordGroup(firstWord);

			// Act
			group.AddWord(nextWord);

			// Assert
			var expectedWords = new string[] { "rabbit", "turtle" };
			var expectedLocation = new Interval(firstWord.Location.StartIndex, nextWord.Location.EndIndex);
			CollectionAssert.AreEqual(expectedWords, group.Words.ToList());
			Assert.AreEqual("rabbitturtle", group.Characters);
			Assert.AreEqual(expectedLocation, group.Location);
		}

		[TestMethod]
		public void AddWord_WordStartsAfterFirstWord_AddsWordAndUpdatesLocationAndCharactersWithFillers()
		{
			// Arrange
			var firstWord = AnyFirstWord;
			var nextWord = StartsAfter;
			var group = new WordGroup(firstWord);

			// Act
			group.AddWord(nextWord);

			// Assert
			var expectedWords = new string[] { "rabbit", "fox" };
			var expectedLocation = new Interval(firstWord.Location.StartIndex, nextWord.Location.EndIndex);
			CollectionAssert.AreEqual(expectedWords, group.Words.ToList());
			Assert.AreEqual("rabbit****fox", group.Characters);
			Assert.AreEqual(expectedLocation, group.Location);
		}
		#endregion

		private const long AnyIndex = 246;
		private WordInstance AnyFirstWord = new WordInstance("rabbit", AnyIndex);
		private WordInstance Prefix = new WordInstance("rabbi", AnyIndex);
		private WordInstance Suffix = new WordInstance("bit", AnyIndex + 3);
		private WordInstance Overlapping = new WordInstance("bitten", AnyIndex + 3);
		private WordInstance Adjacent = new WordInstance("turtle", AnyIndex + 6);
		private WordInstance StartsAfter = new WordInstance("fox", AnyIndex + 10);
	}
}