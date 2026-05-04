namespace RoomReservation.Web.ViewModels
{
	public class RoomUsageStatisticViewModel
	{
		public int RoomId { get; set; }

		public string RoomName { get; set; } = string.Empty;

		public int ReservationCount { get; set; }

		public double ReservedHours { get; set; }
	}
}