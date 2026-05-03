using Microsoft.AspNetCore.Mvc;
using RoomReservation.Data.Repositories;

namespace RoomReservation.Web.Controllers
{
	public class RoomsController : Controller
	{
		private readonly RoomRepository _roomRepository;

		public RoomsController(RoomRepository roomRepository)
		{
			_roomRepository = roomRepository;
		}

		public async Task<IActionResult> Index()
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var rooms = await _roomRepository.GetAllAsync();
			return View(rooms);
		}
	}
}