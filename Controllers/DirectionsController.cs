using Microsoft.AspNetCore.Mvc;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectionsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Direction>> GetAll()
        {
            var directions = new List<Direction> {
                new Direction
                {
                    Direction_id = 1,
                    Name = "Хип-хоп",
                    Description = "Современный уличный танец",
                    AgeGroup = "Дети",
                    Level = "Начальный"
                }
            };

            return Ok(directions);
        }
    }
}
