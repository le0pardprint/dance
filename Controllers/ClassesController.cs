using Microsoft.AspNetCore.Mvc;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Class>> GetAll()
        {
            var classes = new List<Class> {
                new Class
                {
                    Class_id = 2,
                    Group_id = 1,
                    Trainer_id = 2,
                    Date = new DateTime(2026, 5, 12),
                    Time = new TimeSpan(14, 30, 0),            // 14:30
                    Status = "Подтверждено"
                }
            };

            return Ok(classes);
        }
    }
}
