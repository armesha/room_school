using System;

namespace RoomReservationSystem.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }  // Added BookingId property
        public decimal Amount { get; set; }
        public required string Status { get; set; } // e.g., "Paid", "Unpaid"
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
    }
}
