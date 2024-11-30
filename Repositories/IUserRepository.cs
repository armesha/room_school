// Repositories/IUserRepository.cs
using RoomReservationSystem.Models;
using System.Collections.Generic;

namespace RoomReservationSystem.Repositories
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        User GetUserById(int userId);
        IEnumerable<User> GetAllUsers();
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int userId);
    }
}
