using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using FootyLinks.Data;
using FootyLinks.Processes;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using FootyLinks.Core.Domain;
using NHibernate;

namespace FootyLinks.DataImporter
{
	class Program
	{
		public static string ErrorLogPath = @"C:\_Development\FootyLinks\ErrorLog\importlog.txt";


		static void Main(string[] args)
		{
			//string filePath = @"C:\_Development\FootyLinks\PlayersSource\41328.html";

			try
			{
				string sourceFolder = @"C:\_Development\FootyLinks\PlayersSource\";
				var playerFiles = Directory.GetFiles(sourceFolder);

				int playersImported = 0;
				foreach (var playerFile in playerFiles)
				{
					var playerFileInfo = new FileInfo(playerFile);
					int sourceReference = int.Parse(playerFileInfo.Name.Split('.')[0]);
					try
					{
						importPlayer(playerFile, sourceReference);
						playersImported++;
						if (playersImported % 100 == 0)
							Console.WriteLine(string.Format("Imported {0} players", playersImported));
					}
					catch (Exception ex)
					{
						writeErrorToFile(ErrorLogPath, sourceReference, ex);
					}
				}
			}
			catch (Exception ex)
			{
				writeErrorToFile(ErrorLogPath, 0, ex);
			}
			
		}

		private static void importPlayer(string sourceFilePath, int sourceReference)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.Load(sourceFilePath);

			var playerExtractor = new PlayerExtractor(doc);
			string playerName = playerExtractor.GetPlayerName();
			string currentClubName = playerExtractor.GetCurrentClubName();
			IList<string> formerClubNames = playerExtractor.GetFormerClubs();

			//Skip the import if the minimum required info is not present
			//Could add some info logging here
			if (string.IsNullOrEmpty(playerName))
				return;
			if (string.IsNullOrEmpty(currentClubName) && formerClubNames.Count == 0)
				return;

			//Import the Player and clubs
			using (var session = NHibernateHelper.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					try
					{
						importPlayerRecord(session, currentClubName, formerClubNames,
											playerName, sourceReference);
						transaction.Commit();
					}
					catch (Exception ex)
					{
						writeErrorToFile(ErrorLogPath, sourceReference, ex);
						transaction.Rollback();
					}
				}
				session.Clear();
			}
		}

		private static void importPlayerRecord(ISession session, string currentClubName,
										IList<string> formerClubNames,
										string playerName, int sourceReference)
		{
			List<Club> formerClubs = new List<Club>();
			foreach (string formerClubName in formerClubNames)
			{
				var formerClub = FindCreateClub(session, formerClubName);
				session.SaveOrUpdate(formerClub);
				formerClubs.Add(formerClub);
			}

			Player player;
			if (string.IsNullOrEmpty(currentClubName) == false)
			{
				Club currentClub = FindCreateClub(session, currentClubName);
				session.SaveOrUpdate(currentClub);
				player = FindPlayer(session, sourceReference) ??
					new Player(sourceReference, playerName, currentClub);
				foreach (var club in formerClubs)
				{
					club.AddFormerPlayer(player);
				}
			}
			else
			{
				player = FindPlayer(session, sourceReference) ??
					new Player(sourceReference, playerName, formerClubs);
			}

			session.SaveOrUpdate(player);
		}


		//TODO - Refactor these into Repository classes if they will be used anywhere else		
		private static Club FindCreateClub(ISession session, string clubName)
		{
			var clubs = session.QueryOver<Club>().Where(p => p.Name == clubName).List();

			if (clubs == null || clubs.Count > 1)
				return null;

			return clubs.Count == 1 ? clubs[0] : new Club(clubName);
		}

		private static Player FindPlayer(ISession session, int sourceReference)
		{
			var players = session.QueryOver<Player>().Where(p => p.SourceReference == sourceReference).List();

			if (players == null || players.Count > 1)
				return null;

			return players.Count == 1 ? players[0] : null;
		}

		private static void writeErrorToFile(string errorFile, int sourceReference, Exception ex)
		{
			File.AppendAllText(errorFile, string.Format("Source Reference: {0}\r\n", sourceReference));

			File.AppendAllText(errorFile, string.Format("{0}\r\n", ex.ToString()));			
			File.AppendAllText(errorFile, "\r\n\r\n");
		}

	}
}
