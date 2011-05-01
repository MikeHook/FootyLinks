using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using FootyLinks.Core.Dto;

namespace FootyLinks.Processes
{
	public class PlayerExtractor
	{
		private HtmlDocument _htmlDocument;
		private IList<PlayerClubDto> _playerClubs;

		public PlayerExtractor(HtmlDocument htmlDocument)
		{
			_htmlDocument = htmlDocument;
			_playerClubs = getPlayerClubs();
		}

		private IList<PlayerClubDto> getPlayerClubs()
		{
			//Fetch a list of the playersClubs
			var playerClubs = new List<PlayerClubDto>();

			HtmlNode careerTableNode = _htmlDocument.DocumentNode.SelectSingleNode("//table[@class='table right career']");
			if (careerTableNode == null)
				return playerClubs;

			var careerTrNodes = careerTableNode.Descendants("tr");
			//If the player has a single club then there should be 5 'tr' rows
			if (careerTrNodes.Count() < 5)
				return playerClubs;

			var clubTrNodes = careerTrNodes.Skip(2).Take(careerTrNodes.Count() - 4);
			foreach (var clubTrNode in clubTrNodes)
			{
				playerClubs.Add(getPlayerClub(clubTrNode));
			}
			return playerClubs;
		}

		private PlayerClubDto getPlayerClub(HtmlNode clubTrNode)
		{
			var playerClub = new PlayerClubDto();

			var clubTdNodes = clubTrNode.Descendants("td");
			if (clubTdNodes == null || clubTdNodes.Count() < 3)
				return playerClub;

			var clubNameNode = clubTdNodes.First();
			playerClub.ClubName = clubNameNode != null ? clubNameNode.InnerText.Trim() : null;

			var playerJoinDateNode = clubTdNodes.ElementAt(1);
			DateTime joinDate;
			if (playerJoinDateNode != null && DateTime.TryParse(playerJoinDateNode.InnerText.Trim(), out joinDate))
			{
				playerClub.PlayerJoinDate = joinDate;				
			}

			var playerLeftDateNode = clubTdNodes.ElementAt(2);
			DateTime leftDate;
			if (playerLeftDateNode != null && DateTime.TryParse(playerLeftDateNode.InnerText.Trim(), out leftDate))
			{
				playerClub.PlayerLeftDate = leftDate;
			}
			return playerClub;
		}

		public string GetPlayerName()
		{
			//Retrieve the player name from the 'clubInfo' table
			var playerTableNode = _htmlDocument.DocumentNode.SelectSingleNode("//table[@class='clubInfo']");
			var playerTdNodes = playerTableNode.Descendants("td");
			var playerName = playerTdNodes.First().InnerText.Trim();
			return playerName;
		}

		public string GetCurrentClubName()
		{
			if (_playerClubs.Any() == false)
				return null;

			PlayerClubDto mostRecentClub = _playerClubs.First();

			//There is a leaving date for their most recent club so they must be retired
			if (mostRecentClub.PlayerLeftDate != null)
				return null;

			return mostRecentClub.ClubName;
		}

		public IList<string> GetFormerClubs()
		{
			var formerClubs = new List<string>();
			if (_playerClubs.Any() == false)
				return formerClubs;

			var currentClubName = GetCurrentClubName();
			//The first playerClub may be their currentClub, if so it should be skipped
			var formerClubDtos = string.IsNullOrEmpty(currentClubName) ?
									_playerClubs : _playerClubs.Skip(1).Take(_playerClubs.Count - 1);

			foreach (var formerClubDto in formerClubDtos)
			{
				formerClubs.Add(formerClubDto.ClubName);
			}

			return formerClubs;
		}
	}
}
