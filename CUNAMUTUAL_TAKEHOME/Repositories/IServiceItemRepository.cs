using System.Threading.Tasks;

namespace CUNAMUTUAL_TAKEHOME.Repositories
{
    public interface IServiceItemRepository
    {
        Task<string> AddServiceItem(ServiceItem serviceItem);
        Task<ServiceItem> UpdateServiceItemStatus(Statuses status, string identifier);
        Task<ServiceItem> GetServiceItem(string identifier);
    }
}