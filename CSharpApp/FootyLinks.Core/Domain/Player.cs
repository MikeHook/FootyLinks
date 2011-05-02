using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootyLinks.Core.DomainModel;

namespace FootyLinks.Core.Domain
{
	public class Player : Entity
	{
		protected Player()
		{
			FormerClubs = new List<Club>();
		}

		public Player(int sourceReference, string name, Club currentClub) :this()
		{
			SourceReference = sourceReference;
			Name = name;			
			currentClub.AddCurrentPlayer(this);
		}

		public Player(int sourceReference, string name, List<Club> formerClubs)	: this()
		{
			SourceReference = sourceReference;
			Name = name;
			FormerClubs = formerClubs;
			foreach (var club in FormerClubs)
			{
				club.AddFormerPlayer(this);
			}
		}

		public virtual string Name { get; set; }
		public virtual int SourceReference { get; private set; }
		public virtual Club CurrentClub { get; set; }

		//This property should be set through the Club.AddFormerPlayer() method to enforce
		//the relationship in both directions
		public virtual IList<Club> FormerClubs { get; private set; }
	}
}
