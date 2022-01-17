using System;
using System.Data.Common;
using System.Net;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Repositories;
using CUNAMUTUAL_TAKEHOME.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CUNAMUTUAL_TAKEHOME.Controllers
{
    public class ProxyController : Controller
    {
        private IProxyService _thirdPartyService;
        private IServiceItemRepository _repo;

        public ProxyController(IProxyService thirdPartyService, IServiceItemRepository repo)
        {
            _thirdPartyService = thirdPartyService;
            _repo = repo;
        }
        
        [HttpPost]
        [Route("request")]
        public async Task<IActionResult> RequestService([FromBody] ServiceRequest serviceRequest)
        {
            try
            {
                
                ServiceItem newServiceItem = new ServiceItem()
                {
                    Status = Statuses.NONE,
                };
                
                string identifier = await _repo.AddServiceItem(newServiceItem);

                serviceRequest.Id = identifier;
                
                string serviceResponse = await _thirdPartyService.RequestCallback(serviceRequest);
                
                if (serviceResponse != Statuses.STARTED.ToString())
                {
                    return new ContentResult
                    {
                        StatusCode = 500
                    };
                }
                
                var response = new ContentResult
                {
                    StatusCode = 200,
                    Content = serviceResponse
                };

                return response;
            }
            catch (Exception ex)
            {
                return new ContentResult
                {
                    StatusCode = 500
                };
            }
            
        }

        [HttpPost]
        [Route("callback/{id}")]
        public async Task<IActionResult> Callback([FromRoute] string id, [FromBody] string status)
        {
            try
            {
                var storedServiceItem = await _repo.GetServiceItem(id);

                if (storedServiceItem is null)
                {
                    return new ContentResult()
                    {
                        StatusCode = 404,
                        Content = string.Empty
                    };
                }

                Statuses.TryParse(status, out Statuses updatedStatus);
                
                await _repo.UpdateServiceItemStatus(updatedStatus, storedServiceItem.Identifier);

                return new ContentResult
                {
                    StatusCode = 204
                };
            }
            catch (Exception ex)
            {
                return new ContentResult
                {
                    StatusCode = 500
                };
            }
            
            
        }
    }
}