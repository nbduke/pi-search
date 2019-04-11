using Tools;

namespace PiSearch.Core
{
	public class WordGrouper
	{
		private readonly long GroupSeparation;
		private readonly IGroupStore GroupStore;
		private WordGroup CurrentGroup;

		public int GroupsCreated { get; private set; } = 0;

		public WordGrouper(
			long groupSeparation,
			IGroupStore groupStore
		)
		{
			Validate.IsNotNull(groupStore, "groupStore");
			GroupSeparation = groupSeparation;
			GroupStore = groupStore;
		}

		public void AddWord(WordInstance word)
		{
			if (word.Word.Length == 0)
			{
				return;
			}
			else if (CurrentGroup == null)
			{
				CurrentGroup = new WordGroup(word);
			}
			else if (word.Location.StartIndex >= CurrentGroup.Location.StartIndex)
			{
				if (word.Location.StartIndex <= CurrentGroup.Location.EndIndex + GroupSeparation)
				{
					CurrentGroup.AddWord(word); // extend the current group
				}
				else
				{
					CloseLastGroup();
					CurrentGroup = new WordGroup(word);
				}
			}
		}

		public void CloseLastGroup()
		{
			if (CurrentGroup != null)
			{
				GroupStore.AddGroup(CurrentGroup);
				++GroupsCreated;
			}
		}
	}
}