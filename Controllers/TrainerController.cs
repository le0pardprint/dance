using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]  // ← Trainer вместо Teacher
    [ApiController]
    [Route("api/[controller]")]
    public class TrainerController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TrainerController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/trainer/schedule
        [HttpGet("schedule")]
        public async Task<IActionResult> GetMySchedule()
        {
            // Получаем текущего преподавателя (из JWT)
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var trainer = await _dbContext.Trainers.FirstOrDefaultAsync(t => t.User_id == userId);

            if (trainer == null)
                return NotFound(new { message = "Преподаватель не найден" });

            var schedule = await _dbContext.Classes
                .Include(c => c.Group)
                .Where(c => c.Trainer_id == trainer.Trainer_id)
                .OrderBy(c => c.Date)
                .ThenBy(c => c.Time)
                .Select(c => new
                {
                    c.Class_id,
                    c.Date,
                    c.Time,
                    c.Status,
                    GroupName = c.Group.Name,
                    StudentsCount = c.Group.Registrations.Count
                })
                .ToListAsync();

            return Ok(schedule);
        }

        // GET: api/trainer/my-groups
        [HttpGet("my-groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var trainer = await _dbContext.Trainers.FirstOrDefaultAsync(t => t.User_id == userId);

            if (trainer == null)
                return NotFound(new { message = "Преподаватель не найден" });

            var groups = await _dbContext.Groups
                .Include(g => g.Direction)
                .Where(g => g.Trainer_id == trainer.Trainer_id)
                .Select(g => new
                {
                    g.Group_id,
                    g.Name,
                    DirectionName = g.Direction.Name,
                    g.Status,
                    StudentsCount = g.Registrations.Count,
                    Students = g.Registrations.Select(r => new
                    {
                        r.Client.Client_id,
                        r.Client.LastName,
                        r.Client.FirstName,
                        r.Client.Phone
                    }).ToList()
                })
                .ToListAsync();

            return Ok(groups);
        }

        // POST: api/trainer/attendance
        [HttpPost("attendance")]
        public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var trainer = await _dbContext.Trainers.FirstOrDefaultAsync(t => t.User_id == userId);

            if (trainer == null)
                return NotFound(new { message = "Преподаватель не найден" });

            // Проверяем, что класс принадлежит этому преподавателю
            var classObj = await _dbContext.Classes
                .FirstOrDefaultAsync(c => c.Class_id == dto.Class_id && c.Trainer_id == trainer.Trainer_id);

            if (classObj == null)
                return BadRequest(new { message = "Занятие не найдено или не принадлежит вам" });

            var attendance = new AttendanceRecord
            {
                Client_id = dto.Client_id,
                Class_id = dto.Class_id,
                Status = dto.Status
            };

            _dbContext.AttendanceRecords.Add(attendance);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Посещаемость отмечена" });
        }

        // GET: api/trainer/group-students/{groupId}
        [HttpGet("group-students/{groupId}")]
        public async Task<IActionResult> GetGroupStudents(int groupId)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var trainer = await _dbContext.Trainers.FirstOrDefaultAsync(t => t.User_id == userId);

            if (trainer == null)
                return NotFound(new { message = "Преподаватель не найден" });

            var group = await _dbContext.Groups
                .Include(g => g.Registrations)
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(g => g.Group_id == groupId && g.Trainer_id == trainer.Trainer_id);

            if (group == null)
                return NotFound(new { message = "Группа не найдена или не принадлежит вам" });

            var students = group.Registrations.Select(r => new
            {
                r.Client.Client_id,
                r.Client.LastName,
                r.Client.FirstName,
                r.Client.Phone,
                r.Client.Email
            }).ToList();

            return Ok(students);
        }

        // GET: api/trainer/class-attendance/{classId}
        [HttpGet("class-attendance/{classId}")]
        public async Task<IActionResult> GetClassAttendance(int classId)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var trainer = await _dbContext.Trainers.FirstOrDefaultAsync(t => t.User_id == userId);

            if (trainer == null)
                return NotFound(new { message = "Преподаватель не найден" });

            var classObj = await _dbContext.Classes
                .FirstOrDefaultAsync(c => c.Class_id == classId && c.Trainer_id == trainer.Trainer_id);

            if (classObj == null)
                return BadRequest(new { message = "Занятие не найдено или не принадлежит вам" });

            var attendance = await _dbContext.AttendanceRecords
                .Include(a => a.Client)
                .Where(a => a.Class_id == classId)
                .Select(a => new
                {
                    a.Record_id,
                    a.Status,
                    a.Client_id,
                    ClientName = $"{a.Client.FirstName} {a.Client.LastName}"
                })
                .ToListAsync();

            return Ok(attendance);
        }
    }

    public class MarkAttendanceDto
    {
        public int Client_id { get; set; }
        public int Class_id { get; set; }
        public string Status { get; set; }
    }
}