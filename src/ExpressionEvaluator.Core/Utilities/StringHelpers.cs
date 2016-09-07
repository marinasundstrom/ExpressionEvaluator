using System;
using System.IO;
using System.Text;

namespace ExpressionEvaluator.Utilites
{
	public static class StringHelpers
	{
		public static TextReader TextReaderFromString(string text)
		{
			var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text), false);
			memoryStream.Seek(0, SeekOrigin.Begin);
			return new StreamReader(memoryStream);
		}
	}
}
