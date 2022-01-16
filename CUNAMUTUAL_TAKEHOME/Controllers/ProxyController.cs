using Microsoft.AspNetCore.Mvc;

namespace CUNAMUTUAL_TAKEHOME.Controllers
{
    public class ProxyController : Controller
    {
        // POST
        [Route("request")]
        public IActionResult Index(Request request)
        {
            return Ok();
        }
    }

    public class Request
    {
        public string Body { get; set; }
    }
}