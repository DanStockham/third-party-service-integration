using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Controllers;

namespace CUNAMUTUAL_TAKEHOME.Services
{
    public interface IProxyService
    {
        Task<string> RequestCallback(ServiceItem serviceItem);
    }
}