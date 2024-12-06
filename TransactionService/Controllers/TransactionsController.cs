using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Models;

namespace TransactionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionDbContext _context;

        public TransactionsController(TransactionDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add the new transaction to the database
            transaction.CreatedAt = DateTime.UtcNow; // Set the creation timestamp
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Transaction added successfully.",
                TransactionId = transaction.Id
            });
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTransactionsForUser(
            int userId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string category = null)
        {
            // Validate the userId
            if (userId < 0)
            {
                return BadRequest("Invalid user ID.");
            }

            // Start building the query
            var query = _context.Transactions.Where(t => t.UserId == userId);

            // Apply date range filter if provided
            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            // Apply category filter if provided
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // Execute the query
            var transactions = await query.ToListAsync();

            // Return results
            return Ok(transactions);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction updatedTransaction)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound("Transaction not found.");
            }

            transaction.TransactionType = updatedTransaction.TransactionType;
            transaction.Amount = updatedTransaction.Amount;
            transaction.Date = updatedTransaction.Date;
            transaction.Description = updatedTransaction.Description;
            transaction.Category = updatedTransaction.Category;
            transaction.UpdatedAt = DateTime.UtcNow;

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            return Ok("Transaction updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound("Transaction not found.");
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok("Transaction deleted successfully.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound("Transaction not found.");
            }

            return Ok(transaction);
        }

    }
}
