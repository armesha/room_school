using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservationSystem.Models;
using RoomReservationSystem.Repositories;
using System.Collections.Generic;

namespace RoomReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Room>> GetAllRooms([FromQuery] int? limit = null, [FromQuery] int? offset = null, [FromQuery] int? buildingId = null)
        {
            // Для запросов без токена устанавливаем лимит
            if (!User.Identity.IsAuthenticated)
            {
                const int maxLimit = 10; // Максимальное количество комнат для публичного доступа
                if (!limit.HasValue || limit.Value > maxLimit)
                {
                    limit = maxLimit;
                }
            }

            var rooms = _roomRepository.GetAllRooms(limit, offset, buildingId);
            return Ok(new { list = rooms });
        }

        [HttpGet("random")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Room>> GetRandomRooms([FromQuery] int count = 3)
        {
            if (count <= 0 || count > 10)
            {
                return BadRequest(new { message = "Count must be between 1 and 10" });
            }

            var rooms = _roomRepository.GetRandomRooms(count);
            return Ok(new { list = rooms });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<Room> GetRoomById(int id)
        {
            var room = _roomRepository.GetRoomById(id);
            if (room == null)
                return NotFound(new { message = "Room not found." });

            return Ok(new { room });
        }

        [HttpGet("{roomId}/reservations")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Booking>> GetRoomReservations(int roomId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Today;
            if (!endDate.HasValue)
                endDate = startDate.Value.AddMonths(1);

            var reservations = _roomRepository.GetRoomReservations(roomId, startDate.Value, endDate.Value);
            return Ok(new { list = reservations });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult AddRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _roomRepository.AddRoom(room);
            return CreatedAtAction(nameof(GetRoomById), new { id = room.RoomId }, new { room });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult UpdateRoom(int id, [FromBody] Room room)
        {
            if (id != room.RoomId)
                return BadRequest(new { message = "ID mismatch." });

            var existingRoom = _roomRepository.GetRoomById(id);
            if (existingRoom == null)
                return NotFound(new { message = "Room not found." });

            _roomRepository.UpdateRoom(room);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteRoom(int id)
        {
            var existingRoom = _roomRepository.GetRoomById(id);
            _roomRepository.DeleteRoom(id);
            return Ok(new { success = true, message = existingRoom != null ? "Room deleted successfully." : "Room not found but operation was successful." });
        }
    }
}
