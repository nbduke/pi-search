using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tools.DataStructures;

namespace Test
{
	class FakePrefixTreeNode : IPrefixTreeNode
	{
		public readonly string CharacterPath;
		private char[] WordTerminators;

		public FakePrefixTreeNode(string characterPath, params char[] wordTerminators)
		{
			CharacterPath = characterPath;
			WordTerminators = wordTerminators;
		}

		public IEnumerable<IPrefixTreeNode> Children
		{
			get
			{
				if (IsLeaf)
					yield break;
				else
					yield return new FakePrefixTreeNode(
						CharacterPath.Substring(1),
						WordTerminators
					);
			}
		}

		public bool IsLeaf
		{
			get { return CharacterPath.Length <= 1; }
		}

		public char Character
		{
			get { return CharacterPath[0]; }
		}

		public bool IsEndOfWord
		{
			get { return WordTerminators.Contains(Character); }
		}

		public IPrefixTreeNode GetDescendant(string s)
		{
			IPrefixTreeNode descendant = GetChild(s[0]);
			foreach (char c in s.Skip(1))
			{
				if (descendant != null)
					descendant = descendant.GetChild(c);
				else
					break;
			}

			return descendant;
		}

		public IPrefixTreeNode GetChild(char c)
		{
			var child = Children.FirstOrDefault();
			if (child != null && child.Character == c)
				return child;
			else
				return null;
		}

		public void Accept(IVisitor<IPrefixTreeNode> visitor)
		{
			throw new NotImplementedException();
		}
	}
}