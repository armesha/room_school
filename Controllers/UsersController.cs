// Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservationSystem.Models;
using RoomReservationSystem.Repositories;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;

namespace RoomReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UsersController(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        // GET: /api/users
        [HttpGet]
        public ActionResult<IEnumerable<UserResponse>> GetAllUsers()
        {
            var users = _userRepository.GetAllUsers();
            var roles = _roleRepository.GetAllRoles();

            var userResponses = from user in users
                                join role in roles on user.RoleId equals role.RoleId
                                select new UserResponse
                                {
                                    UserId = user.UserId,
                                    Username = user.Username,
                                    Email = user.Email,
                                    Role = role.RoleName,
                                    RegistrationDate = user.RegistrationDate
                                };

            return Ok(new { list = userResponses });
        }

        // GET: /api/users/{id}
        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            var role = _roleRepository.GetRoleById(user.RoleId);
            if (role == null)
                return BadRequest(new { message = "User role not found." });

            var userResponse = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = role.RoleName,
                RegistrationDate = user.RegistrationDate
            };

            return Ok(new { user = userResponse });
        }

        // POST: /api/users
        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _userRepository.AddUser(user);

            var role = _roleRepository.GetRoleById(user.RoleId);
            if (role == null)
                return BadRequest(new { message = "User role not found." });

            var userResponse = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = role.RoleName,
                RegistrationDate = user.RegistrationDate
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, new { user = userResponse });
        }

        // PUT: /api/users/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = _userRepository.GetUserById(id);
            if (existingUser == null)
                return NotFound(new { message = "User not found." });

            existingUser.Username = request.Username;
            existingUser.Email = request.Email;

            if (!string.IsNullOrEmpty(request.RoleName))
            {
                var role = _roleRepository.GetRoleByName(request.RoleName);
                if (role == null)
                    return BadRequest(new { message = "Invalid role name." });

                existingUser.RoleId = role.RoleId;
            }

            _userRepository.UpdateUser(existingUser);

            var updatedUser = _userRepository.GetUserById(id);
            var updatedRole = _roleRepository.GetRoleById(updatedUser.RoleId);

            var userResponse = new UserResponse
            {
                UserId = updatedUser.UserId,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                Role = updatedRole.RoleName,
                RegistrationDate = updatedUser.RegistrationDate
            };

            return Ok(new { user = userResponse });
        }

        // DELETE: /api/users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            try
            {
                _userRepository.DeleteUser(id);

                var role = _roleRepository.GetRoleById(user.RoleId);
                var userResponse = new UserResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Role = role != null ? role.RoleName : "Unknown",
                    RegistrationDate = user.RegistrationDate
                };

                return Ok(new { message = "User deleted successfully.", user = userResponse });
            }
            catch (OracleException ex) when (ex.Number == 2292)
            {
                // ORA-02292: integrity constraint violated - child record found
                return BadRequest(new { message = "Cannot delete user because there are related logs." });
            }
            catch (Exception)
            {
                // Optionally log the exception here using a logging framework
                return StatusCode(500, new { message = "An error occurred while deleting the user." });
            }
        }
    }
}
