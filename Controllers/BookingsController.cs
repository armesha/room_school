using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservationSystem.Models;
using RoomReservationSystem.Services;
using System.Security.Claims;
using System.Collections.Generic;

namespace RoomReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: /api/bookings
        [HttpGet]
        [Authorize(Roles = "Administrator,Registered User")]
        public ActionResult<IEnumerable<Booking>> GetAllBookings()
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user ID." });
            }

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Administrator")
            {
                var allBookings = _bookingService.GetAllBookingsForAdmin();
                return Ok(new { list = allBookings });
            }
            else
            {
                var userBookings = _bookingService.GetAllBookingsForUser(userId);
                return Ok(new { list = userBookings });
            }
        }

        // GET: /api/bookings/all
        [HttpGet("all")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<IEnumerable<Booking>> GetAllBookingsForAdmin()
        {
            var allBookings = _bookingService.GetAllBookingsForAdmin();
            return Ok(new { list = allBookings });
        }

        // GET: /api/bookings/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Registered User")]
        public ActionResult<Booking> GetBookingById(int id)
        {
            var booking = _bookingService.GetBookingById(id);
            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            var userIdClaim = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user ID." });
            }

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Administrator" && booking.UserId != userId)
                return Forbid();

            return Ok(new { booking });
        }

        // POST: /api/bookings
        [HttpPost]
        [Authorize(Roles = "Administrator,Registered User")]
        public IActionResult AddBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user ID." });
            }

            booking.UserId = userId;
            if (string.IsNullOrEmpty(booking.Status))
            {
                booking.Status = "Pending";
            }

            _bookingService.AddBooking(booking);
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, new { booking });
        }

        // PUT: /api/bookings/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Registered User")]
        public IActionResult UpdateBooking(int id, [FromBody] Booking booking)
        {
            if (id != booking.BookingId)
                return BadRequest(new { message = "ID mismatch." });

            var existingBooking = _bookingService.GetBookingById(id);
            if (existingBooking == null)
                return NotFound(new { message = "Booking not found." });

            var userIdClaim = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user ID." });
            }

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Administrator" && existingBooking.UserId != userId)
                return Forbid();

            // Ensure the UserId remains unchanged
            booking.UserId = existingBooking.UserId;

            _bookingService.UpdateBooking(booking);
            return NoContent();
        }

        // DELETE: /api/bookings/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,Registered User")]
        public IActionResult DeleteBooking(int id)
        {
            var existingBooking = _bookingService.GetBookingById(id);
            if (existingBooking == null)
                return NotFound(new { message = "Booking not found." });

            var userIdClaim = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user ID." });
            }

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Administrator" && existingBooking.UserId != userId)
                return Forbid();

            _bookingService.DeleteBooking(id);
            return NoContent();
        }

        // GET: /api/bookings/admin/invoices/unpaid
        [HttpGet("admin/invoices/unpaid")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<IEnumerable<Invoice>> GetUnpaidInvoices()
        {
            var unpaidInvoices = _bookingService.GetUnpaidInvoices(); 
            return Ok(new { list = unpaidInvoices });
        }

        // GET: /api/bookings/user/invoices
        [HttpGet("user/invoices")]
        [Authorize(Roles = "Administrator,Registered User")]
        public ActionResult<IEnumerable<Invoice>> GetUserInvoices()
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user ID." });
            }

            var userInvoices = _bookingService.GetUserInvoices(userId); 
            return Ok(new { list = userInvoices });
        }

        // POST: /api/bookings/admin/invoices/{id}/mark-paid
        [HttpPost("admin/invoices/{id}/mark-paid")]
        [Authorize(Roles = "Administrator")]
        public ActionResult MarkInvoiceAsPaid(int id)
        {
            var success = _bookingService.MarkInvoiceAsPaid(id);
            if (!success)
            {
                return NotFound(new { message = "Invoice not found or already paid." });
            }

            return Ok(new { message = "Invoice marked as paid successfully." });
        }
    }
}
