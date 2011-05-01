using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootyLinks.Core.Domain
{
	public class Player
	{
		public Player()
		{
			FormerClubs = new List<Club>();
		}

		public Player(string name, Club currentClub) :this()
		{
			Name = name;			
			currentClub.AddCurrentPlayer(this);
		}

		public virtual int Id { get; private set; }
		public virtual string Name { get; set; }
		public virtual int SourceReference { get; set; }
		public virtual Club CurrentClub { get; set; }
		public virtual IList<Club> FormerClubs { get; set; }
	}
}
