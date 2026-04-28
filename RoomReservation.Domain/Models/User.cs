namespace RoomReservation.Domain.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Login { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public string? ProfileInfo { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}