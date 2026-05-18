using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public SubscriptionsController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/subscriptions
        [HttpGet]
        public async Task<ActionResult<List<Subscription>>> GetAll()
        {
            var subscriptions = await _dbContext.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Group)
                .ToListAsync();

            return Ok(subscriptions);
        }

        // GET: api/subscriptions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetById(int id)
        {
            var subscription = await _dbContext.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Sub_id == id);

            if (subscription == null)
                return NotFound(new { message = "Абонемент не найден" });

            return Ok(subscription);
        }

        // GET: api/subscriptions/byclient/{clientId}
        [HttpGet("byclient/{clientId}")]
        public async Task<ActionResult<List<Subscription>>> GetByClientId(int clientId)
        {
            var subscriptions = await _dbContext.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Group)
                .Where(s => s.Client_id == clientId)
                .ToListAsync();

            return Ok(subscriptions);
        }

        // GET: api/subscriptions/bygroup/{groupId}
        [HttpGet("bygroup/{groupId}")]
        public async Task<ActionResult<List<Subscription>>> GetByGroupId(int groupId)
        {
            var subscriptions = await _dbContext.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Group)
                .Where(s => s.Group_id == groupId)
                .ToListAsync();

            return Ok(subscriptions);
        }

        // GET: api/subscriptions/bystatus?status=Активен
        [HttpGet("bystatus")]
        public async Task<ActionResult<List<Subscription>>> GetByStatus([FromQuery] string status)
        {
            var subscriptions = await _dbContext.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Group)
                .Where(s => s.Status == status)
                .ToListAsync();

            return Ok(subscriptions);
        }

        // GET: api/subscriptions/active
        [HttpGet("active")]
        public async Task<ActionResult<List<Subscription>>> GetActive()
        {
            var subscriptions = await _dbContext.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Group)
                .Where(s => s.Status == "Активен")
                .ToListAsync();

            return Ok(subscriptions);
        }

        // POST: api/subscriptions
        [HttpPost]
        public async Task<ActionResult<Subscription>> Create(Subscription subscription)
        {
            var client = await _dbContext.Clients.FindAsync(subscription.Client_id);
            if (client == null)
                return BadRequest(new { message = "Клиент не найден" });

            var group = await _dbContext.Groups.FindAsync(subscription.Group_id);
            if (group == null)
                return BadRequest(new { message = "Группа не найдена" });

            _dbContext.Subscriptions.Add(subscription);
            await _dbContext.SaveChangesAsync();

            var createdSubscription = await _dbContext.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Sub_id == subscription.Sub_id);

            return CreatedAtAction(nameof(GetById), new { id = subscription.Sub_id }, createdSubscription);
        }

        // PUT: api/subscriptions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Subscription subscription)
        {
            if (id != subscription.Sub_id)
                return BadRequest(new { message = "ID в запросе не совпадает с ID абонемента" });

            var existingSubscription = await _dbContext.Subscriptions.FindAsync(id);
            if (existingSubscription == null)
                return NotFound(new { message = "Абонемент не найден" });

            existingSubscription.Client_id = subscription.Client_id;
            existingSubscription.Group_id = subscription.Group_id;
            existingSubscription.Amount = subscription.Amount;
            existingSubscription.Status = subscription.Status;

            await _dbContext.SaveChangesAsync();

            return Ok(existingSubscription);
        }

        // PATCH: api/subscriptions/{id}/status  ← НОВЫЙ
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] SubStatusDto dto)
        {
            var subscription = await _dbContext.Subscriptions.FindAsync(id);
            if (subscription == null)
                return NotFound(new { message = "Абонемент не найден" });

            subscription.Status = dto.Status;
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Статус обновлён" });
        }

        // PUT: api/subscriptions/{id}/deactivate
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var subscription = await _dbContext.Subscriptions.FindAsync(id);
            if (subscription == null)
                return NotFound(new { message = "Абонемент не найден" });

            subscription.Status = "Истек";
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Абонемент деактивирован" });
        }

        // DELETE: api/subscriptions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var subscription = await _dbContext.Subscriptions.FindAsync(id);
            if (subscription == null)
                return NotFound(new { message = "Абонемент не найден" });

            _dbContext.Subscriptions.Remove(subscription);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Абонемент удален" });
        }
    }

    public class SubStatusDto
    {
        public string Status { get; set; } = "";
    }
}