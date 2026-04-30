using Microsoft.AspNetCore.Mvc;
using RoomReservation.Common.DTOs;
using RoomReservation.Data.Repositories;
using RoomReservation.Domain.Models;
using RoomReservation.Web.Services;

namespace RoomReservation.Web.Controllers.Api
{
	[ApiController]
	[Route("api/auth")]
	public class AuthApiController : ControllerBase
	{
		private readonly UserRepository _userRepository;
		private readonly PasswordHasher _passwordHasher;
		private readonly IConfiguration _configuration;

		public AuthApiController(
			UserRepository userRepository,
			PasswordHasher passwordHasher,
			IConfiguration configuration)
		{
			_userRepository = userRepository;
			_passwordHasher = passwordHasher;
			_configuration = configuration;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Login))
			{
				return BadRequest("Login is required.");
			}

			if (string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest("Password is required.");
			}

			if (request.Password.Length < 4)
			{
				return BadRequest("Password must have at least 4 characters.");
			}

			var existingUser = await _userRepository.GetByLoginAsync(request.Login);

			if (existingUser != null)
			{
				return BadRequest("User with this login already exists.");
			}

			var user = new User
			{
				Login = request.Login,
				PasswordHash = _passwordHasher.Hash(request.Password),
				ProfileInfo = request.ProfileInfo
			};

			int newUserId = await _userRepository.CreateAsync(user);

			return Ok(new
			{
				id = newUserId,
				login = user.Login,
				message = "User registered successfully."
			});
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Login))
			{
				return BadRequest("Login is required.");
			}

			if (string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest("Password is required.");
			}

			var user = await _userRepository.GetByLoginAsync(request.Login);

			if (user == null)
			{
				return Unauthorized("Invalid login or password.");
			}

			bool passwordIsValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

			if (!passwordIsValid)
			{
				return Unauthorized("Invalid login or password.");
			}

			string token = _configuration["ApiSettings:Token"] ?? string.Empty;

			var response = new LoginResponse
			{
				UserId = user.Id,
				Login = user.Login,
				Token = token
			};

			return Ok(response);
		}
	}
}               