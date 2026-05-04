using Microsoft.AspNetCore.Mvc;
using RoomReservation.Data.Repositories;

namespace RoomReservation.Web.Controllers
{
	public class StatisticsController : Controller
	{
		private readonly ReservationRepository _reservationRepository;

		public StatisticsController(ReservationRepository reservationRepository)
		{
			_reservationRepository = reservationRepository;
		}

		public async Task<IActionResult> Rooms(DateTime? from, DateTime? to)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			DateTime selectedFrom = from ?? DateTime.Now.Date;
			DateTime selectedTo = to ?? DateTime.Now.Date.AddDays(7);

			if (selectedTo <= selectedFrom)
			{
				ModelState.AddModelError("", "Date 'to' must be later than date 'from'.");
			}

			var statistics = await _reservationRepository.GetRoomUsageStatisticsAsync(selectedFrom, selectedTo);

			ViewBag.From = selectedFrom;
			ViewBag.To = selectedTo;

			return View(statistics);
		}
	}
}