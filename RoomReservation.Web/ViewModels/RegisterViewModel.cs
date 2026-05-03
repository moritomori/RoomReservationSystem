using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Web.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "Login")]
		public string Login { get; set; } = string.Empty;

		[Required]
		[MinLength(4)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; } = string.Empty;

		[Display(Name = "Profile info")]
		public string? ProfileInfo { get; set; }
	}
}
