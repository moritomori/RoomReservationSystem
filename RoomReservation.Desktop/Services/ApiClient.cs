using System.Net.Http;
using System.Net.Http.Json;
using RoomReservation.Domain.Models;

namespace RoomReservation.Desktop.Services
{
	public class ApiClient
	{
		private readonly HttpClient _httpClient;

		public ApiClient()
		{
			_httpClient = new HttpClient
			{
				BaseAddress = new Uri("http://localhost:5244/")
			};

			_httpClient.DefaultRequestHeaders.Add("X-Api-Token", "secret-token-123");
		}

		public async Task<List<Room>> GetRoomsAsync()
		{
			return await _httpClient.GetFromJsonAsync<List<Room>>("api/rooms") ?? new List<Room>();
		}

		public async Task<List<Reservation>> GetReservationsAsync()
		{
			return await _httpClient.GetFromJsonAsync<List<Reservation>>("api/reservations") ?? new List<Reservation>();
		}

		public async Task<List<User>> GetUsersAsync()
		{
			return await _httpClient.GetFromJsonAsync<List<User>>("api/users") ?? new List<User>();
		}

		public async Task DeleteUserAsync(int id)
		{
			var response = await _httpClient.DeleteAsync($"api/users/{id}");
			response.EnsureSuccessStatusCode();
		}

		public async Task DeleteRoomAsync(int id)
		{
			var response = await _httpClient.DeleteAsync($"api/rooms/{id}");
			response.EnsureSuccessStatusCode();
		}

		public async Task CreateRoomAsync(Room room)
		{
			var response = await _httpClient.PostAsJsonAsync("api/rooms", room);
			response.EnsureSuccessStatusCode();
		}

		public async Task CancelReservationAsync(int id)
		{
			var response = await _httpClient.PostAsync($"api/reservations/{id}/cancel", null);
			response.EnsureSuccessStatusCode();
		}
	}
}