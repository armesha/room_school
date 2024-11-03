// Repositories/IRoleRepository.cs
using RoomReservationSystem.Models;

namespace RoomReservationSystem.Repositories
{
    public interface IRoleRepository
    {
        Role GetRoleById(int roleId);
        Role GetRoleByName(string roleName);
    }
}
