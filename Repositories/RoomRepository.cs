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
        private readonly IDbConnectionFactory _connectionFactory;

        public RoomRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Room> GetAllRooms()
        {
            var rooms = new List<Room>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT room_id, building_id, room_number, capacity, has_projector, has_whiteboard FROM rooms";

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
                    HasWhiteboard = reader["has_whiteboard"].ToString() == "Y"
                });
            }
            return rooms;
        }

        public Room GetRoomById(int roomId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT room_id, building_id, room_number, capacity, has_projector, has_whiteboard 
                                    FROM rooms WHERE room_id = :room_id";
            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = roomId });

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
                    HasWhiteboard = reader["has_whiteboard"].ToString() == "Y"
                };
            }
            return null;
        }

        public void AddRoom(Room room)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO rooms 
                                    (room_id, building_id, room_number, capacity, has_projector, has_whiteboard) 
                                    VALUES 
                                    (seq_rooms.NEXTVAL, :building_id, :room_number, :capacity, :has_projector, :has_whiteboard)";
            command.Parameters.Add(new OracleParameter("building_id", OracleDbType.Int32) { Value = room.BuildingId });
            command.Parameters.Add(new OracleParameter("room_number", OracleDbType.Varchar2) { Value = room.RoomNumber });
            command.Parameters.Add(new OracleParameter("capacity", OracleDbType.Int32) { Value = room.Capacity });
            command.Parameters.Add(new OracleParameter("has_projector", OracleDbType.Char) { Value = room.HasProjector ? "Y" : "N" });
            command.Parameters.Add(new OracleParameter("has_whiteboard", OracleDbType.Char) { Value = room.HasWhiteboard ? "Y" : "N" });

            command.ExecuteNonQuery();
        }

        public void UpdateRoom(Room room)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE rooms 
                                    SET building_id = :building_id,
                                        room_number = :room_number,
                                        capacity = :capacity,
                                        has_projector = :has_projector,
                                        has_whiteboard = :has_whiteboard
                                    WHERE room_id = :room_id";
            command.Parameters.Add(new OracleParameter("building_id", OracleDbType.Int32) { Value = room.BuildingId });
            command.Parameters.Add(new OracleParameter("room_number", OracleDbType.Varchar2) { Value = room.RoomNumber });
            command.Parameters.Add(new OracleParameter("capacity", OracleDbType.Int32) { Value = room.Capacity });
            command.Parameters.Add(new OracleParameter("has_projector", OracleDbType.Char) { Value = room.HasProjector ? "Y" : "N" });
            command.Parameters.Add(new OracleParameter("has_whiteboard", OracleDbType.Char) { Value = room.HasWhiteboard ? "Y" : "N" });
            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = room.RoomId });

            command.ExecuteNonQuery();
        }

        public void DeleteRoom(int roomId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM rooms WHERE room_id = :room_id";
            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = roomId });

            command.ExecuteNonQuery();
        }
    }
}
