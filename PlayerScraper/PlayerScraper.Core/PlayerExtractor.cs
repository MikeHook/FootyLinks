using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace PlayerScraper.Core
{
	public class PlayerExtractor
	{
		private HtmlDocument _htmlDocument;

		public PlayerExtractor(HtmlDocument htmlDocument)
		{
			_htmlDocument = htmlDocument;
		}

		public string GetPlayerName()
		{
			//Retrieve the player name from the 'clubInfo' table
			var playerTableNode = _htmlDocument.DocumentNode.SelectSingleNode("//table[@class='clubInfo']");
			var playerTdNodes = playerTableNode.Descendants("td");
			var playerName = playerTdNodes.First().InnerText.Trim();
			return playerName;
		}
	}
}
