// Repositories/DatabaseObjectsRepository.cs
using Oracle.ManagedDataAccess.Client;
using RoomReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomReservationSystem.Repositories
{
    public class DatabaseObjectsRepository : IDatabaseObjectsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DatabaseObjectsRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<string> GetAllDatabaseObjects()
        {
            var objects = new List<string>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT object_name FROM user_objects ORDER BY object_type, object_name";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                objects.Add(reader["object_name"].ToString());
            }
            return objects;
        }

        public IEnumerable<string> GetDatabaseObjectsByName(string name)
        {
            var objects = new List<string>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT object_name FROM user_objects 
                                    WHERE object_name LIKE :name 
                                    ORDER BY object_type, object_name";
            command.Parameters.Add(new OracleParameter("name", OracleDbType.Varchar2) { Value = $"%{name}%" });

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                objects.Add(reader["object_name"].ToString());
            }
            return objects;
        }
    }
}
