using System.Security.Cryptography;
using System.Text;

namespace RoomReservation.Web.Services
{
	public class PasswordHasher
	{
		public string Hash(string password)
		{
			using var sha256 = SHA256.Create();

			byte[] bytes = Encoding.UTF8.GetBytes(password);
			byte[] hash = sha256.ComputeHash(bytes);

			return Convert.ToBase64String(hash);
		}

		public bool Verify(string password, string passwordHash)
		{
			string hash = Hash(password);
			return hash == passwordHash;
		}
	}
}