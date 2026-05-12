using Microsoft.AspNetCore.Mvc;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Registration>> GetAll()
        {
            var registration = new List<Registration> {
                new Registration
                {
                    Registration_id = 1,
                    Client_id = 1,
                    Group_id = 1,
                    Registration_date = new DateTime(2026, 5, 5, 10, 30, 0)
                                    }
            };

            return Ok(registration);
        }
    }
}