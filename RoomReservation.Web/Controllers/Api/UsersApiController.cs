using Microsoft.AspNetCore.Mvc;
using RoomReservation.Data.Repositories;

namespace RoomReservation.Web.Controllers.Api
{
	[ApiController]
	[Route("api/users")]
	public class UsersApiController : ControllerBase
	{
		private readonly UserRepository _userRepository;

		public UsersApiController(UserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var users = await _userRepository.GetAllAsync();
			return Ok(users);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			bool deleted = await _userRepository.DeleteAsync(id);

			if (!deleted)
			{
				return NotFound();
			}
			return NoContent();
		}

	}
}
