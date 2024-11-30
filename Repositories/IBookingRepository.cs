// Repositories/IBookingRepository.cs
using RoomReservationSystem.Models;
using System.Collections.Generic;

namespace RoomReservationSystem.Repositories
{
    public interface IBookingRepository
    {
        IEnumerable<Booking> GetAllBookings();
        IEnumerable<Booking> GetBookingsByUserId(int userId);
        Booking GetBookingById(int bookingId);
        void AddBooking(Booking booking);
        void UpdateBooking(Booking booking);
        void DeleteBooking(int bookingId);
    }
}
