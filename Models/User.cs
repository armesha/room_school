// Models/User.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace RoomReservationSystem.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
