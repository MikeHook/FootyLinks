using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using PlayerScraper.Core;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace PlayerScraper.App
{
	class Program
	{
		static void Main(string[] args)
		{
			HtmlDocument doc = new HtmlDocument();

			int playerId = 31854;
			string filePath = string.Format(@"C:\_Development\Android\FootyLinks\PlayerScraper\PlayerScraper.App\ScrappedHtml\{0}", playerId);
			//writeStreamToFile(filePath, getHtmlStream("http://www.soccerbase.com/players/player.sd?player_id=", playerId));

			doc.Load(filePath);

			var playerExtractor = new PlayerExtractor(doc);
			var playerName = playerExtractor.GetPlayerName();
		}

		public static Stream getHtmlStream(string pageToScrape, int playerId)
		{
			string httpAddress = string.Format("{0}{1}",pageToScrape, playerId); 

			WebRequest objRequest = System.Net.HttpWebRequest.Create(httpAddress);
			WebResponse objResponse = objRequest.GetResponse();

			return objResponse.GetResponseStream();
		}


		public static void writeStreamToFile(string filePath, Stream stream)
		{			
			using (var streamReader = new StreamReader(stream))
			{
				string fileContents = streamReader.ReadToEnd();
				//Remove the carriage return, new lines
				fileContents = Regex.Replace(fileContents, "[\t\r\n]", " ");

				File.WriteAllText(filePath, fileContents);
			}
		}
	}
}
