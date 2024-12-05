using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
