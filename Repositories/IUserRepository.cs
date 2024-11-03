// Repositories/IUserRepository.cs
using RoomReservationSystem.Models;

namespace RoomReservationSystem.Repositories
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        User GetUserById(int userId);
        void AddUser(User user);
        // Additional methods as needed
    }
}
