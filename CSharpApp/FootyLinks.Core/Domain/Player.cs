using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootyLinks.Core.DomainModel;

namespace FootyLinks.Core.Domain
{
	public class Player : Entity
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

		public virtual string Name { get; set; }
		public virtual int SourceReference { get; set; }
		public virtual Club CurrentClub { get; set; }
		public virtual IList<Club> FormerClubs { get; set; }
	}
}
