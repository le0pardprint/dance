using Microsoft.AspNetCore.Mvc;
using dance.API.Models;

namespace dance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Subscription>> GetAll()
        {
            var subscriptions = new List<Subscription> {
                new Subscription
                {
                    Sub_id = 1,
                    Client_id = 1,
                    Group_id = 1,
                    Amount = 5000.00m,
                    Status = "Активен"
                                    }
            };

            return Ok(subscriptions);
        }
    }
}