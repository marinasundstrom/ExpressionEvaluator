using System;
using System.IO;
using System.Text;

namespace ExpressionEvaluator
{
	public static class Helpers
	{
		public static TextReader TextReaderFromString(string text)
		{
			var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text), false);
			memoryStream.Seek(0, SeekOrigin.Begin);
			return new StreamReader(memoryStream);
		}
	}
}
