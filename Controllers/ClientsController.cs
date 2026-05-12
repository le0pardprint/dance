using Microsoft.AspNetCore.Mvc;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Client>> GetAll()
        {
            var clients = new List<Client> {
                new Client
                {
                    Client_id = 1,
                    LastName = "Иванов",
                    FirstName = "Иван",
                    Age = 12,
                    Phone = "+7-999-123-45-67",
                    Email = "ivan@gmail.com"
                }
            };

            return Ok(clients);
        }
    }
}
