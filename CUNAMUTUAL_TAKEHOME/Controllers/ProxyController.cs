using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Net;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Repositories;
using CUNAMUTUAL_TAKEHOME.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        public async Task<IActionResult> RequestService([FromBody] ServiceItemRequest serviceItemRequest)
        {
            try
            {
                ServiceItem newServiceItem = new ServiceItem()
                {
                    Status = Statuses.NONE,
                    Body = serviceItemRequest.Body
                };

                string identifier = await _repo.AddServiceItem(newServiceItem);

                string serviceResponse = await _thirdPartyService.RequestCallback(newServiceItem);

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

        [HttpPut]
        [Route("callback/{id}")]
        public async Task<IActionResult> Callback([FromRoute, Required] string id, [FromBody] ServiceItemUpdate payload)
        {
            try
            {
                bool validStatus = Statuses.TryParse(payload.Status, out Statuses status);

                if (!validStatus)
                {
                    return new ContentResult
                    {
                        StatusCode = 400,
                        Content =
                            $"status {payload.Status} is not a valid status. Please use the following statuses: PROCESSED, COMPLETED, or ERROR"
                    };
                }
                
                var updatedServiceItem = _repo.UpdateServiceItem(status, payload.Detail, id);
                
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

        [HttpGet]
        [Route("status/{id}")]
        public async Task<IActionResult> GetStatus([FromRoute] string id)
        {
            try
            {
                var serviceItem = await _repo.GetServiceItem(id);

                if (serviceItem is null)
                {
                    return new ContentResult
                    {
                        StatusCode = 404
                    };
                }

                return new ContentResult
                {
                    StatusCode = 200,
                    Content = JsonConvert.SerializeObject(serviceItem),
                    ContentType = "application/json"
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