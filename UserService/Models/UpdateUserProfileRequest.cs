using System;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class UpdateUserProfileRequest
    {
        [Required]
        public string Username { get; set; } // Required to identify the user

        [EmailAddress]
        public string Email { get; set; } 

        [StringLength(50)]
        public string FirstName { get; set; } 

        [StringLength(50)]
        public string LastName { get; set; } 

        [Phone]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
