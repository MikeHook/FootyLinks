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
			var playerNode = _htmlDocument.DocumentNode.SelectSingleNode("//a[@href");
			var playerNameNode = playerNode.SelectSingleNode("");

			return "";
		}
	}
}
