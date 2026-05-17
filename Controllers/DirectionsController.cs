using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public DirectionsController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/directions
        [HttpGet]
        public async Task<ActionResult<List<Direction>>> GetAll()
        {
            var directions = await _dbContext.Directions
                .Include(d => d.Groups)
                .ToListAsync();

            return Ok(directions);
        }

        // GET: api/directions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Direction>> GetById(int id)
        {
            var direction = await _dbContext.Directions
                .Include(d => d.Groups)
                .FirstOrDefaultAsync(d => d.Direction_id == id);

            if (direction == null)
                return NotFound(new { message = "Направление не найдено" });

            return Ok(direction);
        }

        // GET: api/directions/search?name=Хип-хоп&ageGroup=Дети&level=Начальный
        [HttpGet("search")]
        public async Task<ActionResult<List<Direction>>> Search(
            [FromQuery] string? name,
            [FromQuery] string? ageGroup,
            [FromQuery] string? level)
        {
            var query = _dbContext.Directions
                .Include(d => d.Groups)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(d => d.Name.Contains(name));

            if (!string.IsNullOrEmpty(ageGroup))
                query = query.Where(d => d.AgeGroup == ageGroup);

            if (!string.IsNullOrEmpty(level))
                query = query.Where(d => d.Level == level);

            var directions = await query.ToListAsync();
            return Ok(directions);
        }

        // GET: api/directions/agegroups (получить все возрастные группы для фильтра)
        [HttpGet("agegroups")]
        public async Task<ActionResult<List<string>>> GetAgeGroups()
        {
            var ageGroups = await _dbContext.Directions
                .Select(d => d.AgeGroup)
                .Distinct()
                .ToListAsync();

            return Ok(ageGroups);
        }

        // GET: api/directions/levels (получить все уровни для фильтра)
        [HttpGet("levels")]
        public async Task<ActionResult<List<string>>> GetLevels()
        {
            var levels = await _dbContext.Directions
                .Select(d => d.Level)
                .Distinct()
                .ToListAsync();

            return Ok(levels);
        }

        // POST: api/directions
        [HttpPost]
        public async Task<ActionResult<Direction>> Create(Direction direction)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _dbContext.Directions.Add(direction);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = direction.Direction_id }, direction);
        }

        // PUT: api/directions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Direction direction)
        {
            if (id != direction.Direction_id)
                return BadRequest(new { message = "ID в запросе не совпадает с ID направления" });

            var existingDirection = await _dbContext.Directions.FindAsync(id);
            if (existingDirection == null)
                return NotFound(new { message = "Направление не найдено" });

            // Обновляем поля
            existingDirection.Name = direction.Name;
            existingDirection.Description = direction.Description;
            existingDirection.AgeGroup = direction.AgeGroup;
            existingDirection.Level = direction.Level;

            await _dbContext.SaveChangesAsync();

            return Ok(existingDirection);
        }

        // DELETE: api/directions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var direction = await _dbContext.Directions.FindAsync(id);
            if (direction == null)
                return NotFound(new { message = "Направление не найдено" });

            // Проверяем, есть ли связанные группы или тренеры
            var hasGroups = await _dbContext.Groups.AnyAsync(g => g.Direction_id == id);
            var hasTrainers = await _dbContext.Trainers.AnyAsync(t => t.Direction_id == id);

            if (hasGroups || hasTrainers)
                return BadRequest(new { message = "Нельзя удалить направление, у которого есть группы или тренеры" });

            _dbContext.Directions.Remove(direction);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Направление удалено" });
        }
    }
}