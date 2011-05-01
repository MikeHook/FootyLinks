using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootyLinks.Core.Dto
{
	public class PlayerClubDto
	{
		public string ClubName { get; set; }
		public DateTime? PlayerJoinDate { get; set; }
		public DateTime? PlayerLeftDate { get; set; } 

	}
}
