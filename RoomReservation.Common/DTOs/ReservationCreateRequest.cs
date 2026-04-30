namespace RoomReservation.Common.DTOs
{
	public class ReservationCreateRequest
	{
		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public string Purpose { get; set; } = string.Empty;

		public int NumberOfPeople { get; set; }

		public int UserId { get; set; }

		public int RoomId { get; set; }
	}
}