using System.ComponentModel.DataAnnotations;

namespace ReportService.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign Key to Users

        [Required]
        [MaxLength(50)]
        public string TransactionType { get; set; } // Income, Expense, etc.

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string Description { get; set; } // Optional

        [MaxLength(50)]
        public string Category { get; set; } // Optional

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
