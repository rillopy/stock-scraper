using System;

namespace StandardCrawler
{
	public class WrongExchangeException : Exception
	{
		public WrongExchangeException (string message) : base(message)
		{
		}
	}
}

