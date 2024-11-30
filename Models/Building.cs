using System.ComponentModel.DataAnnotations;

namespace RoomReservationSystem.Models
{
    public class Building
    {
        public int BuildingId { get; set; }

        [Required]
        public string BuildingName { get; set; }

        [Required]
        public string Address { get; set; }

        public string Description { get; set; }

        // Make Image nullable by adding '?'
        public byte[]? Image { get; set; } // Image is optional
    }
}
