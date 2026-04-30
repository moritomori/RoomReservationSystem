namespace RoomReservation.Common.DTOs
{
	public class ReservationUpdateRequest
	{
		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public string Purpose { get; set; } = string.Empty;

		public int NumberOfPeople { get; set; }
	}
}