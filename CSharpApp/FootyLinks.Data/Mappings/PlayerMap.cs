using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootyLinks.Core;
using FluentNHibernate.Mapping;

namespace FootyLinks.Data.Mappings
{
	public class PlayerMap : ClassMap<Player>
	{
		public PlayerMap()
		{
			Id(x => x.Id);
			Map(x => x.Name);
			Map(x => x.SourceReference);

			References(x => x.CurrentClub)
				.Column("CurrentClub_id");

			HasManyToMany(x => x.FormerClubs)
				.ForeignKeyConstraintNames("FK_FormerClub", "FK_FormerPlayer")
				.Cascade.All()
				.Inverse()
				.Table("PlayerClub")			
				;
			
	
		}
	}
}
