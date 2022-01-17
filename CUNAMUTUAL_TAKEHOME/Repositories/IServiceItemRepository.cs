using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Controllers;

namespace CUNAMUTUAL_TAKEHOME.Repositories
{
    public interface IServiceItemRepository
    {
        Task<string> AddServiceItem(ServiceItem serviceItem);
        Task<ServiceItem> UpdateServiceItemStatus(Statuses status, string identifier);
        Task<ServiceItem> GetServiceItem(string identifier);
        Task<ServiceItem> UpdateServiceItem(Statuses status, string detail, string id);
    }
}