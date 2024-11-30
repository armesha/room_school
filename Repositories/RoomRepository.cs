// Repositories/RoomRepository.cs
using Oracle.ManagedDataAccess.Client;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RoomReservationSystem.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public RoomRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public void AddRoom(Room room)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "sp_add_room";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.Add(new OracleParameter("p_building_id", OracleDbType.Int32) { Value = room.BuildingId });
            command.Parameters.Add(new OracleParameter("p_room_number", OracleDbType.Varchar2) { Value = room.RoomNumber });
            command.Parameters.Add(new OracleParameter("p_capacity", OracleDbType.Int32) { Value = room.Capacity });
            command.Parameters.Add(new OracleParameter("p_has_projector", OracleDbType.Char, 1) { Value = room.HasProjector ? "Y" : "N" });
            command.Parameters.Add(new OracleParameter("p_has_whiteboard", OracleDbType.Char, 1) { Value = room.HasWhiteboard ? "Y" : "N" });
            command.Parameters.Add(new OracleParameter("p_description", OracleDbType.Varchar2, 500) { Value = room.Description ?? string.Empty });
            command.Parameters.Add(new OracleParameter("p_image", OracleDbType.Blob) { Value = room.Image ?? new byte[0] });
            command.Parameters.Add(new OracleParameter("p_price", OracleDbType.Decimal) { Value = room.Price }); // Added Price parameter

            connection.Open();
            command.ExecuteNonQuery();

            // Optionally, retrieve the generated RoomId if needed
            // This requires modifying the stored procedure to return the RoomId, possibly via an OUT parameter
        }

        public IEnumerable<Room> GetAllRooms(int? limit = null, int? offset = null, int? buildingId = null)
        {
            var rooms = new List<Room>();

            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            
            var sql = "SELECT * FROM rooms";
            if (buildingId.HasValue)
            {
                sql += " WHERE building_id = :buildingId";
            }
            sql += " ORDER BY building_id, room_number";
            
            if (offset.HasValue)
            {
                sql += " OFFSET :offset ROWS";
            }
            if (limit.HasValue)
            {
                sql += " FETCH NEXT :limit ROWS ONLY";
            }
            
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            if (buildingId.HasValue)
            {
                command.Parameters.Add(new OracleParameter("buildingId", OracleDbType.Int32) { Value = buildingId.Value });
            }
            if (offset.HasValue)
            {
                command.Parameters.Add(new OracleParameter("offset", OracleDbType.Int32) { Value = offset.Value });
            }
            if (limit.HasValue)
            {
                command.Parameters.Add(new OracleParameter("limit", OracleDbType.Int32) { Value = limit.Value });
            }

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                rooms.Add(MapRoomFromReader(reader));
            }

            return rooms;
        }

        public IEnumerable<Room> GetRandomRooms(int count)
        {
            var rooms = new List<Room>();

            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            
            command.CommandText = "SELECT * FROM (SELECT * FROM rooms ORDER BY DBMS_RANDOM.VALUE) WHERE ROWNUM <= :count";
            command.CommandType = CommandType.Text;
            command.Parameters.Add(new OracleParameter("count", OracleDbType.Int32) { Value = count });

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                rooms.Add(MapRoomFromReader(reader));
            }

            return rooms;
        }

        public IEnumerable<DateTime> GetReservedDates(int roomId, DateTime startDate, DateTime endDate)
        {
            var reservedDates = new List<DateTime>();

            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                SELECT DISTINCT TRUNC(start_time) as reserved_date
                FROM bookings
                WHERE room_id = :roomId 
                AND TRUNC(start_time) BETWEEN TRUNC(:startDate) AND TRUNC(:endDate)
                ORDER BY reserved_date";
            
            command.CommandType = CommandType.Text;
            command.Parameters.Add(new OracleParameter("roomId", OracleDbType.Int32) { Value = roomId });
            command.Parameters.Add(new OracleParameter("startDate", OracleDbType.Date) { Value = startDate });
            command.Parameters.Add(new OracleParameter("endDate", OracleDbType.Date) { Value = endDate });

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reservedDates.Add(Convert.ToDateTime(reader["reserved_date"]));
            }

            return reservedDates;
        }

        public Room GetRoomById(int roomId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM rooms WHERE room_id = :room_id";
            command.CommandType = CommandType.Text;

            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = roomId });

            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapRoomFromReader(reader);
            }

            return null;
        }

        public void UpdateRoom(Room room)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE rooms SET building_id = :building_id, room_number = :room_number, capacity = :capacity, has_projector = :has_projector, has_whiteboard = :has_whiteboard, description = :description, image = :image, price = :price WHERE room_id = :room_id";
            command.CommandType = CommandType.Text;

            command.Parameters.Add(new OracleParameter("building_id", OracleDbType.Int32) { Value = room.BuildingId });
            command.Parameters.Add(new OracleParameter("room_number", OracleDbType.Varchar2) { Value = room.RoomNumber });
            command.Parameters.Add(new OracleParameter("capacity", OracleDbType.Int32) { Value = room.Capacity });
            command.Parameters.Add(new OracleParameter("has_projector", OracleDbType.Char, 1) { Value = room.HasProjector ? "Y" : "N" });
            command.Parameters.Add(new OracleParameter("has_whiteboard", OracleDbType.Char, 1) { Value = room.HasWhiteboard ? "Y" : "N" });
            command.Parameters.Add(new OracleParameter("description", OracleDbType.Varchar2, 500) { Value = room.Description ?? string.Empty });
            command.Parameters.Add(new OracleParameter("image", OracleDbType.Blob) { Value = room.Image ?? new byte[0] });
            command.Parameters.Add(new OracleParameter("price", OracleDbType.Decimal) { Value = room.Price }); // Added Price parameter
            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = room.RoomId });

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void DeleteRoom(int roomId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM rooms WHERE room_id = :room_id";
            command.CommandType = CommandType.Text;

            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = roomId });

            connection.Open();
            command.ExecuteNonQuery();
        }

        public IEnumerable<Booking> GetRoomReservations(int roomId, DateTime startDate, DateTime endDate)
        {
            var bookings = new List<Booking>();

            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                SELECT booking_id, user_id, room_id, booking_date, start_time, end_time, status
                FROM bookings
                WHERE room_id = :roomId 
                AND booking_date BETWEEN TRUNC(:startDate) AND TRUNC(:endDate)
                ORDER BY booking_date, start_time";
            
            command.CommandType = CommandType.Text;
            command.Parameters.Add(new OracleParameter("roomId", OracleDbType.Int32) { Value = roomId });
            command.Parameters.Add(new OracleParameter("startDate", OracleDbType.Date) { Value = startDate });
            command.Parameters.Add(new OracleParameter("endDate", OracleDbType.Date) { Value = endDate });

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                bookings.Add(new Booking
                {
                    BookingId = Convert.ToInt32(reader["booking_id"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    RoomId = Convert.ToInt32(reader["room_id"]),
                    BookingDate = Convert.ToDateTime(reader["booking_date"]),
                    StartTime = Convert.ToDateTime(reader["start_time"]),
                    EndTime = Convert.ToDateTime(reader["end_time"]),
                    Status = reader["status"].ToString()
                });
            }

            return bookings;
        }

        private Room MapRoomFromReader(DbDataReader reader)
        {
            return new Room
            {
                RoomId = Convert.ToInt32(reader["room_id"]),
                BuildingId = Convert.ToInt32(reader["building_id"]),
                RoomNumber = reader["room_number"].ToString(),
                Capacity = Convert.ToInt32(reader["capacity"]),
                HasProjector = reader["has_projector"].ToString() == "Y",
                HasWhiteboard = reader["has_whiteboard"].ToString() == "Y",
                Description = reader["description"].ToString(),
                Image = reader["image"] as byte[],
                Price = Convert.ToDecimal(reader["price"])
            };
        }
    }
}
