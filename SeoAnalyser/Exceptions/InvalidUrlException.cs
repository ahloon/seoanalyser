using System;

namespace SeoAnalyser.Exceptions
{
	public class InvalidUrlException : Exception
	{
		public InvalidUrlException()
		{

		}

		public InvalidUrlException(string url)
			: base(String.Format($"Invalid URL: {url}"))
		{

		}
	}
}
