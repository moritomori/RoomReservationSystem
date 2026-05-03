using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Web.ViewModels
{
	public class ReservationCreateViewModel
	{
		public int RoomId { get; set; }

		public string RoomName { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Start time")]
		public DateTime StartTime { get; set; } = DateTime.Now.AddDays(1);

		[Required]
		[Display(Name = "End time")]
		public DateTime EndTime { get; set; } = DateTime.Now.AddDays(1).AddHours(1);

		[Required]
		[Display(Name = "Purpose")]
		public string Purpose { get; set; } = string.Empty;

		[Required]
		[Range(1, 100)]
		[Display(Name = "Number of people")]
		public int NumberOfPeople { get; set; } = 1;
	}
}