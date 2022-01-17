using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Controllers;
using Newtonsoft.Json;

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