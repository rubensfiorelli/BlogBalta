using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        //health check
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }



    }
}
