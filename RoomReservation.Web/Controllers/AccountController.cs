using Microsoft.AspNetCore.Mvc;
using RoomReservation.Data.Repositories;
using RoomReservation.Domain.Models;
using RoomReservation.Web.Services;
using RoomReservation.Web.ViewModels;

namespace RoomReservation.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserRepository _userRepository;
		private readonly PasswordHasher _passwordHasher;

		public AccountController(UserRepository userRepository, PasswordHasher passwordHasher)
		{
			_userRepository = userRepository;
			_passwordHasher = passwordHasher;
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View(new RegisterViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var existingUser = await _userRepository.GetByLoginAsync(model.Login);

			if (existingUser != null)
			{
				ModelState.AddModelError(nameof(model.Login), "User with this login already exists.");
				return View(model);
			}

			var user = new User
			{
				Login = model.Login,
				PasswordHash = _passwordHasher.Hash(model.Password),
				ProfileInfo = model.ProfileInfo
			};

			await _userRepository.CreateAsync(user);

			return RedirectToAction(nameof(Login));
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View(new LoginViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userRepository.GetByLoginAsync(model.Login);

			if (user == null || !_passwordHasher.Verify(model.Password, user.PasswordHash))
			{
				ModelState.AddModelError("", "Invalid login or password.");
				return View(model);
			}

			HttpContext.Session.SetInt32("UserId", user.Id);
			HttpContext.Session.SetString("Login", user.Login);

			return RedirectToAction("Index", "Rooms");
		}

		[HttpPost]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction(nameof(Login));
		}
	}
}