using Microsoft.AspNetCore.Mvc;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Group>> GetAll()
        {
            var groups = new List<Group> {
                new Group
                {
                    Group_id = 1,
                    Name = "Первая",
                    Direction_id = 1,
                    Trainer_id = 1,
                    Status = "Активна"
                }
            };

            return Ok(groups);
        }
    }
}