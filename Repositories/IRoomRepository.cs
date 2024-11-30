// Repositories/IRoomRepository.cs
using RoomReservationSystem.Models;
using System.Collections.Generic;

namespace RoomReservationSystem.Repositories
{
    public interface IRoomRepository
    {
        IEnumerable<Room> GetAllRooms(int? limit = null, int? offset = null, int? buildingId = null);
        IEnumerable<Room> GetRandomRooms(int count);
        Room GetRoomById(int roomId);
        void AddRoom(Room room);
        void UpdateRoom(Room room);
        void DeleteRoom(int roomId);
        IEnumerable<DateTime> GetReservedDates(int roomId, DateTime startDate, DateTime endDate);
        IEnumerable<Booking> GetRoomReservations(int roomId, DateTime startDate, DateTime endDate);
    }
}
