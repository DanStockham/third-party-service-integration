using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Net;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Repositories;
using CUNAMUTUAL_TAKEHOME.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

/*************************************************
 *  My thoughts on the implementation.
 *   - The chose the name proxy because the controller is acting as a passthrough between the consumers the third party service.
 *     The only data we're storing is about the connection and the proxy is not handling the logic like the third party service.
 * 
 *   - There are some response objects that use an integer for the status code without using HttpStatus enum or at least having a constant
 *     that describes what that value is that I'm assigning. Due to time, I ended up using a ContentResult instead.
 * 
 *   - For error handling, I used a try/catch to at least capture the exceptions raised in the web application. However, I think this could
 *     be expanded on depending on what the error is. For instance, if a timeout a http request is raised, that could be used in a retry attempt.
 *     Or if the http service that we're using uses exceptions to describe http errors (like the AWS lambda client) I would explicitly add that exception type
 *     to handle the branch path. Whether that logging the error for debug or operational purposes or sending the client a feedback response.
 * 
 */

namespace CUNAMUTUAL_TAKEHOME.Controllers
{
    public class ProxyController : Controller
    {
        private readonly IProxyService _proxyService;
        private readonly IServiceItemRepository _repo;

        public ProxyController(IProxyService proxyService, IServiceItemRepository repo)
        {
            _proxyService = proxyService;
            _repo = repo;
        }

        /// <summary>Initials the request to the third party service</summary>
        /// <remarks>
        /// This endpoint initiates with the theoretical third party service that this api integrates into.
        /// Because the third party service is being stubbed, the response mimics after the third party finishes its request
        /// against our callback and returns the status string letting us know the request has started. The mock third party http client will
        /// respond differently depend what data is inside `body`. If the `body` contains the string 'success' then the mock http client will respond
        /// with a success but any other string body will respond with internal server error
        /// </remarks>
        /// <param name="serviceItemRequest"></param>
        /// <returns>A string representing the mimicked status after the POST callback is called </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                
                // A quick and dirty debug logging. This is will prevent having to query the sqlite database within the container
                Console.WriteLine($"The service item with identifier {identifier} has been created");
                
                string serviceResponse = await _proxyService.RequestCallback(newServiceItem);

                if (serviceResponse != Statuses.STARTED.ToString())
                {
                    // Logging this would go under a TRACE or INFO scope. I choose a 400 error because this endpoint is expecting 
                    // a status code from the third party service telling it that it has started. Any other status wouldn't be appropriate.
                    
                    return new ContentResult
                    {
                        StatusCode = 400
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
                //If logging was implemented, then we would want to log the exception under a scope related to errors
                return new ContentResult
                {
                    StatusCode = 500
                };
            }
        }

        /// <summary>The POST callback that gets called by the third party service after initial request</summary>
        /// <remarks>
        ///  When the third party service calls back this endpoint, it will also pass back status string of STARTED.
        ///  This endpoint is expecting that status string to be STARTED.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>No Content</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                //If logging was implemented, then we would want to log the exception under a scope related to errors
                return new ContentResult
                {
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// PUT callback will update the status of the service item and save the details about the status update. I'm not sure what details could entail.
        /// It's also possible that the details could be requested from the third party service from this integration instead saving the details to the persistent storage.
        /// </summary>
        /// <remarks>
        ///  The callback will update the request status to either PROCESSED, COMPLETED, or ERROR.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="payload"></param>
        /// <returns>No Content</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("callback/{id}")]
        public async Task<IActionResult> Callback([FromRoute, Required] string id, [FromBody] ServiceItemUpdate payload)
        {
            // Since the third party service is requesting data from this integration. The first thing that need to be done would be authorizing the
            // third party service to use these callback endpoints. This could be done in a middleware.
            try
            {
                // This validation could potential expanded upon based the status in the request. For example, if the third service calls back with COMPLETED
                // And there is business logic that checks that the creation date is 10 business days from today then business logic could be used for the validation.
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
                
                await _repo.UpdateServiceItem(status, payload.Detail, id);
                
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

        /// <summary>Gets the status of the service item or request</summary>
        /// <remarks>
        /// This endpoint gets the status of the service item. All of the properties associated with the service item is stored on persistent storage
        /// However, it's possible that some of the properties needs to be retrieved from the third party. For the context of this assignment, I assumed all fields could be cached or stored 
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Returns the service item stored or cached</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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