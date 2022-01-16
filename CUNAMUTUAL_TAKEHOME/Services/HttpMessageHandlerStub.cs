using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CUNAMUTUAL_TAKEHOME.Services
{
    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var content = await request.Content.ReadAsStringAsync();
            var req = JsonConvert.DeserializeObject<ThirdPartyRequest>(content);
            HttpResponseMessage res;

            switch (req.Body)
            {
                case "success":
                    res = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("STARTED")
                    };
                    break;
                case "failed":
                    res = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent("ERROR")
                    };
                    break;
                default:
                    res = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent("ERROR")
                    };
                    break;
            }

            return res;
        }
    }
}