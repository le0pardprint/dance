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
        public async Task<IActionResult> GetRevenue()
        {
            var totalRevenue = await _dbContext.Subscriptions
                .Where(s => s.Status == "Активен" || s.Status == "Оплачен")
                .SumAsync(s => s.Amount);

            return Ok(new { totalRevenue });
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

        // GET: api/finance/payments-summary — с именами клиентов
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

            // Детали по каждому абонементу с именем клиента
            var details = await _dbContext.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Group)
                .Select(s => new
                {
                    s.Sub_id,
                    ClientName = s.Client.LastName + " " + s.Client.FirstName,
                    GroupName = s.Group.Name,
                    s.Amount,
                    s.Status
                })
                .OrderBy(s => s.Status)
                .ToListAsync();

            var totalSubscriptions = await _dbContext.Subscriptions.CountAsync();
            var totalAmount = await _dbContext.Subscriptions.SumAsync(s => s.Amount);

            return Ok(new
            {
                summary,
                details,
                totalSubscriptions,
                totalAmount
            });
        }

        // POST: api/finance/add-debt — добавить долг клиенту
        [HttpPost("add-debt")]
        public async Task<IActionResult> AddDebt([FromBody] AddDebtDto dto)
        {
            var client = await _dbContext.Clients.FindAsync(dto.ClientId);
            if (client == null)
                return NotFound(new { message = "Клиент не найден" });

            var group = await _dbContext.Groups.FindAsync(dto.GroupId);
            if (group == null)
                return NotFound(new { message = "Группа не найдена" });

            var subscription = new Subscription
            {
                Client_id = dto.ClientId,
                Group_id = dto.GroupId,
                Amount = dto.Amount,
                Status = "Активен"
            };

            _dbContext.Subscriptions.Add(subscription);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Долг добавлен", subscription });
        }
    }

    public class AddDebtDto
    {
        public int ClientId { get; set; }
        public int GroupId { get; set; }
        public decimal Amount { get; set; }
    }
}