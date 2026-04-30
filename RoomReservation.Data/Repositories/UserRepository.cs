using Dapper;
using RoomReservation.Data.Database;
using RoomReservation.Domain.Models;

namespace RoomReservation.Data.Repositories
{
	public class UserRepository
	{
		private readonly DbConnectionFactory _connectionFactory;

		public UserRepository(DbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public async Task<IEnumerable<User>> GetAllAsync()
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					id AS Id,
					login AS Login,
					password_hash AS PasswordHash,
					profile_info AS ProfileInfo,
					created_at AS CreatedAt
				FROM users
				ORDER BY login;
				";

			return await connection.QueryAsync<User>(sql);
		}

		public async Task<User?> GetByIdAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					id AS Id,
					login AS Login,
					password_hash AS PasswordHash,
					profile_info AS ProfileInfo,
					created_at AS CreatedAt
				FROM users
				WHERE id = @Id;
				";

			return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
		}

		public async Task<User?> GetByLoginAsync(string login)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					id AS Id,
					login AS Login,
					password_hash AS PasswordHash,
					profile_info AS ProfileInfo,
					created_at AS CreatedAt
				FROM users
				WHERE login = @Login;
				";

			return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Login = login });
		}

		public async Task<int> CreateAsync(User user)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				INSERT INTO users
					(login, password_hash, profile_info)
				VALUES
					(@Login, @PasswordHash, @ProfileInfo);

				SELECT last_insert_rowid();
				";

			return await connection.ExecuteScalarAsync<int>(sql, user);
		}

		public async Task<bool> UpdateProfileAsync(int id, string? profileInfo)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				UPDATE users
				SET profile_info = @ProfileInfo
				WHERE id = @Id;
				";

			int affectedRows = await connection.ExecuteAsync(sql, new
			{
				Id = id,
				ProfileInfo = profileInfo
			});

			return affectedRows > 0;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				DELETE FROM users
				WHERE id = @Id;
				";

			int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
			return affectedRows > 0;
		}
	}
}