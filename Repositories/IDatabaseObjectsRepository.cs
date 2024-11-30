// Repositories/IDatabaseObjectsRepository.cs
using System.Collections.Generic;

namespace RoomReservationSystem.Repositories
{
    public interface IDatabaseObjectsRepository
    {
        IEnumerable<string> GetAllDatabaseObjects();
        IEnumerable<string> GetDatabaseObjectsByName(string name);
    }
}
