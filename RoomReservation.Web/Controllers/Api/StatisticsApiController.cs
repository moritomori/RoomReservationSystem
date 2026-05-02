using Microsoft.AspNetCore.Mvc;
using RoomReservation.Data.Repositories;

namespace RoomReservation.Web.Controllers.Api
{
	[ApiController]
	[Route("api/statistics")]
	public class StatisticsApiController : ControllerBase
	{
		private readonly ReservationRepository _reservationRepository;

		public StatisticsApiController(ReservationRepository reservationRepository)
		{
			_reservationRepository = reservationRepository;
		}

		[HttpGet("rooms")]
		public async Task<IActionResult> GetRoomUsage([FromQuery] DateTime from, [FromQuery] DateTime to)
		{
			if (to <= from)
			{
				return BadRequest("Date 'to' must be later than date 'from'.");
			}

			var statistics = await _reservationRepository.GetRoomUsageStatisticsAsync(from, to);
			return Ok(statistics);
		}
	}
}