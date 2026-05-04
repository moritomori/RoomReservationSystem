namespace RoomReservation.Web.ViewModels
{
	public class RoomsIndexViewModel
	{
		public DateTime? From { get; set; }

		public DateTime? To { get; set; }

		public int? MinCapacity { get; set; }

		public List<RoomAvailabilityViewModel> Rooms { get; set; } = new();
	}
}