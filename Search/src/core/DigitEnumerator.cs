using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PiSearch.Core
{
	public class DigitEnumerator : IEnumerable<char>
	{
		private Stream DigitStream;

		public DigitEnumerator(Stream digitStream)
		{
			DigitStream = digitStream;
		}

		public IEnumerator<char> GetEnumerator()
		{
			DigitStream.Seek(0, SeekOrigin.Begin);
			int nextDigit = DigitStream.ReadByte();

			while (nextDigit != -1)
			{
				yield return (char)nextDigit;
				nextDigit = DigitStream.ReadByte();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}