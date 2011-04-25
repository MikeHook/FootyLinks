using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootyLinks.Core
{
	public class Club
	{
		public Club()
		{
			CurrentPlayers = new List<Player>();
			FormerPlayers = new List<Player>();
		}

		public Club(string name) : this()
		{
			Name = name;
		}

		public virtual int Id { get; private set; }
		public virtual string Name { get; set; }
		public virtual IList<Player> CurrentPlayers { get; set; }
		public virtual IList<Player> FormerPlayers { get; set; }

		public virtual void AddCurrentPlayer(Player player)
		{
			player.CurrentClub = this;
			CurrentPlayers.Add(player);
		}

		public virtual void AddFormerPlayer(Player player)
		{
			player.FormerClubs.Add(this);
			FormerPlayers.Add(player);
		}
	}
}
