using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Controllers;
using CUNAMUTUAL_TAKEHOME.Repositories;
using Newtonsoft.Json;

namespace CUNAMUTUAL_TAKEHOME.Services
{
    public class ProxyService : IProxyService
    {
        private const string baseUrl = "http://example.com/";
        private const string requestRoute = "request";
        private readonly HttpClient _httpClient;
        private readonly IServiceRequestRepository _repo;

        public ProxyService(IServiceRequestRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _httpClient = httpClientFactory.CreateClient();
        }
        
        public async Task<string> RequestCallback(ServiceRequest serviceRequest)
        {
            var request = new HttpRequestMessage();
            var uri = new Uri($"{baseUrl}{requestRoute}");
            
            var payload = new ThirdPartyRequest
            {
                Body = serviceRequest.Body,
                Callback = uri.AbsolutePath
            };

            request.Content = new StringContent(
                JsonConvert.SerializeObject(payload)
            );

            var res = await _httpClient.SendAsync(request);

            return res.Content.ToString();
        } 
    }
}