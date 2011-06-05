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

			var clubNameTdNode = clubTdNodes.First();
			playerClub.ClubCompactName = clubNameTdNode != null ? clubNameTdNode.InnerText.Trim() : null;

			var clubSourceId = getClubSourceId(clubNameTdNode.Descendants("a").FirstOrDefault());
			//Note - This will throw an exception if the sourceId is null, that's fine by me
			playerClub.ClubSourceId = clubSourceId.Value;

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
			if (playerTableNode == null)
				return null;

			var playerTdNodes = playerTableNode.Descendants("td");
			var playerName = playerTdNodes.First().InnerText.Trim();
			return playerName;
		}

		public int? GetCurrentClubSourceId()
		{
			var clubLinkNode = GetCurrentClubLinkNode();
			if (clubLinkNode == null)
				return null;

			return getClubSourceId(clubLinkNode);
		}

		private string GetCurrentClubName()
		{
			var clubLinkNode = GetCurrentClubLinkNode();
			if (clubLinkNode == null)
				return null;

			return clubLinkNode.InnerText;		
		}

		public PlayerClubDto GetCurrentClubDto()
		{
			int? clubSourceId = GetCurrentClubSourceId();
			if (clubSourceId.HasValue == false)
				return null;

			var currentClubDto = _playerClubs.FirstOrDefault(c => c.ClubSourceId == GetCurrentClubSourceId());
			if (currentClubDto == null)
				return null;

			currentClubDto.ClubName = GetCurrentClubName();

			return currentClubDto;	
		}

		private int? getClubSourceId(HtmlNode clubLinkNode)
		{
			if (clubLinkNode == null)
				return null;

			var clubLink = clubLinkNode.Attributes["href"];
			if (clubLink == null || clubLink.Value == null)
				return null;

			var clubIdString = clubLink.Value.Substring(clubLink.Value.LastIndexOf('=') + 1);
			return int.Parse(clubIdString);
		}

		private HtmlNode GetCurrentClubLinkNode()
		{
			var currentClubNode = _htmlDocument.DocumentNode.SelectSingleNode("//div[@class='midfielder bull']");
			if (currentClubNode == null)
				return null;

			var currentClubLinkNode = currentClubNode.SelectSingleNode("a");
			if (currentClubLinkNode == null)
				return null;

			return currentClubLinkNode;					
		}

		public IList<PlayerClubDto> GetFormerClubs()
		{
			int? clubSourceId = GetCurrentClubSourceId();
			if (_playerClubs.Any() && clubSourceId.HasValue)
			{
				//Don't include their current club in the former clubs list
				return _playerClubs.Where(p => p.ClubSourceId != clubSourceId).ToList();
			}
			return _playerClubs;			
		}
	}
}
