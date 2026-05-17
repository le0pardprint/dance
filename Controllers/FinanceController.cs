using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public FinanceController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/finance/revenue
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var query = _dbContext.Subscriptions.AsQueryable();

            // Убираем фильтрацию по Registration_date (этого поля нет)
            // Просто считаем общую сумму
            var totalRevenue = await query
                .Where(s => s.Status == "Активен" || s.Status == "Оплачен")
                .SumAsync(s => s.Amount);

            // Убираем группировку по месяцам (нет даты)
            return Ok(new
            {
                totalRevenue = totalRevenue,
                message = "Фильтрация по датам временно отключена (нет поля Registration_date в Subscription)"
            });
        }

        // GET: api/finance/debts
        [HttpGet("debts")]
        public async Task<IActionResult> GetClientDebts()
        {
            var clientsWithDebts = await _dbContext.Clients
                .Select(c => new
                {
                    c.Client_id,
                    c.LastName,
                    c.FirstName,
                    TotalPaid = _dbContext.Subscriptions
                        .Where(s => s.Client_id == c.Client_id && s.Status == "Оплачен")
                        .Sum(s => s.Amount),
                    TotalAmount = _dbContext.Subscriptions
                        .Where(s => s.Client_id == c.Client_id)
                        .Sum(s => s.Amount),
                    Debt = _dbContext.Subscriptions
                        .Where(s => s.Client_id == c.Client_id)
                        .Sum(s => s.Amount) - _dbContext.Subscriptions
                        .Where(s => s.Client_id == c.Client_id && s.Status == "Оплачен")
                        .Sum(s => s.Amount)
                })
                .Where(c => c.Debt > 0)
                .ToListAsync();

            return Ok(clientsWithDebts);
        }

        // GET: api/finance/teacher-payments
        [HttpGet("teacher-payments")]
        public async Task<IActionResult> GetTeacherPayments([FromQuery] int year, [FromQuery] int month)
        {
            var payments = await _dbContext.Trainers
                .Select(t => new
                {
                    t.Trainer_id,
                    t.FirstName,
                    t.LastName,
                    ClassesCount = _dbContext.Classes
                        .Count(c => c.Trainer_id == t.Trainer_id && c.Status == "Проведено"),
                    HourlyRate = 1000,
                    TotalPayment = _dbContext.Classes
                        .Count(c => c.Trainer_id == t.Trainer_id && c.Status == "Проведено") * 1000
                })
                .ToListAsync();

            return Ok(new { year, month, payments });
        }

        // GET: api/finance/payments-summary
        [HttpGet("payments-summary")]
        public async Task<IActionResult> GetPaymentsSummary()
        {
            var summary = await _dbContext.Subscriptions
                .GroupBy(s => s.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(s => s.Amount)
                })
                .ToListAsync();

            var totalSubscriptions = await _dbContext.Subscriptions.CountAsync();
            var totalAmount = await _dbContext.Subscriptions.SumAsync(s => s.Amount);

            return Ok(new
            {
                summary = summary,
                totalSubscriptions = totalSubscriptions,
                totalAmount = totalAmount
            });
        }
    }
}