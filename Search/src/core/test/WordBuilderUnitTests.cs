using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PiSearch.Core;

namespace Test
{
	[TestClass]
	public class WordBuilderUnitTests
	{
		#region TryExtend
		[TestMethod]
		public void TryExtend_WithNextCharacterOnPath_ReturnsTrue()
		{
			// Arrange
			string path = "hello";
			var builder = CreateBuilderWith(path);

			// Act
			bool result = builder.TryExtend('e', AnyIndex);

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void TryExtend_WithCharacterNotOnPath_ReturnsFalse()
		{
			// Arrange
			string path = "hello";
			var builder = CreateBuilderWith(path);

			// Act
			bool result = builder.TryExtend('x', AnyIndex);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TryExtend_WithDifferentCharacterOnPath_ReturnsFalse()
		{
			// Arrange
			string path = "hello";
			var builder = CreateBuilderWith(path);

			// Act
			bool result = builder.TryExtend('o', AnyIndex);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TryExtend_AnyCharacterAfterFailedAttempt_ReturnsFalse()
		{
			// Arrange
			string path = "goodbye";
			var builder = CreateBuilderWith(path);

			// Act
			builder.TryExtend('y', AnyIndex);
			bool secondTry = builder.TryExtend('o', AnyIndex);

			// Assert
			Assert.IsFalse(secondTry);
		}
		#endregion

		#region Words
		[TestMethod]
		public void Words_BuilderConstructedWithValidWord_ReturnsWordGivenToConstructor()
		{
			// Arrange
			long startIndex = AnyIndex;
			var builder = new WordBuilder(startIndex, new FakePrefixTreeNode("I", 'I'));

			// Act
			var words = builder.Words.ToList();

			// Assert
			var expectedWord = new WordInstance("I", startIndex);
			Assert.AreEqual(1, words.Count);
			Assert.AreEqual(expectedWord, words[0]);
		}

		[TestMethod]
		public void Words_BuilderWasNeverExtended_ReturnsEmptyCollection()
		{
			// Arrange
			var builder = CreateBuilderWith("anypath");

			// Act
			var words = builder.Words;

			// Assert
			Assert.AreEqual(0, words.Count());
		}

		[TestMethod]
		public void Words_BuilderWasExtendedButNoWordsFound_ReturnsEmptyCollection()
		{
			// Arrange
			string path = "abcdefg";
			var builder = CreateBuilderWith(path);
			Extend(builder, path, AnyIndex, 5);

			// Act
			var words = builder.Words;

			// Assert
			Assert.AreEqual(0, words.Count());
		}

		[TestMethod]
		public void Words_EveryCharacterOnPathEndsAWord_ReturnsAllSuffixesOfPath()
		{
			// Arrange
			string path = "lmnop";
			long startIndex = AnyIndex;
			var builder = CreateBuilderWith(path, startIndex, path.ToCharArray());
			Extend(builder, path, startIndex, path.Length);

			// Act
			var words = builder.Words.ToList();

			// Assert
			WordInstance[] expectedWords = new WordInstance[]
			{
				new WordInstance("l", startIndex),
				new WordInstance("lm", startIndex),
				new WordInstance("lmn", startIndex),
				new WordInstance("lmno", startIndex),
				new WordInstance("lmnop", startIndex)
			};
			CollectionAssert.AreEqual(expectedWords, words);
		}

		[TestMethod]
		public void Words_PathContainsSomeWordsAndBuilderTraversesWholePath_ReturnsAllWords()
		{
			// Arrange
			string path = "fathers";
			long startIndex = AnyIndex;
			var builder = CreateBuilderWith(path, startIndex, 't', 'r', 's');
			Extend(builder, path, startIndex, path.Length);

			// Act
			var words = builder.Words.ToList();

			// Assert
			WordInstance[] expectedWords = new WordInstance[]
			{
				new WordInstance("fat", startIndex),
				new WordInstance("father", startIndex),
				new WordInstance("fathers", startIndex)
			};
			CollectionAssert.AreEqual(expectedWords, words);
		}

		[TestMethod]
		public void Words_PathContainsSomeWordsAndBuilderTraversesSomeOfPath_ReturnsAllWordsFound()
		{
			// Arrange
			string path = "howdy";
			long startIndex = AnyIndex;
			var builder = CreateBuilderWith(path, startIndex, 'w', 'y');
			Extend(builder, path, startIndex, 3);
			builder.TryExtend('a', AnyIndex);

			// Act
			var words = builder.Words.ToList();

			// Assert
			var expectedWord = new WordInstance("how", startIndex);
			Assert.AreEqual(1, words.Count);
			Assert.AreEqual(expectedWord, words[0]);
		}
		#endregion

		private const long AnyIndex = 77;

		private WordBuilder CreateBuilderWith(
			string characterPath,
			long startIndex = 0,
			params char[] wordTerminators
		)
		{
			return new WordBuilder(
				startIndex,
				new FakePrefixTreeNode(characterPath, wordTerminators)
			);
		}

		private void Extend(
			WordBuilder builder,
			string characterPath,
			long startIndex,
			int times
		)
		{
			for (int i = 1; i <= times && i < characterPath.Length; ++i)
			{
				builder.TryExtend(characterPath[i], startIndex + i);
			}
		}
	}
}
