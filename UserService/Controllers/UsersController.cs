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

        public UsersController(UserDbContext context, IEmailService emailService, IPasswordHashingService passwordHashingService)
        {
            _context = context;
            _emailService = emailService;
            _passwordHashingService = passwordHashingService;
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

            return Ok("Login successful.");
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Generate password reset token
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            try
            {
                // Send password reset email
                var resetLink = Url.Action(
                    "ResetPassword",
                    "Users",
                    new { token = user.PasswordResetToken, email = user.Email },
                    Request.Scheme
                );
                var subject = "Reset your password";
                var plainTextContent = $"Click here to reset your password: {resetLink}";
                var htmlContent = $"<p>Click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";
                await _emailService.SendEmailAsync(user.Email, subject, plainTextContent, htmlContent);

                return Ok("Password reset email sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
                return StatusCode(500, "Error sending password reset email.");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Invalid request.");
            }

            // Find user by email and token
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordResetToken == request.Token);
            if (user == null)
            {
                return BadRequest("Invalid token or email.");
            }

            // Check if the token has expired
            if (user.PasswordResetTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Token has expired. Please request a new password reset.");
            }

            // Hash the new password
            user.PasswordHash = _passwordHashingService.HashPassword(user, request.NewPassword);

            // Clear the token and save changes
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password reset successfully.");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Invalid request. Please provide all required fields.");
            }

            // Find user by username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Verify current password using the PasswordHashingService
            var verificationResult = _passwordHashingService.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                return Unauthorized("Current password is incorrect.");
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
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username))
            {
                return BadRequest("Invalid request. Username is required.");
            }

            // Find user by username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update fields
            user.Email = request.Email ?? user.Email;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;

            // Save changes
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("User profile updated successfully.");
        }

    }
}
