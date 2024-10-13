using BlazorApp.Models;
using BlazorApp.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    }
}
