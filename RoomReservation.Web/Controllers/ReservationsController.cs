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

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
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

			var room = await _roomRepository.GetByIdAsync(reservation.RoomId);

			if (room == null)
			{
				return NotFound();
			}

			var model = new ReservationEditViewModel
			{
				Id = reservation.Id,
				RoomId = reservation.RoomId,
				RoomName = room.Name,
				StartTime = reservation.StartTime,
				EndTime = reservation.EndTime,
				Purpose = reservation.Purpose,
				NumberOfPeople = reservation.NumberOfPeople
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(ReservationEditViewModel model)
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var reservation = await _reservationRepository.GetByIdAsync(model.Id);

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

			var room = await _roomRepository.GetByIdAsync(reservation.RoomId);

			if (room == null)
			{
				return NotFound();
			}

			model.RoomName = room.Name;
			model.RoomId = room.Id;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (model.StartTime <= DateTime.Now)
			{
				ModelState.AddModelError(nameof(model.StartTime), "Reservation must be only for future time.");
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
				reservation.RoomId,
				model.StartTime,
				model.EndTime,
				reservation.Id);

			if (hasCollision)
			{
				ModelState.AddModelError("", "Selected room is already reserved in this time interval.");
				return View(model);
			}

			if (reservation.StartTime != model.StartTime)
			{
				await _reservationRepository.AddHistoryAsync(
					reservation.Id,
					"StartTime",
					reservation.StartTime.ToString("s"),
					model.StartTime.ToString("s"));
			}

			if (reservation.EndTime != model.EndTime)
			{
				await _reservationRepository.AddHistoryAsync(
					reservation.Id,
					"EndTime",
					reservation.EndTime.ToString("s"),
					model.EndTime.ToString("s"));
			}

			if (reservation.Purpose != model.Purpose)
			{
				await _reservationRepository.AddHistoryAsync(
					reservation.Id,
					"Purpose",
					reservation.Purpose,
					model.Purpose);
			}

			if (reservation.NumberOfPeople != model.NumberOfPeople)
			{
				await _reservationRepository.AddHistoryAsync(
					reservation.Id,
					"NumberOfPeople",
					reservation.NumberOfPeople.ToString(),
					model.NumberOfPeople.ToString());
			}

			reservation.StartTime = model.StartTime;
			reservation.EndTime = model.EndTime;
			reservation.Purpose = model.Purpose;
			reservation.NumberOfPeople = model.NumberOfPeople;

			await _reservationRepository.UpdateAsync(reservation);

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