namespace RoomReservation.Domain.Models
{
	public class ReservationHistory
	{
		public int Id { get; set; }
		public int ReservationId { get; set; }
		public string FieldName { get; set; } = string.Empty;
		public string? OldValue { get; set; }
		public string? NewValue { get; set; }
		public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
	}
}