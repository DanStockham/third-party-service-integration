using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

/**************
 *  Since the third party service is theoretical, I mocked the http message handler on the clients to simulate the request/responses from the
 *  third party service. It is still limited since some of the actions the third party does is callbacks. Therefore, I mimicked those callbacks in
 *  the controller actions themselves.
 */

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