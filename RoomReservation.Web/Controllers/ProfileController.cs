using Microsoft.AspNetCore.Mvc;
using RoomReservation.Data.Repositories;

namespace RoomReservation.Web.Controllers
{
	public class ProfileController : Controller
	{
		private readonly UserRepository _userRepository;

		public ProfileController(UserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<IActionResult> Index()
		{
			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var user = await _userRepository.GetByIdAsync(userId.Value);

			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}

			return View(user);
		}

		[HttpGet]
		public async Task<IActionResult> GetProfileInfo()
		{
			int? userId = HttpContext.Session.GetInt32("UserId");
			if(userId == null)
			{
				return Unauthorized();
			}

			var user = await _userRepository.GetByIdAsync(userId.Value);
			if (user == null)
			{
				return NotFound();
			}
			return Json(new
			{
				login = user.Login,
				profileInfo = user.ProfileInfo,
				createdAt = user.CreatedAt
			});
		}
	}
}
