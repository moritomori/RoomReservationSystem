using RoomReservation.Common;

namespace RoomReservation.Domain.Models
{
	public class Reservation
	{
		public int Id { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Purpose { get; set; } = string.Empty;
		public int NumberOfPeople { get; set; }
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public Status Status { get; set; } = Status.Active;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}