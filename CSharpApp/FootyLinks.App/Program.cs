using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using FootyLinks.Processes;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace FootyLinks.App
{
	class Program
	{
		static void Main(string[] args)
		{
			//string filePath = @"C:\_Development\FootyLinks\PlayersSource\41328.html";

			string sourceFolder = @"C:\_Development\FootyLinks\PlayersSource\";
			var playerFiles = Directory.GetFiles(sourceFolder);

			foreach (var playerFile in playerFiles)
			{
				importPlayer(playerFile);
			}			
		}

		private static void importPlayer(string sourceFilePath)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.Load(sourceFilePath);

			var playerExtractor = new PlayerExtractor(doc);
			string playerName = playerExtractor.GetPlayerName();
			string currentClub = playerExtractor.GetCurrentClubName();
			IList<string> formerClubs = playerExtractor.GetFormerClubs();

			//TODO - Use the NHibernateHelper to open a session and save new players and clubs
			//TODO2 - Wire up the NHibernate Linq provider in the DatabaseDeployer to test it
		}
	}
}
