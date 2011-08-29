using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootyLinks.Core.Domain;
using FluentNHibernate.Mapping;

namespace FootyLinks.Data.Mappings
{
	public class ClubMap : ClassMap<Club>
	{
		public ClubMap()
		{
			Id(x => x.Id).GeneratedBy.Increment();
			Map(x => x.SourceId);
			Map(x => x.Name);
			Map(x => x.CompactName);
			
			HasMany(x => x.CurrentPlayers)
				.KeyColumn("CurrentClub_id")
				.ForeignKeyConstraintName("FK_CurrentClub")
				.Inverse()
				.Cascade.All();
			
			HasManyToMany(x => x.FormerPlayers)
				.ForeignKeyConstraintNames("FK_FormerClub", "FK_FormerPlayer")
				.Cascade.All()
				.Table("PlayerClub");
			  
		}
	}
}
