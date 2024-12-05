using Microsoft.AspNetCore.Identity;
using UserService.Models;

namespace UserService.Services
{
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public PasswordHashingService()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
    }
}
