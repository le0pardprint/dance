using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public GroupsController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/groups
        [HttpGet]
        public async Task<ActionResult<List<Group>>> GetAll()
        {
            var groups = await _dbContext.Groups
                .Include(g => g.Direction)
                .Include(g => g.Trainer)
                .ToListAsync();
            return Ok(groups);
        }

        // GET: api/groups/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetById(int id)
        {
            var group = await _dbContext.Groups
                .Include(g => g.Direction)
                .Include(g => g.Trainer)
                .FirstOrDefaultAsync(g => g.Group_id == id);

            if (group == null)
                return NotFound(new { message = "Группа не найдена" });

            return Ok(group);
        }

        // POST: api/groups
        [HttpPost]
        public async Task<ActionResult<Group>> Create(Group group)
        {
            var direction = await _dbContext.Directions.FindAsync(group.Direction_id);
            if (direction == null)
                return BadRequest(new { message = "Направление не найдено" });

            var trainer = await _dbContext.Trainers.FindAsync(group.Trainer_id);
            if (trainer == null)
                return BadRequest(new { message = "Тренер не найден" });

            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var created = await _dbContext.Groups
                .Include(g => g.Direction)
                .Include(g => g.Trainer)
                .FirstOrDefaultAsync(g => g.Group_id == group.Group_id);

            return CreatedAtAction(nameof(GetById), new { id = group.Group_id }, created);
        }

        // PUT: api/groups/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Group group)
        {
            if (id != group.Group_id)
                return BadRequest(new { message = "ID не совпадает" });

            var existing = await _dbContext.Groups.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Группа не найдена" });

            existing.Name = group.Name;
            existing.Direction_id = group.Direction_id;
            existing.Trainer_id = group.Trainer_id;
            existing.Status = group.Status;

            await _dbContext.SaveChangesAsync();
            return Ok(existing);
        }

        // PATCH: api/groups/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] GroupStatusDto dto)
        {
            var group = await _dbContext.Groups.FindAsync(id);
            if (group == null)
                return NotFound(new { message = "Группа не найдена" });

            group.Status = dto.Status;
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Статус обновлён" });
        }

        // DELETE: api/groups/{id} — каскадное удаление
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _dbContext.Groups
                .Include(g => g.Registrations)
                .Include(g => g.Subscriptions)
                .Include(g => g.Classes)
                    .ThenInclude(c => c.AttendanceRecords)
                .FirstOrDefaultAsync(g => g.Group_id == id);

            if (group == null)
                return NotFound(new { message = "Группа не найдена" });

            // Удаляем посещаемость занятий
            foreach (var cls in group.Classes)
                _dbContext.AttendanceRecords.RemoveRange(cls.AttendanceRecords);

            // Удаляем занятия, регистрации, абонементы
            _dbContext.Classes.RemoveRange(group.Classes);
            _dbContext.Registrations.RemoveRange(group.Registrations);
            _dbContext.Subscriptions.RemoveRange(group.Subscriptions);

            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Группа и все связанные данные удалены" });
        }
    }

    public class GroupStatusDto
    {
        public string Status { get; set; } = "";
    }
}