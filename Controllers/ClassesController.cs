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

        // GET: api/classes/status?status=Подтверждено
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
            // Проверяем, существует ли группа
            var group = await _dbContext.Groups.FindAsync(classObj.Group_id);
            if (group == null)
                return BadRequest(new { message = "Группа не найдена" });

            // Проверяем, существует ли тренер
            var trainer = await _dbContext.Trainers.FindAsync(classObj.Trainer_id);
            if (trainer == null)
                return BadRequest(new { message = "Тренер не найден" });

            _dbContext.Classes.Add(classObj);
            await _dbContext.SaveChangesAsync();

            // Загружаем связанные данные для ответа
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

            // Обновляем поля
            existingClass.Group_id = classObj.Group_id;
            existingClass.Trainer_id = classObj.Trainer_id;
            existingClass.Date = classObj.Date;
            existingClass.Time = classObj.Time;
            existingClass.Status = classObj.Status;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/classes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var classObj = await _dbContext.Classes.FindAsync(id);
            if (classObj == null)
                return NotFound(new { message = "Занятие не найдено" });

            // Проверяем, есть ли записи о посещаемости
            var hasAttendance = await _dbContext.AttendanceRecords.AnyAsync(a => a.Class_id == id);
            if (hasAttendance)
                return BadRequest(new { message = "Нельзя удалить занятие, у которого есть записи о посещаемости" });

            _dbContext.Classes.Remove(classObj);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Занятие удалено" });
        }
    }
}