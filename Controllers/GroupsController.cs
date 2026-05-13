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

        // Конструктор получает доступ к базе данных
        public GroupsController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/groups
        [HttpGet]
        public async Task<ActionResult<List<Group>>> GetAll()
        {
            // Берём группы из базы данных (с подгрузкой направлений и тренеров)
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
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = group.Group_id }, group);
        }

        // PUT: api/groups/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Group group)
        {
            if (id != group.Group_id)
                return BadRequest();

            _dbContext.Entry(group).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/groups/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _dbContext.Groups.FindAsync(id);
            if (group == null)
                return NotFound();

            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}