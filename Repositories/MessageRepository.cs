// Repositories/MessageRepository.cs
using Oracle.ManagedDataAccess.Client;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomReservationSystem.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MessageRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Message> GetAllMessages()
        {
            var messages = new List<Message>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT message_id, sender_id, receiver_id, subject, body, sent_at 
                                    FROM messages";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                messages.Add(new Message
                {
                    MessageId = Convert.ToInt32(reader["message_id"]),
                    SenderId = Convert.ToInt32(reader["sender_id"]),
                    ReceiverId = Convert.ToInt32(reader["receiver_id"]),
                    Subject = reader["subject"].ToString(),
                    Body = reader["body"].ToString(),
                    SentAt = Convert.ToDateTime(reader["sent_at"]) // Adjusted
                });
            }
            return messages;
        }

        public IEnumerable<Message> GetMessagesByUserId(int userId)
        {
            var messages = new List<Message>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT message_id, sender_id, receiver_id, subject, body, sent_at 
                                    FROM messages 
                                    WHERE sender_id = :user_id OR receiver_id = :user_id";
            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32) { Value = userId });

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                messages.Add(new Message
                {
                    MessageId = Convert.ToInt32(reader["message_id"]),
                    SenderId = Convert.ToInt32(reader["sender_id"]),
                    ReceiverId = Convert.ToInt32(reader["receiver_id"]),
                    Subject = reader["subject"].ToString(),
                    Body = reader["body"].ToString(),
                    SentAt = Convert.ToDateTime(reader["sent_at"]) // Adjusted
                });
            }
            return messages;
        }

        public void AddMessage(Message message)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO messages 
                                    (message_id, sender_id, receiver_id, subject, body, sent_at) 
                                    VALUES 
                                    (seq_messages.NEXTVAL, :sender_id, :receiver_id, :subject, :body, :sent_at)";
            command.Parameters.Add(new OracleParameter("sender_id", OracleDbType.Int32) { Value = message.SenderId });
            command.Parameters.Add(new OracleParameter("receiver_id", OracleDbType.Int32) { Value = message.ReceiverId });
            command.Parameters.Add(new OracleParameter("subject", OracleDbType.Varchar2) { Value = message.Subject });
            command.Parameters.Add(new OracleParameter("body", OracleDbType.Varchar2) { Value = message.Body });
            command.Parameters.Add(new OracleParameter("sent_at", OracleDbType.Date) { Value = message.SentAt }); // Adjusted

            command.ExecuteNonQuery();
        }
    }
}
