// Models/Room.cs
using System.ComponentModel.DataAnnotations;

namespace RoomReservationSystem.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        [Required(ErrorMessage = "BuildingId is required.")]
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "RoomNumber is required.")]
        [StringLength(20, ErrorMessage = "RoomNumber cannot exceed 20 characters.")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        public bool HasProjector { get; set; }
        public bool HasWhiteboard { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        // Made Image nullable by using byte[]? and removed [Required] attribute
        public byte[]? Image { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; } // Ensure Price property is present
    }
}
