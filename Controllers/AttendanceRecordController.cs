using Microsoft.AspNetCore.Mvc;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceRecordController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<AttendanceRecord>> GetAll()
        {
            var attendancerecord = new List<AttendanceRecord> {
                new AttendanceRecord
                {
                    Record_id = 1,
                    Client_id = 1,
                    Class_id = 1,
                    Status = "Присутствовал"
                                    }
            };

            return Ok(attendancerecord);
        }
    }
}