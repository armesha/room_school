using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservationSystem.Models;
using RoomReservationSystem.Services;
using System.Collections.Generic;

namespace RoomReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingService _buildingService;

        public BuildingsController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        // GET: /api/buildings
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Building>> GetAllBuildings([FromQuery] int? limit = null, [FromQuery] int? offset = null)
        {
            // Для неавторизованных пользователей устанавливаем лимит
            if (!User.Identity.IsAuthenticated)
            {
                const int maxLimit = 10; // Максимальное количество зданий для публичного доступа
                if (!limit.HasValue || limit.Value > maxLimit)
                {
                    limit = maxLimit;
                }
            }

            var buildings = _buildingService.GetAllBuildings(limit, offset);
            return Ok(new { list = buildings });
        }

        // GET: /api/buildings/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<Building> GetBuildingById(int id)
        {
            var building = _buildingService.GetBuildingById(id);
            if (building == null)
            {
                return NotFound(new { message = "Building not found." });
            }

            return Ok(new { building });
        }
    }
}
