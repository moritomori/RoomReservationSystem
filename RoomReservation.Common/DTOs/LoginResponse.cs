namespace RoomReservation.Common.DTOs
{
	public class LoginResponse
	{
		public int UserId { get; set; }

		public string Login { get; set; } = string.Empty;

		public string Token { get; set; } = string.Empty;
	}
}