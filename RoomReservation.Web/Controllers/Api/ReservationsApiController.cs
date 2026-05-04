using Microsoft.AspNetCore.Mvc;
using RoomReservation.Common;
using RoomReservation.Common.DTOs;
using RoomReservation.Data.Repositories;
using RoomReservation.Domain.Models;

namespace RoomReservation.Web.Controllers.Api
{
	[ApiController]
	[Route("api/reservations")]
	public class ReservationsApiController : ControllerBase
	{
		private readonly ReservationRepository _reservationRepository;
		private readonly RoomRepository _roomRepository;
		private readonly UserRepository _userRepository;

		public ReservationsApiController(
			ReservationRepository reservationRepository,
			RoomRepository roomRepository,
			UserRepository userRepository)
		{
			_reservationRepository = reservationRepository;
			_roomRepository = roomRepository;
			_userRepository = userRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var reservations = await _reservationRepository.GetAllAsync();
			return Ok(reservations);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var reservation = await _reservationRepository.GetByIdAsync(id);

			if (reservation == null)
			{
				return NotFound();
			}

			return Ok(reservation);
		}

		[HttpGet("{id}/history")]
		public async Task<IActionResult> GetHistory(int id)
		{
			var reservation = await _reservationRepository.GetByIdAsync(id);

			if (reservation == null)
			{
				return NotFound();
			}

			var history = await _reservationRepository.GetHistoryAsync(id);
			return Ok(history);
		}

		[HttpGet("user/{userId}")]
		public async Task<IActionResult> GetByUserId(int userId)
		{
			var reservations = await _reservationRepository.GetByUserIdAsync(userId);
			return Ok(reservations);
		}

		[HttpPost]
		public async Task<IActionResult> Create(ReservationCreateRequest request)
		{
			var validationError = await ValidateReservationDataAsync(
				request.RoomId,
				request.UserId,
				request.StartTime,
				request.EndTime,
				request.Purpose,
				request.NumberOfPeople);

			if (validationError != null)
			{
				return BadRequest(validationError);
			}

			bool hasCollision = await _reservationRepository.HasCollisionAsync(
				request.RoomId,
				request.StartTime,
				request.EndTime);

			if (hasCollision)
			{
				return BadRequest("Selected room is already reserved in this time interval.");
			}

			var reservation = new Reservation
			{
				StartTime = request.StartTime,
				EndTime = request.EndTime,
				Purpose = request.Purpose,
				NumberOfPeople = request.NumberOfPeople,
				UserId = request.UserId,
				RoomId = request.RoomId,
				Status = Status.Active
			};

			int newId = await _reservationRepository.CreateAsync(reservation);
			reservation.Id = newId;

			await _reservationRepository.AddHistoryAsync(
				newId,
				"Status",
				null,
				Status.Active.ToString());

			return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, ReservationUpdateRequest request)
		{
			var reservation = await _reservationRepository.GetByIdAsync(id);

			if (reservation == null)
			{
				return NotFound();
			}

			if (reservation.Status == Status.Cancelled)
			{
				return BadRequest("Cancelled reservation cannot be edited.");
			}

			if (reservation.StartTime <= DateTime.Now)
			{
				return BadRequest("Past or already started reservation cannot be edited.");
			}

			var validationError = await ValidateReservationDataAsync(
				reservation.RoomId,
				reservation.UserId,
				request.StartTime,
				request.EndTime,
				request.Purpose,
				request.NumberOfPeople);

			if (validationError != null)
			{
				return BadRequest(validationError);
			}

			bool hasCollision = await _reservationRepository.HasCollisionAsync(
				reservation.RoomId,
				request.StartTime,
				request.EndTime,
				id);

			if (hasCollision)
			{
				return BadRequest("Selected room is already reserved in this time interval.");
			}

			await AddUpdateHistoryAsync(reservation, request);

			reservation.StartTime = request.StartTime;
			reservation.EndTime = request.EndTime;
			reservation.Purpose = request.Purpose;
			reservation.NumberOfPeople = request.NumberOfPeople;

			bool updated = await _reservationRepository.UpdateAsync(reservation);

			if (!updated)
			{
				return NotFound();
			}

			return NoContent();
		}

		[HttpPost("{id}/cancel")]
		public async Task<IActionResult> Cancel(int id)
		{
			var reservation = await _reservationRepository.GetByIdAsync(id);

			if (reservation == null)
			{
				return NotFound();
			}

			if (reservation.Status == Status.Cancelled)
			{
				return BadRequest("Reservation is already cancelled.");
			}

			if (reservation.StartTime <= DateTime.Now)
			{
				return BadRequest("Past or already started reservation cannot be cancelled.");
			}

			bool cancelled = await _reservationRepository.CancelAsync(id);

			if (!cancelled)
			{
				return NotFound();
			}

			await _reservationRepository.AddHistoryAsync(
				id,
				"Status",
				reservation.Status.ToString(),
				Status.Cancelled.ToString());

			return NoContent();
		}

		private async Task<string?> ValidateReservationDataAsync(
			int roomId,
			int userId,
			DateTime startTime,
			DateTime endTime,
			string purpose,
			int numberOfPeople)
		{
			if (startTime <= DateTime.Now)
			{
				return "Reservation must be created only for future time.";
			}

			if (endTime <= startTime)
			{
				return "End time must be later than start time.";
			}

			if (string.IsNullOrWhiteSpace(purpose))
			{
				return "Purpose is required.";
			}

			if (numberOfPeople <= 0)
			{
				return "Number of people must be greater than zero.";
			}

			var room = await _roomRepository.GetByIdAsync(roomId);

			if (room == null)
			{
				return "Room does not exist.";
			}

			var user = await _userRepository.GetByIdAsync(userId);

			if (user == null)
			{
				return "User does not exist.";
			}

			if (numberOfPeople > room.Capacity)
			{
				return "Number of people cannot be greater than room capacity.";
			}

			double durationMinutes = (endTime - startTime).TotalMinutes;

			if (durationMinutes > room.MaxReservationDurationMinutes)
			{
				return "Reservation is longer than maximum allowed duration for this room.";
			}

			return null;
		}

		private async Task AddUpdateHistoryAsync(Reservation oldReservation, ReservationUpdateRequest newData)
		{
			if (oldReservation.StartTime != newData.StartTime)
			{
				await _reservationRepository.AddHistoryAsync(
					oldReservation.Id,
					"StartTime",
					oldReservation.StartTime.ToString("s"),
					newData.StartTime.ToString("s"));
			}

			if (oldReservation.EndTime != newData.EndTime)
			{
				await _reservationRepository.AddHistoryAsync(
					oldReservation.Id,
					"EndTime",
					oldReservation.EndTime.ToString("s"),
					newData.EndTime.ToString("s"));
			}

			if (oldReservation.Purpose != newData.Purpose)
			{
				await _reservationRepository.AddHistoryAsync(
					oldReservation.Id,
					"Purpose",
					oldReservation.Purpose,
					newData.Purpose);
			}

			if (oldReservation.NumberOfPeople != newData.NumberOfPeople)
			{
				await _reservationRepository.AddHistoryAsync(
					oldReservation.Id,
					"NumberOfPeople",
					oldReservation.NumberOfPeople.ToString(),
					newData.NumberOfPeople.ToString());
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var reservation = await _reservationRepository.GetByIdAsync(id);

			if (reservation == null)
			{
				return NotFound();
			}

			bool deleted = await _reservationRepository.DeleteAsync(id);

			if (!deleted)
			{
				return NotFound();
			}

			return NoContent();
		}
	}
}