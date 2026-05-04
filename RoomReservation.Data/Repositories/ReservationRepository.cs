using Dapper;
using RoomReservation.Common;
using RoomReservation.Common.DTOs;
using RoomReservation.Data.Database;
using RoomReservation.Domain.Models;

namespace RoomReservation.Data.Repositories
{
	public class ReservationRepository
	{
		private readonly DbConnectionFactory _connectionFactory;

		public ReservationRepository(DbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public async Task<IEnumerable<Reservation>> GetAllAsync()
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					id AS Id,
					start_time AS StartTime,
					end_time AS EndTime,
					purpose AS Purpose,
					number_of_people AS NumberOfPeople,
					user_id AS UserId,
					room_id AS RoomId,
					status AS Status,
					created_at AS CreatedAt
				FROM reservations
				ORDER BY start_time DESC;
				";

			return await connection.QueryAsync<Reservation>(sql);
		}

		public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					id AS Id,
					start_time AS StartTime,
					end_time AS EndTime,
					purpose AS Purpose,
					number_of_people AS NumberOfPeople,
					user_id AS UserId,
					room_id AS RoomId,
					status AS Status,
					created_at AS CreatedAt
				FROM reservations
				WHERE user_id = @UserId
				ORDER BY start_time DESC;
				";

			return await connection.QueryAsync<Reservation>(sql, new { UserId = userId });
		}

		public async Task<Reservation?> GetByIdAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					id AS Id,
					start_time AS StartTime,
					end_time AS EndTime,
					purpose AS Purpose,
					number_of_people AS NumberOfPeople,
					user_id AS UserId,
					room_id AS RoomId,
					status AS Status,
					created_at AS CreatedAt
				FROM reservations
				WHERE id = @Id;
				";

			return await connection.QueryFirstOrDefaultAsync<Reservation>(sql, new { Id = id });
		}

		public async Task<bool> HasCollisionAsync(int roomId, DateTime startTime, DateTime endTime, int? ignoredReservationId = null)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT COUNT(*)
				FROM reservations
				WHERE room_id = @RoomId
				  AND status = @ActiveStatus
				  AND (@IgnoredReservationId IS NULL OR id != @IgnoredReservationId)
				  AND start_time < @EndTime
				  AND end_time > @StartTime;
				";

			int count = await connection.ExecuteScalarAsync<int>(sql, new
			{
				RoomId = roomId,
				StartTime = startTime,
				EndTime = endTime,
				IgnoredReservationId = ignoredReservationId,
				ActiveStatus = (int)Status.Active
			});

			return count > 0;
		}

		public async Task<int> CreateAsync(Reservation reservation)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				INSERT INTO reservations
					(start_time, end_time, purpose, number_of_people, user_id, room_id, status)
				VALUES
					(@StartTime, @EndTime, @Purpose, @NumberOfPeople, @UserId, @RoomId, @Status);

				SELECT last_insert_rowid();
				";

			return await connection.ExecuteScalarAsync<int>(sql, reservation);
		}

		public async Task<bool> UpdateAsync(Reservation reservation)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				UPDATE reservations
				SET
					start_time = @StartTime,
					end_time = @EndTime,
					purpose = @Purpose,
					number_of_people = @NumberOfPeople
				WHERE id = @Id;
				";

			int affectedRows = await connection.ExecuteAsync(sql, reservation);
			return affectedRows > 0;
		}

		public async Task<bool> CancelAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				UPDATE reservations
				SET status = @CancelledStatus
				WHERE id = @Id;
				";

			int affectedRows = await connection.ExecuteAsync(sql, new
			{
				Id = id,
				CancelledStatus = (int)Status.Cancelled
			});

			return affectedRows > 0;
		}

		public async Task AddHistoryAsync(int reservationId, string fieldName, string? oldValue, string? newValue)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				INSERT INTO reservation_history
					(reservation_id, field_name, old_value, new_value)
				VALUES
					(@ReservationId, @FieldName, @OldValue, @NewValue);
				";

			await connection.ExecuteAsync(sql, new
			{
				ReservationId = reservationId,
				FieldName = fieldName,
				OldValue = oldValue,
				NewValue = newValue
			});
		}

		public async Task<IEnumerable<ReservationHistory>> GetHistoryAsync(int reservationId)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					id AS Id,
					reservation_id AS ReservationId,
					field_name AS FieldName,
					old_value AS OldValue,
					new_value AS NewValue,
					changed_at AS ChangedAt
				FROM reservation_history
				WHERE reservation_id = @ReservationId
				ORDER BY changed_at DESC;
				";

			return await connection.QueryAsync<ReservationHistory>(sql, new
			{
				ReservationId = reservationId
			});
		}

		public async Task<IEnumerable<RoomUsageStatisticDto>> GetRoomUsageStatisticsAsync(DateTime from, DateTime to)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					r.id AS RoomId,
					r.name AS RoomName,
					COUNT(res.id) AS ReservationCount,
					COALESCE(SUM((julianday(res.end_time) - julianday(res.start_time)) * 24.0), 0.0) AS ReservedHours				FROM rooms r
				LEFT JOIN reservations res ON r.id = res.room_id
					AND res.status = @ActiveStatus
					AND res.start_time >= @From
					AND res.end_time <= @To
				GROUP BY r.id, r.name
				ORDER BY ReservedHours DESC;
				";

			return await connection.QueryAsync<RoomUsageStatisticDto>(sql, new
			{
				From = from,
				To = to,
				ActiveStatus = (int)Status.Active
			});
		}
		public async Task<IEnumerable<ReservationListItemDto>> GetListByUserIdAsync(int userId)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				SELECT
					res.id AS Id,
					r.name AS RoomName,
					res.start_time AS StartTime,
					res.end_time AS EndTime,
					res.purpose AS Purpose,
					res.number_of_people AS NumberOfPeople,
					res.status AS Status
				FROM reservations res
				JOIN rooms r ON res.room_id = r.id
				WHERE res.user_id = @UserId
				ORDER BY res.start_time DESC;
				";

			return await connection.QueryAsync<ReservationListItemDto>(sql, new { UserId = userId });
		}

		public async Task<bool> DeleteAsync(int id)
		{
			using var connection = _connectionFactory.CreateConnection();

			string sql = @"
				DELETE FROM reservations
				WHERE id = @Id;
				";

			int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });

			return affectedRows > 0;
		}
	}

}