using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Services;
using UserService.Data;
using UserService.Models;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly JwtTokenService _jwtTokenService;

        public UsersController(UserDbContext context, IEmailService emailService, IPasswordHashingService passwordHashingService, JwtTokenService jwtTokenService)
        {
            _context = context;
            _emailService = emailService;
            _passwordHashingService = passwordHashingService;
            _jwtTokenService = jwtTokenService;
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
            user.PasswordHash = _passwordHashingService.HashPassword(user, user.PasswordHash);

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

            var result = _passwordHashingService.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate a JWT token
            var token = _jwtTokenService.GenerateToken(user);  // Implement this method

            return Ok(new { message = "Login successful.", token, userId = user.Id }); // Include userId if needed
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Invalid request. Please provide both username and email.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Find user by username and email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Email == request.Email);
            if (user == null)
            {
                return NotFound("No matching account found.");
            }

            // Generate a temporary password
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8); // Example: random 8-character string
            user.PasswordHash = _passwordHashingService.HashPassword(user, tempPassword);
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            try
            {
                // Send email with temporary password
                var subject = "Your Temporary Password";
                var plainTextContent = $"Your temporary password is: {tempPassword}\nIt will expire in 1 hour.";
                var htmlContent = $"<p>Your temporary password is:</p><strong>{tempPassword}</strong><p>It will expire in 1 hour.</p>";
                await _emailService.SendEmailAsync(user.Email, subject, plainTextContent, htmlContent);

                return Ok("Temporary password sent to your email.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return StatusCode(500, "Error sending temporary password email.");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Invalid request. Please provide all required fields.");
            }

            // Find user by username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Hash the new password using the PasswordHashingService
            user.PasswordHash = _passwordHashingService.HashPassword(user, request.NewPassword);

            // Save changes to the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required.");
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Return user details without sensitive information like password hash
            var userProfile = new
            {
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.DateOfBirth
            };

            return Ok(userProfile);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest updatedUser)
        {
            if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Username))
            {
                return BadRequest("Invalid request. Username is required.");
            }

            // Find user by username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == updatedUser.Username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update fields
            user.Email = updatedUser.Email ?? user.Email;
            user.FirstName = updatedUser.FirstName ?? user.FirstName;
            user.LastName = updatedUser.LastName ?? user.LastName;
            user.PhoneNumber = updatedUser.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirth = updatedUser.DateOfBirth ?? user.DateOfBirth;

            // Save changes
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("User profile updated successfully.");
        }

    }
}
