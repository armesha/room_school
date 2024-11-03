// Repositories/UserRepository.cs
using Oracle.ManagedDataAccess.Client;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using System;
using System.Data;

namespace RoomReservationSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public User GetUserByUsername(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT user_id, username, password_hash, email, role_id, registration_date FROM users WHERE username = :username";
            command.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2) { Value = username });

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = Convert.ToInt32(reader["user_id"]),
                    Username = reader["username"].ToString(),
                    PasswordHash = reader["password_hash"].ToString(),
                    Email = reader["email"].ToString(),
                    RoleId = Convert.ToInt32(reader["role_id"]),
                    RegistrationDate = Convert.ToDateTime(reader["registration_date"])
                };
            }
            return null;
        }

        public User GetUserById(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT user_id, username, password_hash, email, role_id, registration_date FROM users WHERE user_id = :user_id";
            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32) { Value = userId });

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = Convert.ToInt32(reader["user_id"]),
                    Username = reader["username"].ToString(),
                    PasswordHash = reader["password_hash"].ToString(),
                    Email = reader["email"].ToString(),
                    RoleId = Convert.ToInt32(reader["role_id"]),
                    RegistrationDate = Convert.ToDateTime(reader["registration_date"])
                };
            }
            return null;
        }

        public void AddUser(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO users 
                                    (user_id, username, password_hash, email, role_id, registration_date) 
                                    VALUES 
                                    (seq_users.NEXTVAL, :username, :password_hash, :email, :role_id, :registration_date)";
            command.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2) { Value = user.Username });
            command.Parameters.Add(new OracleParameter("password_hash", OracleDbType.Varchar2) { Value = user.PasswordHash });
            command.Parameters.Add(new OracleParameter("email", OracleDbType.Varchar2) { Value = user.Email });
            command.Parameters.Add(new OracleParameter("role_id", OracleDbType.Int32) { Value = user.RoleId });
            command.Parameters.Add(new OracleParameter("registration_date", OracleDbType.Date) { Value = user.RegistrationDate });

            command.ExecuteNonQuery();
        }
    }
}
