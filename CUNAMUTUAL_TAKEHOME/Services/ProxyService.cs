using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Controllers;
using Newtonsoft.Json;

/*************************************************
 *  My thoughts on the implementation.
 *   - One of the ways to ensure better structure between an integration (proxy/api/etc.) and the service that is consumes
 *     is having a contract between them. It creates an expectation on the integration side and make error handling much easier
 *
 *   - Again, logging could be added around the http requests but time didn't permit :(
 *
 *   - Caching can be handy if a service is going be used frequently. If the third party API was multi-tenant and
 *     there could multiple instances that could negotiated against, the http clients could be cached in memory and
 *     have their own threads.
 */

namespace CUNAMUTUAL_TAKEHOME.Services
{
    public class ProxyService : IProxyService
    {
        private const string baseUrl = "http://example.com/";
        private const string requestRoute = "request";
        private readonly HttpClient _httpClient;

        public ProxyService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ThirdPartyService");
        }
        
        public async Task<string> RequestCallback(ServiceItem serviceItem)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"{baseUrl}{requestRoute}");
            
            var payload = new ThirdPartyRequest
            {
                Body = serviceItem.Body,
                Callback = $"callback/{serviceItem.Identifier}"
            };

            request.Content = new StringContent(
                JsonConvert.SerializeObject(payload)
            );

            var res = await _httpClient.SendAsync(request);

            return await res.Content.ReadAsStringAsync();
        } 
    }
}