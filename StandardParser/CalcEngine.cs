using StandardCrawler;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace StandardParser
{
	public class CalcEngine
	{
		private List<Stock> stockList;

		public CalcEngine (List<Stock> items)
		{
			stockList = items;
		}

		public Int64 TotalMarketCap ()
		{
			Int64 mktCap = 0;
			foreach (Stock s in stockList) {
				mktCap += (Int64)s.mktCap;
			}
			return mktCap;
		}

		public Int64 TotalEarnings ()
		{
			Int64 earnings = 0;
			foreach (Stock s in stockList) {
				earnings += (Int64)(s.earningsPerShare* s.Shares);
			}
			return earnings;
		}
	}
}

