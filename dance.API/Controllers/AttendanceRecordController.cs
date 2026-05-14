using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceRecordController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AttendanceRecordController(AppDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/AttendanceRecord
        [HttpGet]
        public async Task<ActionResult<List<AttendanceRecord>>> GetAll()
        {
            var attendanceRecords = await _dbContext.AttendanceRecords
                .Include(a => a.Client)
                .Include(a => a.Class)
                .ThenInclude(c => c.Group)
                .ToListAsync();

            return Ok(attendanceRecords);
        }

        // GET: api/AttendanceRecord/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceRecord>> GetById(int id)
        {
            var attendanceRecord = await _dbContext.AttendanceRecords
                .Include(a => a.Client)
                .Include(a => a.Class)
                .ThenInclude(c => c.Group)
                .FirstOrDefaultAsync(a => a.Record_id == id);

            if (attendanceRecord == null)
                return NotFound(new { message = "Запись посещаемости не найдена" });

            return Ok(attendanceRecord);
        }

        // GET: api/AttendanceRecord/byclient/{clientId}
        [HttpGet("byclient/{clientId}")]
        public async Task<ActionResult<List<AttendanceRecord>>> GetByClientId(int clientId)
        {
            var attendanceRecords = await _dbContext.AttendanceRecords
                .Include(a => a.Client)
                .Include(a => a.Class)
                .ThenInclude(c => c.Group)
                .Where(a => a.Client_id == clientId)
                .ToListAsync();

            return Ok(attendanceRecords);
        }

        // GET: api/AttendanceRecord/byclass/{classId}
        [HttpGet("byclass/{classId}")]
        public async Task<ActionResult<List<AttendanceRecord>>> GetByClassId(int classId)
        {
            var attendanceRecords = await _dbContext.AttendanceRecords
                .Include(a => a.Client)
                .Include(a => a.Class)
                .ThenInclude(c => c.Group)
                .Where(a => a.Class_id == classId)
                .ToListAsync();

            return Ok(attendanceRecords);
        }

        // GET: api/AttendanceRecord/status?status=Присутствовал
        [HttpGet("status")]
        public async Task<ActionResult<List<AttendanceRecord>>> GetByStatus([FromQuery] string status)
        {
            var attendanceRecords = await _dbContext.AttendanceRecords
                .Include(a => a.Client)
                .Include(a => a.Class)
                .ThenInclude(c => c.Group)
                .Where(a => a.Status == status)
                .ToListAsync();

            return Ok(attendanceRecords);
        }

        // POST: api/AttendanceRecord
        [HttpPost]
        public async Task<ActionResult<AttendanceRecord>> Create(AttendanceRecord attendanceRecord)
        {
            // Проверяем, существует ли клиент
            var client = await _dbContext.Clients.FindAsync(attendanceRecord.Client_id);
            if (client == null)
                return BadRequest(new { message = "Клиент не найден" });

            // Проверяем, существует ли занятие
            var classObj = await _dbContext.Classes.FindAsync(attendanceRecord.Class_id);
            if (classObj == null)
                return BadRequest(new { message = "Занятие не найдено" });

            _dbContext.AttendanceRecords.Add(attendanceRecord);
            await _dbContext.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            var createdRecord = await _dbContext.AttendanceRecords
                .Include(a => a.Client)
                .Include(a => a.Class)
                .ThenInclude(c => c.Group)
                .FirstOrDefaultAsync(a => a.Record_id == attendanceRecord.Record_id);

            return CreatedAtAction(nameof(GetById), new { id = attendanceRecord.Record_id }, createdRecord);
        }

        // PUT: api/AttendanceRecord/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AttendanceRecord attendanceRecord)
        {
            if (id != attendanceRecord.Record_id)
                return BadRequest(new { message = "ID в запросе не совпадает с ID записи" });

            var existingRecord = await _dbContext.AttendanceRecords.FindAsync(id);
            if (existingRecord == null)
                return NotFound(new { message = "Запись посещаемости не найдена" });

            // Обновляем поля
            existingRecord.Client_id = attendanceRecord.Client_id;
            existingRecord.Class_id = attendanceRecord.Class_id;
            existingRecord.Status = attendanceRecord.Status;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/AttendanceRecord/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var attendanceRecord = await _dbContext.AttendanceRecords.FindAsync(id);
            if (attendanceRecord == null)
                return NotFound(new { message = "Запись посещаемости не найдена" });

            _dbContext.AttendanceRecords.Remove(attendanceRecord);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Запись посещаемости удалена" });
        }
    }
}