using Microsoft.Data.Sqlite;
using System.Data;

namespace RoomReservation.Data.Database
{
	public class DbConnectionFactory
	{
		private readonly string _connectionString;

		public DbConnectionFactory(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IDbConnection CreateConnection()
		{
			return new SqliteConnection(_connectionString);
		}
	}
}