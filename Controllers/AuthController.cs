using Microsoft.AspNetCore.Mvc;
using SurfingPointServer.Models;
using SurfingPointServer.Services;
using System;

namespace SurfingPointServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtService = new JwtService(_configuration);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (user.DateJoined == default)
                    user.DateJoined = DateTime.Now;

                // הצפנת הסיסמה
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                int newId = DBServices.AddUser(user);
                return Ok(new { message = "User registered successfully", userId = newId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            var existingUser = DBServices.GetUserByEmail(request.Email);

            if (existingUser == null)
                return Unauthorized("User not found");

            // כאן היה השם השגוי PasswordHash - תיקנו ל־Password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, existingUser.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized("Invalid password");

            var token = _jwtService.GenerateToken(existingUser);

            return Ok(new { token });
        }
    }
}
