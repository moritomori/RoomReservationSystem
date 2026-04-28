namespace RoomReservation.Domain.Models
{
	public class Room
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Capacity { get; set; }
		public string? Equipment { get; set; }
		public int MaxReservationDurationMinutes { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
