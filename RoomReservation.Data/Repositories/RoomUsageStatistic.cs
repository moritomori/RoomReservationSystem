using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomReservation.Data.Repositories
{
	public class RoomUsageStatistic
	{
		public int RoomId { get; set; }

		public string RoomName { get; set; } = string.Empty;

		public int ReservationCount { get; set; }

		public double ReservedHours { get; set; }
	}
}
