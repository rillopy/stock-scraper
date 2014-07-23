using StandardCrawler;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace StandardParser
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TextReader reader = Console.In;
			List<Stock> stocks = new List<Stock> ();
			int i = 0;
			for (i = 0; i < 500; i++) {

				string row = reader.ReadLine ();
				string[] items = row.Split(new Char[]{' '});
				Stock s = new Stock(items[0].Trim ());
				s.price = Double.Parse (items[1].Trim ());
				s.earningsPerShare = Double.Parse (items[2].Trim ());
				s.mktCap = Double.Parse (items[3].Trim ());
				s.freeFloat = Int64.Parse (items[4].Trim ());
				stocks.Add (s);
			}

			CalcEngine calcEngine = new CalcEngine(stocks);
			Console.WriteLine (string.Format ("{0,14:P3}", (double)calcEngine.TotalMarketCapFromFreeFloat() / (double)calcEngine.TotalEarningsFromFreeFloat()/100.0));
		}
	}
}
