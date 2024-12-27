using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class ChangePasswordRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters long.")]
        public string NewPassword { get; set; }
    }
}
