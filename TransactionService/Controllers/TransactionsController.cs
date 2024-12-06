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
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(TransactionDbContext context, ILogger<TransactionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            try
            {
                    if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid transaction data: {Transaction}", transaction);
                    return BadRequest(ModelState);
                }

                // Additional validation (e.g., transaction type)
                if (transaction.TransactionType != "Income" && transaction.TransactionType != "Expense")
                {
                    _logger.LogWarning("Invalid transaction type: {TransactionType}", transaction.TransactionType);
                    return BadRequest("Invalid transaction type. Must be 'Income' or 'Expense'.");
                }

                transaction.CreatedAt = DateTime.UtcNow; // Set created timestamp
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Transaction added successfully: {TransactionId}", transaction.Id);
                return Ok("Transaction added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding transaction: {Transaction}", transaction);
                return StatusCode(500, "An error occurred while adding the transaction.");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTransactionsForUser(
    int userId,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromQuery] string category = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sortBy = "date",
    [FromQuery] string sortOrder = "desc")
        {
            try
            {
                var transactionsQuery = _context.Transactions.Where(t => t.UserId == userId);

                if (startDate.HasValue)
                {
                    transactionsQuery = transactionsQuery.Where(t => t.Date >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    transactionsQuery = transactionsQuery.Where(t => t.Date <= endDate.Value);
                }

                if (!string.IsNullOrEmpty(category))
                {
                    transactionsQuery = transactionsQuery.Where(t => t.Category == category);
                }

                // Sorting
                transactionsQuery = sortOrder.ToLower() switch
                {
                    "asc" => sortBy.ToLower() switch
                    {
                        "amount" => transactionsQuery.OrderBy(t => t.Amount),
                        _ => transactionsQuery.OrderBy(t => t.Date),
                    },
                    _ => sortBy.ToLower() switch
                    {
                        "amount" => transactionsQuery.OrderByDescending(t => t.Amount),
                        _ => transactionsQuery.OrderByDescending(t => t.Date),
                    },
                };

                // Pagination
                var totalItems = await transactionsQuery.CountAsync();
                var transactions = await transactionsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    Transactions = transactions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transactions for user {UserId}", userId);
                return StatusCode(500, "An error occurred while fetching transactions.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction updatedTransaction)
        {
            try
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
                _logger.LogInformation("Transaction updated successfully: {TransactionId}", id);
                return Ok("Transaction updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction: {TransactionId}", id);
                return StatusCode(500, "An error occurred while updating the transaction.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction not found: {TransactionId}", id);
                    return NotFound("Transaction not found.");
                }

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Transaction deleted successfully: {TransactionId}", id);

                return Ok("Transaction deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction: {TransactionId}", id);
                return StatusCode(500, "An error occurred while deleting the transaction.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction not found: {TransactionId}", id);
                    return NotFound("Transaction not found.");
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transaction: {TransactionId}", id);
                return StatusCode(500, "An error occurred while fetching the transaction.");
            }
        }

    }
}
