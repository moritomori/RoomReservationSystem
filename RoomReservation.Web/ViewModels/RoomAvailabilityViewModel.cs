using RoomReservation.Domain.Models;

namespace RoomReservation.Web.ViewModels
{
	public class RoomAvailabilityViewModel
	{
		public Room Room { get; set; } = new Room();

		public bool IsAvailable { get; set; } = true;
	}
}