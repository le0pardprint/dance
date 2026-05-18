using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;

namespace dance.API.Controllers
{
    [Authorize(Roles = "Admin,Client")]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ClientController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/client/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.User_id == userId);

            if (client == null)
                return NotFound(new { message = "Клиент не найден" });

            return Ok(new
            {
                client.Client_id,
                client.FirstName,
                client.LastName,
                client.Age,
                client.Phone,
                client.Email
            });
        }

        // GET: api/client/schedule
        [HttpGet("schedule")]
        public async Task<IActionResult> GetMySchedule()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.User_id == userId);

            if (client == null)
                return NotFound(new { message = "Клиент не найден" });

            var groupIds = await _dbContext.Registrations
                .Where(r => r.Client_id == client.Client_id)
                .Select(r => r.Group_id)
                .ToListAsync();

            var schedule = await _dbContext.Classes
                .Include(c => c.Group)
                .Include(c => c.Trainer)
                .Where(c => groupIds.Contains(c.Group_id))
                .OrderBy(c => c.Date)
                .ThenBy(c => c.Time)
                .Select(c => new
                {
                    c.Class_id,
                    c.Date,
                    c.Time,
                    c.Status,
                    GroupName = c.Group.Name,
                    DirectionName = c.Group.Direction.Name,
                    TrainerName = $"{c.Trainer.FirstName} {c.Trainer.LastName}"
                })
                .ToListAsync();

            return Ok(schedule);
        }

        // GET: api/client/payments
        [HttpGet("payments")]
        public async Task<IActionResult> GetMyPayments()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.User_id == userId);

            if (client == null)
                return NotFound(new { message = "Клиент не найден" });

            var subscriptions = await _dbContext.Subscriptions
                .Where(s => s.Client_id == client.Client_id)
                .Select(s => new
                {
                    s.Sub_id,
                    s.Amount,
                    s.Status,
                    GroupName = s.Group.Name
                })
                .ToListAsync();

            var totalPaid = subscriptions.Where(s => s.Status == "Оплачен").Sum(s => s.Amount);
            var totalDebt = subscriptions.Where(s => s.Status == "Активен").Sum(s => s.Amount);

            return Ok(new
            {
                subscriptions = subscriptions,
                totalPaid = totalPaid,
                totalDebt = totalDebt
            });
        }

        // GET: api/client/my-groups
        [HttpGet("my-groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.User_id == userId);

            if (client == null)
                return NotFound(new { message = "Клиент не найден" });

            var groups = await _dbContext.Registrations
                .Where(r => r.Client_id == client.Client_id)
                .Select(r => new
                {
                    r.Group.Group_id,
                    r.Group.Name,
                    DirectionName = r.Group.Direction.Name,
                    TrainerName = $"{r.Group.Trainer.FirstName} {r.Group.Trainer.LastName}",
                    r.Group.Status
                })
                .ToListAsync();

            return Ok(groups);
        }
    }
}