using Microsoft.AspNetCore.Identity;
using UserService.Models;

namespace UserService.Services
{
    public interface IPasswordHashingService
    {
        string HashPassword(User user, string password);
        PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
    }
}

