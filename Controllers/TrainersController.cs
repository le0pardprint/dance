using Microsoft.AspNetCore.Mvc;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Direction>> GetAll()
        {
            var trainers = new List<Trainer> {
                new Trainer
                {
                    Trainer_id = 1,
                    LastName = "Сергей",
                    FirstName = "Сергеев",
                    Direction_id = 1,
                    Phone = "+7-999-987-65-43",
                    Email = "sergey@gmail.com"
                }
            };

            return Ok(trainers);
        }
    }
}