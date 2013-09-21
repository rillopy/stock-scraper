using System;
using System.Data.Linq;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace StandardCrawler
{
	public class Stock
	{
		private static string finWebUrl = "http://markets.ft.com/research/Markets/Tearsheets/Summary?s=";
		private static string xPath_Price = "//*[@id=\"wsod\"]/div[2]/div/div[1]/div[1]/table/tbody/tr[1]/td[1]/span";
		private static string xPath_MktCap = "//*[@id=\"wsod\"]/div[3]/div/div[2]/div[1]/div[2]/table[2]/tbody/tr[5]/td/text()";
		private static string xPath_EPS = "//*[@id=\"wsod\"]/div[3]/div/div[2]/div[1]/div[2]/table[2]/tbody/tr[6]/td/text()";
		private static string xPath_FreeFloat = "//*[@id=\"wsod\"]/div[3]/div/div[2]/div[1]/div[2]/table[2]/tbody/tr[3]/td";
		private static string xPath_OutstandingShares = "//*[@id=\"wsod\"]/div[3]/div/div[2]/div[1]/div[2]/table[2]/tbody/tr[2]/td";
		public string ticker;
		public double mktCap;
		public double earningsPerShare;
		public double price;
		public Int64 freeFloat;
		public int outstandingShares;
		public Int64 shares;

		public Stock (string ticker)
		{
			this.ticker = ticker;
			mktCap = 0.0;
			earningsPerShare = 0.0;
			price = 0.0;
			freeFloat = 0;
			outstandingShares = 0;
		}

		public Int64 Shares {
			get {
				return (Int64)(mktCap / price);
			}
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
				throw new Exception (string.Format ("Wrong exchange for {0}", ticker));
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

		/// <summary>
		/// Parses the human-readble version of a number.
		/// </summary>
		/// <returns>
		/// The number.
		/// </returns>
		/// <param name='numberWithUnits'>
		/// Number with m or mn or bn units
		/// </param>
		protected double parseHumanNumber(string numberWithUnits)
		{
			double retVal =0.0;
			numberWithUnits = numberWithUnits.Trim ().Replace ("$","");
			if (!Double.TryParse (numberWithUnits, out retVal)) {
				if (numberWithUnits.Length > 2) {
					numberWithUnits = numberWithUnits.Replace ("bn", "b").Replace ("mn", "m");			
					string cleaner = numberWithUnits.Substring (0, numberWithUnits.Length - 1);
					Double.TryParse (cleaner, out retVal);

					string units = numberWithUnits.Substring (numberWithUnits.Length - 1, 1);
					switch (units) {
					case "B":
					case "b":
						retVal *= 1000000000;
						break;
					case "M":
					case "m":
						retVal *= 1000000;
						break;
					}

				} else {
					throw new Exception (string.Format ("Unable to parse {0}", ticker));
				}
			}
			return retVal;
		}
	}
}

