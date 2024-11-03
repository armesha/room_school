// Models/User.cs
using System;

namespace RoomReservationSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Stored as hashed
        public string Email { get; set; }
        public int RoleId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
