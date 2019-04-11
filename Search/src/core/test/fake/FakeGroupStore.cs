using System.Collections.Generic;

using PiSearch.Core;

namespace Test
{
	class FakeGroupStore : IGroupStore
	{
		public List<WordGroup> Groups = new List<WordGroup>();

		public int GroupsAdded
		{
			get { return Groups.Count; }
		}

		public void AddGroup(WordGroup group)
		{
			Groups.Add(group);
		}
	}
}