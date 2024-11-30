// Repositories/RoomRepository.cs
using Oracle.ManagedDataAccess.Client;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;

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

        public IEnumerable<Room> GetAllRooms(int? limit = null, int? offset = null)
        {
            var rooms = new List<Room>();

            using var connection = _dbConnectionFactory.CreateConnection();
            using var command = connection.CreateCommand();
            
            var sql = "SELECT * FROM rooms";
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
                rooms.Add(new Room
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
                });
            }

            return rooms;
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
    }
}
