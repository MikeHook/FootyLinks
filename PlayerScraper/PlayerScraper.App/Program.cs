using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using PlayerScraper.Core;
using System.Net;
using System.IO;

namespace PlayerScraper.App
{
	class Program
	{
		static void Main(string[] args)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.Load(getHtmlStream("http://www.soccerbase.com/players/player.sd?player_id=31854"));

			var playerExtractor = new PlayerExtractor(doc);
			var playerName = playerExtractor.GetPlayerName();
		}

		public static Stream getHtmlStream(string siteToScrape)
		{
			WebRequest objRequest = System.Net.HttpWebRequest.Create(siteToScrape);
			WebResponse objResponse = objRequest.GetResponse();

			return objResponse.GetResponseStream();
		}
	}
}
