using System;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password Hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } // Changed Password to PasswordHash

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; } // Nullable

        // Email confirmation properties
        [Required]
        public bool EmailConfirmed { get; set; } = false;

        public string? EmailConfirmationToken { get; set; }

        public DateTime EmailConfirmationTokenExpires { get; set; }

        // Constructor to initialize default values
        public User()
        {
            Username = string.Empty;
            PasswordHash = string.Empty;
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            PhoneNumber = string.Empty;
        }
    }
}

