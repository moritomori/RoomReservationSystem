using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Web.ViewModels
{
	public class ReservationEditViewModel
	{
		public int Id { get; set; }

		public int RoomId { get; set; }

		public string RoomName { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Start time")]
		public DateTime StartTime { get; set; }

		[Required]
		[Display(Name = "End time")]
		public DateTime EndTime { get; set; }

		[Required]
		[Display(Name = "Purpose")]
		public string Purpose { get; set; } = string.Empty;

		[Required]
		[Range(1, 100)]
		[Display(Name = "Number of people")]
		public int NumberOfPeople { get; set; }
	}
}