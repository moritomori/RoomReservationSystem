using RoomReservation.Common;

namespace RoomReservation.Common.DTOs
{
	public class ReservationListItemDto
	{
		public int Id { get; set; }

		public string RoomName { get; set; } = string.Empty;

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public string Purpose { get; set; } = string.Empty;

		public int NumberOfPeople { get; set; }

		public Status Status { get; set; }
	}
}