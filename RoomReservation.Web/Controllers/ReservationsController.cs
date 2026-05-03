using Microsoft.AspNetCore.Mvc;
using RoomReservation.Common;
using RoomReservation.Data.Repositories;
using RoomReservation.Domain.Models;
using RoomReservation.Web.ViewModels;

namespace RoomReservation.Web.Controllers
{
	public class ReservationsController : Controller
	{
		private readonly ReservationRepository _reservationRepository;
		private readonly RoomRepository _roomRepository;

		public ReservationsController(
			ReservationRepository reservationRepository,
			RoomRepository roomRepository)
		{
			_reservationRepository = reservationRepository;
			_roomRepository = roomRepository;
		}

		public async Task<IActionResult> MyReservations()
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var reservations = await _reservationRepository.GetByUserIdAsync(userId.Value);
			return View(reservations);
		}

		[HttpGet]
		public async Task<IActionResult> Create(int roomId)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var room = await _roomRepository.GetByIdAsync(roomId);

			if (room == null)
			{
				return NotFound();
			}

			var model = new ReservationCreateViewModel
			{
				RoomId = room.Id,
				RoomName = room.Name
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Create(ReservationCreateViewModel model)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var room = await _roomRepository.GetByIdAsync(model.RoomId);

			if (room == null)
			{
				return NotFound();
			}

			model.RoomName = room.Name;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (model.StartTime <= DateTime.Now)
			{
				ModelState.AddModelError(nameof(model.StartTime), "Reservation must be created only for future time.");
				return View(model);
			}

			if (model.EndTime <= model.StartTime)
			{
				ModelState.AddModelError(nameof(model.EndTime), "End time must be later than start time.");
				return View(model);
			}

			if (model.NumberOfPeople > room.Capacity)
			{
				ModelState.AddModelError(nameof(model.NumberOfPeople), "Number of people cannot be greater than room capacity.");
				return View(model);
			}

			double durationMinutes = (model.EndTime - model.StartTime).TotalMinutes;

			if (durationMinutes > room.MaxReservationDurationMinutes)
			{
				ModelState.AddModelError(nameof(model.EndTime), "Reservation is longer than maximum allowed duration for this room.");
				return View(model);
			}

			bool hasCollision = await _reservationRepository.HasCollisionAsync(
				model.RoomId,
				model.StartTime,
				model.EndTime);

			if (hasCollision)
			{
				ModelState.AddModelError("", "Selected room is already reserved in this time interval.");
				return View(model);
			}

			var reservation = new Reservation
			{
				StartTime = model.StartTime,
				EndTime = model.EndTime,
				Purpose = model.Purpose,
				NumberOfPeople = model.NumberOfPeople,
				UserId = userId.Value,
				RoomId = model.RoomId,
				Status = Status.Active
			};

			int newId = await _reservationRepository.CreateAsync(reservation);

			await _reservationRepository.AddHistoryAsync(
				newId,
				"Status",
				null,
				Status.Active.ToString());

			return RedirectToAction(nameof(MyReservations));
		}

		[HttpPost]
		public async Task<IActionResult> Cancel(int id)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var reservation = await _reservationRepository.GetByIdAsync(id);

			if (reservation == null)
			{
				return NotFound();
			}

			if (reservation.UserId != userId.Value)
			{
				return Forbid();
			}

			if (reservation.Status == Status.Cancelled || reservation.StartTime <= DateTime.Now)
			{
				return RedirectToAction(nameof(MyReservations));
			}

			await _reservationRepository.CancelAsync(id);

			await _reservationRepository.AddHistoryAsync(
				id,
				"Status",
				reservation.Status.ToString(),
				Status.Cancelled.ToString());

			return RedirectToAction(nameof(MyReservations));
		}
	}
}