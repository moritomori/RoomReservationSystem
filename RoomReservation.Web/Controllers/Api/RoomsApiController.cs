using Microsoft.AspNetCore.Mvc;
using RoomReservation.Data.Repositories;
using RoomReservation.Domain.Models;

namespace RoomReservation.Web.Controllers.Api
{
	[ApiController]
	[Route("api/rooms")]
	public class RoomsApiController : ControllerBase
	{
		private readonly RoomRepository _roomRepository;

		public RoomsApiController(RoomRepository roomRepository)
		{
			_roomRepository = roomRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var rooms = await _roomRepository.GetAllAsync();
			return Ok(rooms);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var room = await _roomRepository.GetByIdAsync(id);

			if (room == null)
			{
				return NotFound();
			}

			return Ok(room);
		}

		[HttpPost]
		public async Task<IActionResult> Create(Room room)
		{
			if (string.IsNullOrWhiteSpace(room.Name))
			{
				return BadRequest("Room name is required.");
			}

			if (room.Capacity <= 0)
			{
				return BadRequest("Capacity must be greater than zero.");
			}

			if (room.MaxReservationDurationMinutes <= 0)
			{
				return BadRequest("Max reservation duration must be greater than zero.");
			}

			int newId = await _roomRepository.CreateAsync(room);
			room.Id = newId;

			return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, Room room)
		{
			if (id != room.Id)
			{
				return BadRequest("Route id and room id are different.");
			}

			if (string.IsNullOrWhiteSpace(room.Name))
			{
				return BadRequest("Room name is required.");
			}

			if (room.Capacity <= 0)
			{
				return BadRequest("Capacity must be greater than zero.");
			}

			if (room.MaxReservationDurationMinutes <= 0)
			{
				return BadRequest("Max reservation duration must be greater than zero.");
			}

			bool updated = await _roomRepository.UpdateAsync(room);

			if (!updated)
			{
				return NotFound();
			}

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			bool deleted = await _roomRepository.DeleteAsync(id);

			if (!deleted)
			{
				return NotFound();
			}

			return NoContent();
		}
	}
}