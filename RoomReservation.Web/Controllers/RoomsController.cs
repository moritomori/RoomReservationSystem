using Microsoft.AspNetCore.Mvc;
using RoomReservation.Data.Repositories;
using RoomReservation.Web.ViewModels;

namespace RoomReservation.Web.Controllers
{
	public class RoomsController : Controller
	{
		private readonly RoomRepository _roomRepository;
		private readonly ReservationRepository _reservationRepository;

		public RoomsController(
			RoomRepository roomRepository,
			ReservationRepository reservationRepository)
		{
			_roomRepository = roomRepository;
			_reservationRepository = reservationRepository;
		}

		public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? minCapacity, string? sort)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var rooms = await _roomRepository.GetAllSortedAsync(sort);

			if (minCapacity.HasValue)
			{
				rooms = rooms.Where(room => room.Capacity >= minCapacity.Value);
			}

			var model = new RoomsIndexViewModel
			{
				From = from,
				To = to,
				MinCapacity = minCapacity
			};

			foreach (var room in rooms)
			{
				bool isAvailable = true;

				if (from.HasValue && to.HasValue && to.Value > from.Value)
				{
					bool hasCollision = await _reservationRepository.HasCollisionAsync(
						room.Id,
						from.Value,
						to.Value);

					isAvailable = !hasCollision;
				}

				model.Rooms.Add(new RoomAvailabilityViewModel
				{
					Room = room,
					IsAvailable = isAvailable
				});
			}

			return View(model);
		}
	}
}