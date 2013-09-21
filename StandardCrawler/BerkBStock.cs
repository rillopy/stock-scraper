using System;
using System.Data.Linq;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace StandardCrawler
{
	public class BerkBStock : Stock
	{
		public BerkBStock () : base("BRK.B")
		{

		}

		public override void Refresh ()
		{
			base.Refresh ();

			WebClient client = new WebClient ();
			
			client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
			string s = string.Empty;
			
			int i = 0;
			do {
				try {
					i++;
					Stream page = client.OpenRead ("http://finance.yahoo.com/q?s=BRK-B");
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

			// market cap
			HtmlNodeCollection coll = doc.DocumentNode.SelectNodes ("//*[@id=\"yfs_j10_brk-b\"]");
			if (coll != null) {
				foreach (HtmlNode node in coll) {
					mktCap = parseHumanNumber (node.InnerText);
				}
			}
		}
	}
}

