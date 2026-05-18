using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ClassesController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/classes
        [HttpGet]
        public async Task<ActionResult<List<Class>>> GetAll()
        {
            var classes = await _dbContext.Classes
                .Include(c => c.Group)
                    .ThenInclude(g => g.Direction)
                .Include(c => c.Group)
                    .ThenInclude(g => g.Trainer)
                .Include(c => c.Trainer)
                .Include(c => c.AttendanceRecords)
                .ToListAsync();

            return Ok(classes);
        }

        // GET: api/classes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Class>> GetById(int id)
        {
            var classObj = await _dbContext.Classes
                .Include(c => c.Group)
                    .ThenInclude(g => g.Direction)
                .Include(c => c.Group)
                    .ThenInclude(g => g.Trainer)
                .Include(c => c.Trainer)
                .Include(c => c.AttendanceRecords)
                .FirstOrDefaultAsync(c => c.Class_id == id);

            if (classObj == null)
                return NotFound(new { message = "Занятие не найдено" });

            return Ok(classObj);
        }

        // GET: api/classes/bygroup/{groupId}
        [HttpGet("bygroup/{groupId}")]
        public async Task<ActionResult<List<Class>>> GetByGroupId(int groupId)
        {
            var classes = await _dbContext.Classes
                .Include(c => c.Group)
                .Include(c => c.Trainer)
                .Where(c => c.Group_id == groupId)
                .ToListAsync();

            return Ok(classes);
        }

        // GET: api/classes/bytrainer/{trainerId}
        [HttpGet("bytrainer/{trainerId}")]
        public async Task<ActionResult<List<Class>>> GetByTrainerId(int trainerId)
        {
            var classes = await _dbContext.Classes
                .Include(c => c.Group)
                .Include(c => c.Trainer)
                .Where(c => c.Trainer_id == trainerId)
                .ToListAsync();

            return Ok(classes);
        }

        // GET: api/classes/bydate?date=2026-05-12
        [HttpGet("bydate")]
        public async Task<ActionResult<List<Class>>> GetByDate([FromQuery] DateTime date)
        {
            var classes = await _dbContext.Classes
                .Include(c => c.Group)
                .Include(c => c.Trainer)
                .Where(c => c.Date.Date == date.Date)
                .ToListAsync();

            return Ok(classes);
        }

        // GET: api/classes/status?status=Запланировано
        [HttpGet("status")]
        public async Task<ActionResult<List<Class>>> GetByStatus([FromQuery] string status)
        {
            var classes = await _dbContext.Classes
                .Include(c => c.Group)
                .Include(c => c.Trainer)
                .Where(c => c.Status == status)
                .ToListAsync();

            return Ok(classes);
        }

        // POST: api/classes
        [HttpPost]
        public async Task<ActionResult<Class>> Create(Class classObj)
        {
            var group = await _dbContext.Groups.FindAsync(classObj.Group_id);
            if (group == null)
                return BadRequest(new { message = "Группа не найдена" });

            var trainer = await _dbContext.Trainers.FindAsync(classObj.Trainer_id);
            if (trainer == null)
                return BadRequest(new { message = "Тренер не найден" });

            _dbContext.Classes.Add(classObj);
            await _dbContext.SaveChangesAsync();

            var createdClass = await _dbContext.Classes
                .Include(c => c.Group)
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(c => c.Class_id == classObj.Class_id);

            return CreatedAtAction(nameof(GetById), new { id = classObj.Class_id }, createdClass);
        }

        // PUT: api/classes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Class classObj)
        {
            if (id != classObj.Class_id)
                return BadRequest(new { message = "ID в запросе не совпадает с ID занятия" });

            var existingClass = await _dbContext.Classes.FindAsync(id);
            if (existingClass == null)
                return NotFound(new { message = "Занятие не найдено" });

            existingClass.Group_id = classObj.Group_id;
            existingClass.Trainer_id = classObj.Trainer_id;
            existingClass.Date = classObj.Date;
            existingClass.Time = classObj.Time;
            existingClass.Status = classObj.Status;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/classes/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ClassStatusDto dto)
        {
            var classObj = await _dbContext.Classes.FindAsync(id);
            if (classObj == null)
                return NotFound(new { message = "Занятие не найдено" });

            classObj.Status = dto.Status;
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Статус обновлён" });
        }

        // DELETE: api/classes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var classObj = await _dbContext.Classes.FindAsync(id);
            if (classObj == null)
                return NotFound(new { message = "Занятие не найдено" });

            var hasAttendance = await _dbContext.AttendanceRecords.AnyAsync(a => a.Class_id == id);
            if (hasAttendance)
                return BadRequest(new { message = "Нельзя удалить занятие, у которого есть записи о посещаемости" });

            _dbContext.Classes.Remove(classObj);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Занятие удалено" });
        }
    }

    public class ClassStatusDto
    {
        public string Status { get; set; } = "";
    }
}