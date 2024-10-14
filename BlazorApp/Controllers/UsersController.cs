using BlazorApp.Models;
using BlazorApp.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(Users newUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the new user to the database
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User {Username} registered successfully", newUser.username);

                    return Ok(new { message = "User registered successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while registering user");
                    return StatusCode(500, "Internal server error");
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.username == model.Username && u.password == model.Password);
                if (user != null)
                {
                    return Ok(new LoginResult { Success = true });
                }
                return Ok(new LoginResult { Success = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, "An error occurred during login");
            }
        }

        public class LoginModel
        {
            [Required(ErrorMessage = "El nombre de usuario es requerido")]
            public string Username { get; set; }

            [Required(ErrorMessage = "La contraseña es requerida")]
            public string Password { get; set; }
        }

        public class LoginResult
        {
            public bool Success { get; set; }
        }
    }
}
