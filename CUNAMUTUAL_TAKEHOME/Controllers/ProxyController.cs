using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Services;
using Microsoft.AspNetCore.Mvc;

namespace CUNAMUTUAL_TAKEHOME.Controllers
{
    public class ProxyController : Controller
    {
        private IProxyService _thirdPartyService;

        public ProxyController(IProxyService proxyService)
        {
            _thirdPartyService = proxyService;
        }
        
        [HttpPost]
        [Route("request")]
        public async Task<IActionResult> RequestService([FromBody] ServiceRequest serviceRequest)
        {
            string serviceResponse = await _thirdPartyService.RequestCallback(serviceRequest);
            var response = new ContentResult
            {
                StatusCode = 200,
                Content = serviceResponse
            };
            
            return response;
        }
    }
}