// Repositories/BookingRepository.cs
using Oracle.ManagedDataAccess.Client;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomReservationSystem.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BookingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Booking> GetAllBookings()
        {
            var bookings = new List<Booking>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT booking_id, user_id, room_id, booking_date, start_time, end_time, status 
                                    FROM bookings";

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

        public IEnumerable<Booking> GetBookingsByUserId(int userId)
        {
            var bookings = new List<Booking>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT booking_id, user_id, room_id, booking_date, start_time, end_time, status 
                                    FROM bookings 
                                    WHERE user_id = :user_id";
            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32) { Value = userId });

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

        public Booking GetBookingById(int bookingId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT booking_id, user_id, room_id, booking_date, start_time, end_time, status 
                                    FROM bookings 
                                    WHERE booking_id = :booking_id";
            command.Parameters.Add(new OracleParameter("booking_id", OracleDbType.Int32) { Value = bookingId });

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Booking
                {
                    BookingId = Convert.ToInt32(reader["booking_id"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    RoomId = Convert.ToInt32(reader["room_id"]),
                    BookingDate = Convert.ToDateTime(reader["booking_date"]),
                    StartTime = Convert.ToDateTime(reader["start_time"]),
                    EndTime = Convert.ToDateTime(reader["end_time"]),
                    Status = reader["status"].ToString()
                };
            }
            return null;
        }

        public void AddBooking(Booking booking)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO bookings 
                                    (booking_id, user_id, room_id, booking_date, start_time, end_time, status) 
                                    VALUES 
                                    (seq_bookings.NEXTVAL, :user_id, :room_id, :booking_date, :start_time, :end_time, :status)";
            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32) { Value = booking.UserId });
            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = booking.RoomId });
            command.Parameters.Add(new OracleParameter("booking_date", OracleDbType.Date) { Value = booking.BookingDate });
            command.Parameters.Add(new OracleParameter("start_time", OracleDbType.Date) { Value = booking.StartTime });
            command.Parameters.Add(new OracleParameter("end_time", OracleDbType.Date) { Value = booking.EndTime });
            command.Parameters.Add(new OracleParameter("status", OracleDbType.Varchar2) { Value = booking.Status });

            command.ExecuteNonQuery();
        }

        public void UpdateBooking(Booking booking)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE bookings 
                                    SET room_id = :room_id,
                                        booking_date = :booking_date,
                                        start_time = :start_time,
                                        end_time = :end_time,
                                        status = :status,
                                        user_id = :user_id
                                    WHERE booking_id = :booking_id";
            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = booking.RoomId });
            command.Parameters.Add(new OracleParameter("booking_date", OracleDbType.Date) { Value = booking.BookingDate });
            command.Parameters.Add(new OracleParameter("start_time", OracleDbType.Date) { Value = booking.StartTime });
            command.Parameters.Add(new OracleParameter("end_time", OracleDbType.Date) { Value = booking.EndTime });
            command.Parameters.Add(new OracleParameter("status", OracleDbType.Varchar2) { Value = booking.Status });
            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32) { Value = booking.UserId });
            command.Parameters.Add(new OracleParameter("booking_id", OracleDbType.Int32) { Value = booking.BookingId });

            command.ExecuteNonQuery();
        }

        public void DeleteBooking(int bookingId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM bookings WHERE booking_id = :booking_id";
            command.Parameters.Add(new OracleParameter("booking_id", OracleDbType.Int32) { Value = bookingId });

            command.ExecuteNonQuery();
        }
    }
}
