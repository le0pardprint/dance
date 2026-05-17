using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TrainersController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/trainers
        [HttpGet]
        public async Task<ActionResult<List<Trainer>>> GetAll()
        {
            var trainers = await _dbContext.Trainers
                .Include(t => t.Direction)
                .Include(t => t.Groups)
                .Include(t => t.Classes)
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: api/trainers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Trainer>> GetById(int id)
        {
            var trainer = await _dbContext.Trainers
                .Include(t => t.Direction)
                .Include(t => t.Groups)
                .Include(t => t.Classes)
                .FirstOrDefaultAsync(t => t.Trainer_id == id);

            if (trainer == null)
                return NotFound(new { message = "Тренер не найден" });

            return Ok(trainer);
        }

        // GET: api/trainers/search?lastName=Сергеев&firstName=Сергей&directionId=1
        [HttpGet("search")]
        public async Task<ActionResult<List<Trainer>>> Search(
            [FromQuery] string? lastName,
            [FromQuery] string? firstName,
            [FromQuery] int? directionId,
            [FromQuery] string? phone,
            [FromQuery] string? email)
        {
            var query = _dbContext.Trainers
                .Include(t => t.Direction)
                .Include(t => t.Groups)
                .Include(t => t.Classes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(t => t.LastName.Contains(lastName));

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(t => t.FirstName.Contains(firstName));

            if (directionId.HasValue)
                query = query.Where(t => t.Direction_id == directionId.Value);

            if (!string.IsNullOrEmpty(phone))
                query = query.Where(t => t.Phone != null && t.Phone.Contains(phone));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(t => t.Email != null && t.Email.Contains(email));

            var trainers = await query.ToListAsync();
            return Ok(trainers);
        }

        // GET: api/trainers/bydirection/{directionId}
        [HttpGet("bydirection/{directionId}")]
        public async Task<ActionResult<List<Trainer>>> GetByDirectionId(int directionId)
        {
            var trainers = await _dbContext.Trainers
                .Include(t => t.Direction)
                .Include(t => t.Groups)
                .Include(t => t.Classes)
                .Where(t => t.Direction_id == directionId)
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: api/trainers/withgroups (тренеры, у которых есть группы)
        [HttpGet("withgroups")]
        public async Task<ActionResult<List<Trainer>>> GetTrainersWithGroups()
        {
            var trainers = await _dbContext.Trainers
                .Include(t => t.Direction)
                .Include(t => t.Groups)
                .Include(t => t.Classes)
                .Where(t => t.Groups != null && t.Groups.Any())
                .ToListAsync();

            return Ok(trainers);
        }

        // POST: api/trainers
        [HttpPost]
        public async Task<ActionResult<Trainer>> Create(Trainer trainer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Проверяем, существует ли направление
            var direction = await _dbContext.Directions.FindAsync(trainer.Direction_id);
            if (direction == null)
                return BadRequest(new { message = "Направление не найдено" });

            _dbContext.Trainers.Add(trainer);
            await _dbContext.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            var createdTrainer = await _dbContext.Trainers
                .Include(t => t.Direction)
                .FirstOrDefaultAsync(t => t.Trainer_id == trainer.Trainer_id);

            return CreatedAtAction(nameof(GetById), new { id = trainer.Trainer_id }, createdTrainer);
        }

        // PUT: api/trainers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Trainer trainer)
        {
            if (id != trainer.Trainer_id)
                return BadRequest(new { message = "ID в запросе не совпадает с ID тренера" });

            var existingTrainer = await _dbContext.Trainers.FindAsync(id);
            if (existingTrainer == null)
                return NotFound(new { message = "Тренер не найден" });

            // Обновляем поля
            existingTrainer.LastName = trainer.LastName;
            existingTrainer.FirstName = trainer.FirstName;
            existingTrainer.Direction_id = trainer.Direction_id;
            existingTrainer.Phone = trainer.Phone;
            existingTrainer.Email = trainer.Email;

            await _dbContext.SaveChangesAsync();

            return Ok(existingTrainer);
        }

        // DELETE: api/trainers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await _dbContext.Trainers.FindAsync(id);
            if (trainer == null)
                return NotFound(new { message = "Тренер не найден" });

            // Проверяем, есть ли связанные группы или занятия
            var hasGroups = await _dbContext.Groups.AnyAsync(g => g.Trainer_id == id);
            var hasClasses = await _dbContext.Classes.AnyAsync(c => c.Trainer_id == id);

            if (hasGroups || hasClasses)
                return BadRequest(new { message = "Нельзя удалить тренера, у которого есть группы или занятия" });

            _dbContext.Trainers.Remove(trainer);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Тренер удален" });
        }
    }
}