using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace StandardCrawler
{
	public class YahooStock : Stock
	{

		// remove all /tbody and unindexed tr tds from browser generated xpaths
		private static string finWebUrl = "http://finance.yahoo.com/q/ks?s=";
		private static string xPath_Price = "//*[@id=\"yfi_rt_quote_summary\"]/div[2]/div[1]/span[1]";
		private static string xPath_MktCap = "/html/body/div[4]/div[4]/table[2]/tr[2]/td[1]/table[2]/tr/td/table/tr[1]/td[2]";
		private static string xPath_EPS = "//*[@id=\"yfncsumtab\"]/tr[2]/td[1]/table[7]/tr[1]/td[1]/table/tr[8]/td[2]";
		private static string xPath_FreeFloat = "//*[@id=\"yfncsumtab\"]/tr[2]/td[3]/table[3]/tr[1]/td[1]/table/tr[5]/td[2]";

		public YahooStock (string ticker) : base(ticker)
		{
			this.ticker = ticker.Replace('.','-');
		}

		public override void Refresh ()
		{
			WebClient client = new WebClient ();
			
			client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
			string s = string.Empty;

			int i = 0;
			do {
				try {
					i++;
					if (ticker.Contains (":")) {
						ticker = ticker.Substring (0, ticker.IndexOf (':'));
					}
					Stream page = client.OpenRead (string.Format ("{0}{1}{2}", finWebUrl, ticker, "+Key+Statistics"));
					StreamReader sr = new StreamReader (page);
					
					s = sr.ReadToEnd ();
					page.Close ();
				} catch (WebException exp) {
					Console.Error.WriteLine (exp.Message);
					Console.Error.WriteLine ("Trying again...");
				}
			} while ( s == string.Empty && i < 15);

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

