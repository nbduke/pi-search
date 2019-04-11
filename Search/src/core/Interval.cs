using Tools;

namespace PiSearch.Core
{
	public struct Interval
	{
		public readonly long StartIndex;
		public long EndIndex;

		public Interval(long start)
			: this(start, start)
		{
		}

		public Interval(long start, long end)
		{
			Validate.IsTrue(start <= end, "start cannot exceed end");
			StartIndex = start;
			EndIndex = end;
		}

		public long Length
		{
			get { return EndIndex - StartIndex; }
		}

		public override bool Equals(object obj)
		{
			return (obj is Interval other) && this == other;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int simpleHash = 17;
				simpleHash = simpleHash * 23 + StartIndex.GetHashCode();
				simpleHash = simpleHash * 23 + EndIndex.GetHashCode();
				return simpleHash;
			}
		}

		public override string ToString()
		{
			return $"[{StartIndex}, {EndIndex})";
		}

		public static bool operator==(Interval a, Interval b)
		{
			return a.StartIndex == b.StartIndex && a.EndIndex == b.EndIndex;
		}

		public static bool operator!=(Interval a, Interval b)
		{
			return !(a == b);
		}
	}
}