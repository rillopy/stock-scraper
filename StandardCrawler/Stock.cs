using System;

namespace StandardCrawler
{
	public class Stock
	{
		public string ticker;
		public double mktCap;
		public double earningsPerShare;
		public double price;
		public Int64 freeFloat;
		public int outstandingShares;
		public Int64 shares;

		public virtual void Refresh () 
		{

		}

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

