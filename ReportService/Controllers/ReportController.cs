using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportDbContext _context;

        public ReportController(ReportDbContext context)
        {
            _context = context;
        }

        [HttpGet("monthly-summary")]
        public async Task<IActionResult> GetMonthlySummary(int userId, int year, int month)
        {
            if (userId < 0)
                return BadRequest("Invalid user ID.");

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date.Year == year && t.Date.Month == month)
                .GroupBy(t => t.TransactionType)
                .Select(g => new
                {
                    TransactionType = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .ToListAsync();

            return Ok(transactions);
        }

        [HttpGet("category-breakdown")]
        public async Task<IActionResult> GetCategoryBreakdown(int userId, int year, int month)
        {
            if (userId < 0)
                return BadRequest("Invalid user ID.");

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date.Year == year && t.Date.Month == month)
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .ToListAsync();

            return Ok(transactions);
        }

        [HttpGet("custom-date-range")]
        public async Task<IActionResult> GetCustomDateRangeReport(int userId, DateTime startDate, DateTime endDate)
        {
            if (userId < 0)
                return BadRequest("Invalid user ID.");
            if (startDate > endDate)
                return BadRequest("Start date cannot be after end date.");

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .Select(t => new
                {
                    t.Id,
                    t.TransactionType,
                    t.Amount,
                    t.Date,
                    t.Description,
                    t.Category
                })
                .ToListAsync();

            return Ok(transactions);
        }

    }
}
