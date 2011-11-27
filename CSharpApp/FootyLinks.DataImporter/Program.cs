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
using FootyLinks.Core.Dto;

namespace FootyLinks.DataImporter
{
	class Program
	{
		public static string ErrorLogPath = @"C:\_Development\FootyLinks\ErrorLog\importErrorlog.txt";
		public static string InfoLogPath = @"C:\_Development\FootyLinks\ErrorLog\importInfolog.txt";

		/*
		static void Main(string[] args)
		{
			//Reina: 24340		 
			string filePath = @"C:\_Development\FootyLinks\PlayersSource\24340.html";
			//Test(filePath);

			var playerFileInfo = new FileInfo(filePath);
			int sourceReference = int.Parse(playerFileInfo.Name.Split('.')[0]);
			importPlayer(filePath, sourceReference);
		}
		 */

		
		static void Main(string[] args)
		{
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
						writeErrorToFile(sourceReference, ex);
					}
				}
			}
			catch (Exception ex)
			{
				writeErrorToFile(0, ex);
			}
			
		}
		

		private static void Test(string sourceFilePath)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.Load(sourceFilePath);

			var playerExtractor = new PlayerExtractor(doc);
			int? squadNo = playerExtractor.GetSquadNumber();
			int? clubId = playerExtractor.GetCurrentClubSourceId();

			var clubDto = playerExtractor.GetCurrentClubDto();
		}

		private static void importPlayer(string sourceFilePath, int sourceReference)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.Load(sourceFilePath);

			var playerExtractor = new PlayerExtractor(doc);

			string playerName = playerExtractor.GetPlayerName();
			//Skip the import if the minimum required info is not present
			if (string.IsNullOrEmpty(playerName))
			{
				writeInfoToFile(sourceReference, "No player name found");
				return;
			}			

			var currentClubDto = playerExtractor.GetCurrentClubDto();
			IList<PlayerClubDto> formerClubDtos = playerExtractor.GetFormerClubs();
			if (currentClubDto == null || formerClubDtos.Count == 0)
			{
				writeInfoToFile(sourceReference, "Current or former clubs not found for player: " + playerName);
				return;
			}

			//Import the Player and clubs
			using (var session = NHibernateHelper.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					try
					{
						importPlayerRecord(session, currentClubDto, formerClubDtos,
											playerName, sourceReference,
											playerExtractor.GetSquadNumber());
						transaction.Commit();
					}
					catch (Exception ex)
					{
						writeErrorToFile(sourceReference, ex);
						transaction.Rollback();
					}
				}
				session.Clear();
			}
		}

		private static void importPlayerRecord(ISession session, PlayerClubDto currentClubDto,
										IList<PlayerClubDto> formerClubDtos,
										string playerName, int sourceReference, int? squadNumber)
		{
			List<Club> formerClubs = new List<Club>();
			foreach (var formerClubDto in formerClubDtos)
			{
				var formerClub = FindCreateClubBySourceId(session, formerClubDto);
				session.SaveOrUpdate(formerClub);
				formerClubs.Add(formerClub);
			}

			Player player;
			if (currentClubDto != null)
			{
				Club currentClub = FindCreateClubBySourceId(session, currentClubDto);
				currentClub.Name = currentClubDto.ClubName;
				session.SaveOrUpdate(currentClub);
				player = FindPlayer(session, sourceReference) ??
					new Player(sourceReference, playerName, currentClub);
				player.SquadNumber = squadNumber;
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
		private static Club FindCreateClubBySourceId(ISession session, PlayerClubDto clubDto)
		{
			var clubs = session.QueryOver<Club>().Where(p => p.SourceId == clubDto.ClubSourceId).List();

			if (clubs == null || clubs.Count > 1)
				return null;

			return clubs.Count == 1 ? clubs[0] : new Club(clubDto.ClubSourceId, clubDto.ClubCompactName);
		}

		private static Player FindPlayer(ISession session, int sourceReference)
		{
			var players = session.QueryOver<Player>().Where(p => p.SourceReference == sourceReference).List();

			if (players == null || players.Count > 1)
				return null;

			return players.Count == 1 ? players[0] : null;
		}

		private static void writeInfoToFile(int sourceReference, string message)
		{
			File.AppendAllText(InfoLogPath, string.Format("Source Reference: {0}\r\n", sourceReference));

			File.AppendAllText(InfoLogPath, string.Format("{0}\r\n", message));
			File.AppendAllText(InfoLogPath, "\r\n\r\n");
		}

		private static void writeErrorToFile(int sourceReference, Exception ex)
		{
			File.AppendAllText(ErrorLogPath, string.Format("Source Reference: {0}\r\n", sourceReference));

			File.AppendAllText(ErrorLogPath, string.Format("{0}\r\n", ex.ToString()));
			File.AppendAllText(ErrorLogPath, "\r\n\r\n");
		}

	}
}
