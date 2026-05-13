using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ClientsController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/clients
        [HttpGet]
        public async Task<ActionResult<List<Client>>> GetAll()
        {
            var clients = await _dbContext.Clients
                .Include(c => c.AttendanceRecords)
                .Include(c => c.Registrations)
                .Include(c => c.Subscriptions)
                .ToListAsync();

            return Ok(clients);
        }

        // GET: api/clients/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(int id)
        {
            var client = await _dbContext.Clients
                .Include(c => c.AttendanceRecords)
                .Include(c => c.Registrations)
                .Include(c => c.Subscriptions)
                .FirstOrDefaultAsync(c => c.Client_id == id);

            if (client == null)
                return NotFound(new { message = "Клиент не найден" });

            return Ok(client);
        }

        // GET: api/clients/search?lastName=Иванов&firstName=Иван&phone=...
        [HttpGet("search")]
        public async Task<ActionResult<List<Client>>> Search(
            [FromQuery] string? lastName,
            [FromQuery] string? firstName,
            [FromQuery] string? phone,
            [FromQuery] string? email)
        {
            var query = _dbContext.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(c => c.LastName.Contains(lastName));

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(c => c.FirstName.Contains(firstName));

            if (!string.IsNullOrEmpty(phone))
                query = query.Where(c => c.Phone != null && c.Phone.Contains(phone));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(c => c.Email != null && c.Email.Contains(email));

            var clients = await query.ToListAsync();
            return Ok(clients);
        }

        // GET: api/clients/byage?minAge=10&maxAge=18
        [HttpGet("byage")]
        public async Task<ActionResult<List<Client>>> GetByAge(
            [FromQuery] int? minAge,
            [FromQuery] int? maxAge)
        {
            var query = _dbContext.Clients.AsQueryable();

            if (minAge.HasValue)
                query = query.Where(c => c.Age >= minAge.Value);

            if (maxAge.HasValue)
                query = query.Where(c => c.Age <= maxAge.Value);

            var clients = await query.ToListAsync();
            return Ok(clients);
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<Client>> Create(Client client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = client.Client_id }, client);
        }

        // PUT: api/clients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Client client)
        {
            if (id != client.Client_id)
                return BadRequest(new { message = "ID в запросе не совпадает с ID клиента" });

            var existingClient = await _dbContext.Clients.FindAsync(id);
            if (existingClient == null)
                return NotFound(new { message = "Клиент не найден" });

            // Обновляем поля
            existingClient.LastName = client.LastName;
            existingClient.FirstName = client.FirstName;
            existingClient.Age = client.Age;
            existingClient.Phone = client.Phone;
            existingClient.Email = client.Email;

            await _dbContext.SaveChangesAsync();

            return Ok(existingClient);
        }

        // DELETE: api/clients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _dbContext.Clients.FindAsync(id);
            if (client == null)
                return NotFound(new { message = "Клиент не найден" });

            // Проверяем, есть ли связанные записи
            var hasAttendance = await _dbContext.AttendanceRecords.AnyAsync(a => a.Client_id == id);
            var hasRegistrations = await _dbContext.Registrations.AnyAsync(r => r.Client_id == id);
            var hasSubscriptions = await _dbContext.Subscriptions.AnyAsync(s => s.Client_id == id);

            if (hasAttendance || hasRegistrations || hasSubscriptions)
                return BadRequest(new { message = "Нельзя удалить клиента, у которого есть записи о посещениях, регистрациях или абонементах" });

            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Клиент удален" });
        }
    }
}