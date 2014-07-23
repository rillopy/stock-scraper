using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace StandardCrawler
{
	public class ReutersStock : Stock
	{
		private static string finWebUrl = "http://www.reuters.com/finance/stocks/overview?symbol=";
		private static string xPath_Price = "//*[@id=\"headerQuoteContainer\"]/div[1]/div/span[2]/text()";
		private static string xPath_MktCap = "//*[@id=\"overallRatios\"]/div/div[2]/table/tbody/tr[2]/td[2]/strong/text()";
		private static string xPath_EPS = "//*[@id=\"companyVsIndustry\"]/div/div[2]/table/tbody/tr[3]/td[2]/text()";
		private static string xPath_FreeFloat = "//*[@id=\"wsod\"]/div[3]/div/div[2]/div[1]/div[2]/table[2]/tbody/tr[3]/td";
		private static string xPath_OutstandingShares = "//*[@id=\"wsod\"]/div[3]/div/div[2]/div[1]/div[2]/table[2]/tbody/tr[2]/td";
		
		public ReutersStock (string ticker) : base(ticker)
		{
		}
		
		public virtual void Refresh ()
		{
			WebClient client = new WebClient ();
			
			client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
			string s = string.Empty;
			
			int i = 0;
			do {
				try {
					i++;
					Stream page = client.OpenRead (string.Format ("{0}{1}", finWebUrl, ticker));
					StreamReader sr = new StreamReader (page);
					
					s = sr.ReadToEnd ();
					page.Close ();
				} catch (WebException exp) {
					Console.Error.WriteLine (exp.Message);
					Console.Error.WriteLine (string.Format ("Trying again... {0} {1}", i, ticker));
				}
			} while ( s == string.Empty && i < 15);
			
			if (s.IndexOf ("NYQ") == -1 && s.IndexOf ("NSQ") == -1) {
				throw new WrongExchangeException (string.Format ("Wrong exchange for {0}", ticker));
			}
			
			HtmlDocument doc = new HtmlDocument ();
			doc.LoadHtml (s);
			
			// stock price
			foreach (HtmlNode node in doc.DocumentNode.SelectNodes(xPath_Price)) {
				Double.TryParse (node.InnerText, out price);
			}
			
			// stock earnings per share
			foreach (HtmlNode node in doc.DocumentNode.SelectNodes(xPath_EPS)) {
				Double.TryParse (node.InnerText, out earningsPerShare);
			}
			
			// market cap
			foreach (HtmlNode node in doc.DocumentNode.SelectNodes(xPath_MktCap)) {
				mktCap = parseHumanNumber (node.InnerText);
			}
			
			// free float
			foreach (HtmlNode node in doc.DocumentNode.SelectNodes(xPath_FreeFloat)) {
				freeFloat = (Int64)(parseHumanNumber (node.InnerText));
			}
		}
	}
}

