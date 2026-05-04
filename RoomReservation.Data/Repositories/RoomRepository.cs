using Dapper;
using RoomReservation.Data.Database;
using RoomReservation.Domain.Models;

namespace RoomReservation.Data.Repositories
{
	public class RoomRepository
	{
		private readonly DbConnectionFactory _connectionFactory;

		public RoomRepository(DbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public async Task<IEnumerable<Room>> GetAllAsync()
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT 
					id AS Id,
					name AS Name,
					capacity AS Capacity,
					equipment AS Equipment,
					max_reservation_duration_minutes AS MaxReservationDurationMinutes,
					created_at AS CreatedAt
				FROM rooms
				ORDER BY name;
				";

			return await connection.QueryAsync<Room>(sql);
		}

		public async Task<IEnumerable<Room>> GetAllSortedAsync(string? sort)
		{
			using var connection = _connectionFactory.CreateConnection();

			string orderBy = sort switch
			{
				"capacity" => "capacity DESC",
				"name" => "name ASC",
				_ => "id ASC"
			};

			string sql = $@"
				SELECT 
					id AS Id,
					name AS Name,
					capacity AS Capacity,
					equipment AS Equipment,
					max_reservation_duration_minutes AS MaxReservationDurationMinutes,
					created_at AS CreatedAt
				FROM rooms
				ORDER BY {orderBy};
				";

			return await connection.QueryAsync<Room>(sql);
		}

		public async Task<Room?> GetByIdAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT 
					id AS Id,
					name AS Name,
					capacity AS Capacity,
					equipment AS Equipment,
					max_reservation_duration_minutes AS MaxReservationDurationMinutes,
					created_at AS CreatedAt
				FROM rooms
				WHERE id = @Id;
				";

			return await connection.QueryFirstOrDefaultAsync<Room>(sql, new { Id = id });
		}

		public async Task<int> CreateAsync(Room room)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				INSERT INTO rooms 
					(name, capacity, equipment, max_reservation_duration_minutes)
				VALUES 
					(@Name, @Capacity, @Equipment, @MaxReservationDurationMinutes);

				SELECT last_insert_rowid();
				";

			return await connection.ExecuteScalarAsync<int>(sql, room);
		}

		public async Task<bool> UpdateAsync(Room room)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				UPDATE rooms
				SET 
					name = @Name,
					capacity = @Capacity,
					equipment = @Equipment,
					max_reservation_duration_minutes = @MaxReservationDurationMinutes
				WHERE id = @Id;
				";

			int affectedRows = await connection.ExecuteAsync(sql, room);
			return affectedRows > 0;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				DELETE FROM rooms
				WHERE id = @Id;
				";

			int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
			return affectedRows > 0;
		}
	}
}