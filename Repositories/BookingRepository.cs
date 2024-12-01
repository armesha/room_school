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
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                INSERT INTO bookings 
                    (booking_id, user_id, room_id, booking_date, start_time, end_time, status) 
                VALUES 
                    (seq_bookings.NEXTVAL, :user_id, :room_id, :booking_date, :start_time, :end_time, :status)
                RETURNING booking_id INTO :booking_id";

            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32) { Value = booking.UserId });
            command.Parameters.Add(new OracleParameter("room_id", OracleDbType.Int32) { Value = booking.RoomId });
            command.Parameters.Add(new OracleParameter("booking_date", OracleDbType.Date) { Value = booking.BookingDate });
            command.Parameters.Add(new OracleParameter("start_time", OracleDbType.Date) { Value = booking.StartTime });
            command.Parameters.Add(new OracleParameter("end_time", OracleDbType.Date) { Value = booking.EndTime });
            command.Parameters.Add(new OracleParameter("status", OracleDbType.Varchar2) { Value = booking.Status });

            var bookingIdParam = new OracleParameter("booking_id", OracleDbType.Int32);
            bookingIdParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(bookingIdParam);

            try
            {
                command.ExecuteNonQuery();
                
                // Convert OracleDecimal to int using ToInt32 method
                var bookingIdOracleDecimal = (Oracle.ManagedDataAccess.Types.OracleDecimal)bookingIdParam.Value;
                booking.BookingId = bookingIdOracleDecimal.ToInt32();
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error executing booking insert: {ex.Message}");
                throw;
            }

            // Create invoice in a separate command
            using var invoiceCommand = connection.CreateCommand();
            invoiceCommand.CommandText = "BEGIN sp_create_invoice(:p_booking_id); END;";
            invoiceCommand.Parameters.Add(new OracleParameter("p_booking_id", OracleDbType.Int32) { Value = booking.BookingId });
            invoiceCommand.ExecuteNonQuery();
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

        public IEnumerable<Invoice> GetUnpaidInvoices()
        {
            var invoices = new List<Invoice>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT i.invoice_id, i.booking_id, i.amount, i.invoice_date, b.user_id,
                                    CASE WHEN i.is_paid = 1 THEN 'Paid' ELSE 'Unpaid' END as status
                                    FROM invoices i
                                    JOIN bookings b ON i.booking_id = b.booking_id
                                    WHERE i.is_paid = 0";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                invoices.Add(new Invoice
                {
                    InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                    BookingId = Convert.ToInt32(reader["booking_id"]),
                    Amount = Convert.ToDecimal(reader["amount"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    DueDate = Convert.ToDateTime(reader["invoice_date"]),
                    CreatedAt = Convert.ToDateTime(reader["invoice_date"]),
                    Status = reader["status"].ToString()
                });
            }
            return invoices;
        }

        public IEnumerable<Invoice> GetUserInvoices(int userId)
        {
            var invoices = new List<Invoice>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT 
                                i.invoice_id, 
                                i.booking_id,
                                i.amount,
                                i.invoice_date,
                                b.user_id,
                                i.invoice_date as created_at,
                                CASE WHEN i.is_paid = 1 THEN 'Paid' ELSE 'Unpaid' END as status
                            FROM invoices i
                            JOIN bookings b ON i.booking_id = b.booking_id
                            WHERE b.user_id = :user_id";
            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32) { Value = userId });

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                invoices.Add(new Invoice
                {
                    InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                    BookingId = Convert.ToInt32(reader["booking_id"]),
                    Amount = Convert.ToDecimal(reader["amount"]),
                    Status = reader["status"].ToString(),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    CreatedAt = Convert.ToDateTime(reader["created_at"]),
                    DueDate = Convert.ToDateTime(reader["invoice_date"])
                });
            }
            return invoices;
        }

        public bool MarkInvoiceAsPaid(int invoiceId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE invoices 
                                   SET is_paid = 1 
                                   WHERE invoice_id = :invoice_id";
            command.Parameters.Add(new OracleParameter("invoice_id", OracleDbType.Int32) { Value = invoiceId });

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}
