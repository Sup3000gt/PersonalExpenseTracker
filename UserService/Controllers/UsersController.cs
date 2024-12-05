using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UsersController(UserDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            // Check if the username or email already exists
            if (_context.Users.Any(u => u.Username == user.Username || u.Email == user.Email))
            {
                return BadRequest("Username or email already exists.");
            }

            // Hash the password
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash));
                user.PasswordHash = Convert.ToBase64String(bytes);
            }

            // Generate email confirmation token
            user.EmailConfirmationToken = Guid.NewGuid().ToString();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);

            // Set default values
            user.EmailConfirmed = false;

            // Save the user to the database
            _context.Users.Add(user);
            _context.SaveChanges();

            // Send confirmation email (this is a placeholder, actual email service integration required)
            SendConfirmationEmail(user.Email, user.EmailConfirmationToken);

            return Ok("User registered successfully. Please check your email to activate your account.");
        }

        /// <summary>
        /// Confirm user's email
        /// </summary>
        [HttpGet("confirm-email")]
        public IActionResult ConfirmEmail(string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.EmailConfirmationToken == token);

            if (user == null || user.EmailConfirmationTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired token.");
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = DateTime.MinValue;

            _context.SaveChanges();

            return Ok("Email confirmed successfully. You can now log in.");
        }

        /// <summary>
        /// Log in a user
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] User loginUser)
        {
            // Find the user by username
            var user = _context.Users.SingleOrDefault(u => u.Username == loginUser.Username);
            if (user == null) return NotFound("User not found.");

            // Verify email confirmation
            if (!user.EmailConfirmed)
            {
                return Unauthorized("Email is not confirmed. Please confirm your email before logging in.");
            }

            // Verify password
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(loginUser.PasswordHash));
                var hashedPassword = Convert.ToBase64String(bytes);

                if (user.PasswordHash != hashedPassword)
                {
                    return Unauthorized("Invalid credentials.");
                }
            }

            return Ok("Login successful.");
        }

        /// <summary>
        /// Placeholder for sending confirmation emails
        /// </summary>
        private void SendConfirmationEmail(string email, string token)
        {
            // Placeholder: Use an actual email service like SendGrid, SMTP, etc.
            Console.WriteLine($"Email sent to {email} with confirmation token: {token}");
        }
    }
}
