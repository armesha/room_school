// Repositories/IBuildingRepository.cs
using RoomReservationSystem.Models;
using System.Collections.Generic;

namespace RoomReservationSystem.Repositories
{
    public interface IBuildingRepository
    {
        IEnumerable<Building> GetAllBuildings(int? limit = null, int? offset = null);
        Building GetBuildingById(int buildingId);
        void AddBuilding(Building building);
        void UpdateBuilding(Building building);
        void DeleteBuilding(int buildingId);
    }
}
