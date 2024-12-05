using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Services;
using System.Security.Cryptography;
using System.Text;
using UserService.Data;
using UserService.Models;
using Microsoft.AspNetCore.Identity;

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

            // Hash the password using PasswordHasher<T>
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);

            // Generate email confirmation token
            user.EmailConfirmationToken = Guid.NewGuid().ToString();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);
            user.EmailConfirmed = false;

            // Save the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            try
            {
                // Generate the confirmation link using the local server URL for testing
                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "Users",
                    new { token = user.EmailConfirmationToken, email = user.Email },
                    Request.Scheme,
                    Request.Host.ToUriComponent()
                );

                var subject = "Please confirm your email";
                var plainTextContent = $"Hello {user.FirstName},\n\nPlease confirm your email by clicking this link: {confirmationLink}";
                var htmlContent = $"<p>Hello {user.FirstName},</p><p>Please confirm your email by clicking this link: <a href='{confirmationLink}'>Activate Account</a></p>";

                // Use your email service to send the email
                await _emailService.SendEmailAsync(user.Email, subject, plainTextContent, htmlContent);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
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

            // Retrieve user by email and token
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.EmailConfirmationToken == token);

            if (user == null)
            {
                return BadRequest("Invalid token or user not found.");
            }

            // Check if the token has expired
            if (user.EmailConfirmationTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Token has expired. Please request a new confirmation email.");
            }

            // Mark the email as confirmed
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null; // Clear the token
            user.EmailConfirmationTokenExpires = DateTime.MinValue;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Email confirmed successfully.");
        }


        /// Log in a user
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = _context.Users.SingleOrDefault(u => u.Username == loginRequest.Username);
            if (user == null || !user.EmailConfirmed)
            {
                return Unauthorized("Invalid username or password or email not confirmed.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok("Login successful.");
        }

    }
}
