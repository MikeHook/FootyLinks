using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootyLinks.Core.DomainModel;

namespace FootyLinks.Core.Domain
{
	public class Club : Entity
	{
		protected Club()
		{
			CurrentPlayers = new List<Player>();
			FormerPlayers = new List<Player>();
		}

		public Club(string name) : this()
		{
			Name = name;
		}
	
		public virtual string Name { get; set; }
		public virtual IList<Player> CurrentPlayers { get; set; }
		public virtual IList<Player> FormerPlayers { get; set; }

		public virtual void AddCurrentPlayer(Player player)
		{
			player.CurrentClub = this;
			if (CurrentPlayers.Contains(player) == false)
			{
				CurrentPlayers.Add(player);
			}
		}

		public virtual void AddFormerPlayer(Player player)
		{
			if (player.FormerClubs.Contains(this) == false)
			{
				player.FormerClubs.Add(this);
			}
			if (FormerPlayers.Contains(player) == false)
			{
				FormerPlayers.Add(player);
			}
		}
	}
}
