namespace RoomReservation.Common.DTOs
{
	public class ApiEndpointDescription
	{
		public string Controller { get; set; } = string.Empty;

		public string Action { get; set; } = string.Empty;

		public string HttpMethod { get; set; } = string.Empty;

		public string Route { get; set; } = string.Empty;

		public string ReturnType { get; set; } = string.Empty;

		public List<string> Parameters { get; set; } = new();
	}
}