using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Services;
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
        private readonly IEmailService _emailService;

        public UsersController(UserDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Check if username or email already exists
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest("This username is already taken.");
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest("An account with this email already exists.");
            }

            // Remove non-numeric characters from phone number
            var phoneDigits = new string(user.PhoneNumber.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length != 10)
            {
                return BadRequest("Phone number must be exactly 10 digits.");
            }

            // Validate Date of Birth
            if (user.DateOfBirth == null || user.DateOfBirth > DateTime.UtcNow)
            {
                return BadRequest("Invalid or missing Date of Birth.");
            }

            // Generate email confirmation token
            user.EmailConfirmationToken = Guid.NewGuid().ToString();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            user.EmailConfirmed = false;

            // Hash the password
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash));
                user.PasswordHash = Convert.ToBase64String(bytes);
            }

            // Save the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            try
            {
                // Send confirmation email
                var confirmationLink = $"https://yourdomain.com/api/account/confirm-email?token={user.EmailConfirmationToken}&email={user.Email}";
                var subject = "Please confirm your email";
                var plainTextContent = $"Hello {user.FirstName},\n\nPlease confirm your email by clicking this link: {confirmationLink}";
                var htmlContent = $"<p>Hello {user.FirstName},</p><p>Please confirm your email by clicking this link: <a href='{confirmationLink}'>Activate Account</a></p>";

                await _emailService.SendEmailAsync(user.Email, subject, plainTextContent, htmlContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return StatusCode(500, "An error occurred while sending the confirmation email. Please try again later.");
            }

            return Ok("User registered successfully. Please check your email to activate your account.");
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid token or email.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.EmailConfirmationToken == token);

            if (user == null)
            {
                return BadRequest("Invalid token or user does not exist.");
            }

            if (user.EmailConfirmationTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Token has expired. Please request a new confirmation email.");
            }

            // Mark email as confirmed
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null; // Clear the token
            user.EmailConfirmationTokenExpires = DateTime.MinValue;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Email confirmed successfully. You can now log in.");
        }


        /// Log in a user
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
        //private void SendConfirmationEmail(string email, string token)
        //{
        //    // Placeholder: Use an actual email service like SendGrid, SMTP, etc.
        //    Console.WriteLine($"Email sent to {email} with confirmation token: {token}");
        //}
    }
}
