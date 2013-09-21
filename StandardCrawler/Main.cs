using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.IO;
using System.Threading;
using GemBox.Spreadsheet;

namespace StandardCrawler
{
	class MainClass
	{
		public static string sp500url = "http://us.spindices.com/idsexport/file.xls?hostIdentifier=48190c8c-42c4-46af-8d1a-0cd5db894797&selectedModule=Constituents&selectedSubModule=ConstituentsFullList&indexId=340";

		public static void Main (string[] args)
		{
			WebClient client = new WebClient ();

			client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
			client.DownloadFile (sp500url, "out.xls");

			var fileName = string.Format ("{0}/out.xls", Directory.GetCurrentDirectory ());

			// Set license key to use GemBox.Spreadsheet in a Free mode.
			SpreadsheetInfo.SetLicense ("FREE-LIMITED-KEY");
			SpreadsheetInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;

			Hashtable symbols = new Hashtable ();

			do {
				PopulateSymbols (fileName, symbols);
			} while (symbols.ContainsValue("TRIAL"));
			Random r = new Random ();

			foreach (var key in symbols.Keys) {

				Stock myStock = null;
				if ( symbols[key].ToString() == "BRK.B")
				{
					myStock = new BerkBStock();
				}
				else{
					myStock = new Stock (symbols [key].ToString ());
				}
				Thread.Sleep (r.Next (6000));
				try {
					if (myStock != null){
					myStock.Refresh ();
					Console.WriteLine (string.Format ("{0} {1} {2} {3} {4}", myStock.ticker, myStock.price, myStock.earningsPerShare, myStock.mktCap, myStock.freeFloat));
					}
				} catch (Exception e) {
					Console.WriteLine (e.Message);
				}
			}
		}

		public static void PopulateSymbols (string filename, Hashtable symbols)
		{
			ExcelFile ef = ExcelFile.Load (filename);
			bool capture = false;
			foreach (var sheet in ef.Worksheets) {
				foreach (var row in sheet.Rows) {
					var cell = row.Cells [1];

					if (cell.Value != null && capture && (symbols [row.Index] == null || symbols [row.Index].ToString () == "TRIAL")) {
						switch (cell.Value.ToString ()) {
						case "ABC":
						case "ACT":
						case "ACE":
						case "AEP":
						case "AMD":
						case "GAS":
						case "APH":
						case "ADM":
						case "ARG":
						case "AGN":
						case "ATI":
						case "AXP":
						case "AMP":
						case "ALL":
						case "APC":
						case "ADT":
						case "ADI":
						case "AVP":
						case "BA":
						case "BBY":
						case "BMS":
						case "BMY":
						case "CVC":
						case "COG":
						case "CAM":
						case "CAT":
						case "CBG":
						case "CBS":
						case "CCL":
						case "CMG":
						case "CMS":
						case "CRM":
						case "CF":
						case "CFN":
						case "CLF":
						case "CME":
						case "COP":
						case "CCE":
						case "STZ":
						case "COV":
						case "CSX":
						case "CMI":
						case "DG":
						case "DIS":
						case "DVA":
						case "DPS":
						case "ECL":
						case "EW":
						case "EMR":
						case "ESV":
						case "EOG":
						case "FLS":
						case "FTI":
						case "FRX":
						case "HAL":
						case "HAS":
						case "HCP":
						case "HON":
						case "HOT":
						case "HRL":
						case "HSP":
						case "ICE":
						case "HUM":
						case "TEG":
						case "KEY":
						case "KMI":
						case "L":
						case "LEG":
						case "LNC":
						case "LMT":
						case "LO":
						case "LSI":
						case "MAC":
						case "MAR":
						case "MAS":
						case "MET":
						case "MPC":
						case "MA":
						case "MCK":
						case "MCC":
						case "MMC":
						case "MRO":
						case "TAP":
						case "MON":
						case "MS":
						case "MOS":
						case "MIS":
						case "MRK":
						case "MSI":
						case "MUR":
						case "NBR":
						case "NEE":
						case "NEM":
						case "NE":
						case "NI":
						case "NBL":
						case "PEP":
						case "PM":
						case "PGR":
						case "PEG":
						case "PHM":
						case "PFG":
						case "POM":
						case "PPL":
						case "PRU":
						case "RTN":
						case "RRC":
						case "SAI":
						case "SEE":
						case "SHW":
						case "SNI":
						case "SO":
						case "SPG":
						case "SRE":
						case "SWN":
						case "RF":
						case "S":
						case "STJ":
						case "STT":
						case "STI":
						case "TEL":
						case "TDC":
						case "TMO":
						case "TWC":
						case "TRV":
						case "TSN":
						case "UPS":
						case "UNM":
						case "VLO":
						case "VTR":
						case "V":
						case "WM":
						case "WAT":
						case "WEC":
						case "WPX":
						case "WYN":
						case "XEL":
						case "XL":
						case "XYL":
						case "ZTS":
							symbols [row.Index] = string.Format ("{0}:{1}", cell.Value.ToString (), "NYQ");
							break;
						case "ALTR":
						case "APOL":
						case "COST":
						case "FAST":
						case "FITB":
						case "INTU":
						case "LIFE":
						case "TRIP":
						case "WFM":
						case "WIN":
							symbols [row.Index] = string.Format ("{0}:{1}", cell.Value.ToString (), "NSQ");
							break;
						case "BMC":
							// replace BMC Software with Delta Airlines 
							symbols [row.Index] = string.Format ("{0}:{1}", "DAL", "NYQ");
							break;
						default:
							symbols [row.Index] = cell.Value.ToString ();
							break;
						}
					}

					if (cell.Value != null && cell.Value.ToString () == "Symbol") { 
						capture = true;
					}
					
					if (capture && (cell.Value == null || cell.Value.ToString () == String.Empty)) {
						break;
					}
				}
			}
		}
	}
}
