using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public RegistrationController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/registration
        [HttpGet]
        public async Task<ActionResult<List<Registration>>> GetAll()
        {
            var registrations = await _dbContext.Registrations
                .Include(r => r.Client)
                .Include(r => r.Group)
                .ToListAsync();

            return Ok(registrations);
        }

        // GET: api/registration/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Registration>> GetById(int id)
        {
            var registration = await _dbContext.Registrations
                .Include(r => r.Client)
                .Include(r => r.Group)
                .FirstOrDefaultAsync(r => r.Registration_id == id);

            if (registration == null)
                return NotFound(new { message = "Запись не найдена" });

            return Ok(registration);
        }

        // GET: api/registration/byclient/{clientId}
        [HttpGet("byclient/{clientId}")]
        public async Task<ActionResult<List<Registration>>> GetByClientId(int clientId)
        {
            var registrations = await _dbContext.Registrations
                .Include(r => r.Client)
                .Include(r => r.Group)
                .Where(r => r.Client_id == clientId)
                .ToListAsync();

            return Ok(registrations);
        }

        // GET: api/registration/bygroup/{groupId}
        [HttpGet("bygroup/{groupId}")]
        public async Task<ActionResult<List<Registration>>> GetByGroupId(int groupId)
        {
            var registrations = await _dbContext.Registrations
                .Include(r => r.Client)
                .Include(r => r.Group)
                .Where(r => r.Group_id == groupId)
                .ToListAsync();

            return Ok(registrations);
        }

        // GET: api/registration/bydate?date=2026-05-05
        [HttpGet("bydate")]
        public async Task<ActionResult<List<Registration>>> GetByDate([FromQuery] DateTime date)
        {
            var registrations = await _dbContext.Registrations
                .Include(r => r.Client)
                .Include(r => r.Group)
                .Where(r => r.Registration_date.Date == date.Date)
                .ToListAsync();

            return Ok(registrations);
        }

        // GET: api/registration/daterange?start=2026-05-01&end=2026-05-31
        [HttpGet("daterange")]
        public async Task<ActionResult<List<Registration>>> GetByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var registrations = await _dbContext.Registrations
                .Include(r => r.Client)
                .Include(r => r.Group)
                .Where(r => r.Registration_date >= start && r.Registration_date <= end)
                .ToListAsync();

            return Ok(registrations);
        }

        // POST: api/registration
        [HttpPost]
        public async Task<ActionResult<Registration>> Create(Registration registration)
        {
            // Проверяем, существует ли клиент
            var client = await _dbContext.Clients.FindAsync(registration.Client_id);
            if (client == null)
                return BadRequest(new { message = "Клиент не найден" });

            // Проверяем, существует ли группа
            var group = await _dbContext.Groups.FindAsync(registration.Group_id);
            if (group == null)
                return BadRequest(new { message = "Группа не найдена" });

            _dbContext.Registrations.Add(registration);
            await _dbContext.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            var createdRegistration = await _dbContext.Registrations
                .Include(r => r.Client)
                .Include(r => r.Group)
                .FirstOrDefaultAsync(r => r.Registration_id == registration.Registration_id);

            return CreatedAtAction(nameof(GetById), new { id = registration.Registration_id }, createdRegistration);
        }

        // PUT: api/registration/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Registration registration)
        {
            if (id != registration.Registration_id)
                return BadRequest(new { message = "ID в запросе не совпадает с ID записи" });

            var existingRegistration = await _dbContext.Registrations.FindAsync(id);
            if (existingRegistration == null)
                return NotFound(new { message = "Запись не найдена" });

            // Обновляем поля
            existingRegistration.Client_id = registration.Client_id;
            existingRegistration.Group_id = registration.Group_id;
            existingRegistration.Registration_date = registration.Registration_date;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/registration/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var registration = await _dbContext.Registrations.FindAsync(id);
            if (registration == null)
                return NotFound(new { message = "Запись не найдена" });

            _dbContext.Registrations.Remove(registration);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Запись удалена" });
        }
    }
}