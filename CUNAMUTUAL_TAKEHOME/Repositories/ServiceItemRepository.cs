using System;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Controllers;
using Microsoft.EntityFrameworkCore;

namespace CUNAMUTUAL_TAKEHOME.Repositories
{
    public class ServiceItemRepository : IServiceItemRepository
    {
        private readonly MyContext _dbContext;
        
        public ServiceItemRepository(MyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceItem> UpdateServiceItemStatus(Statuses status, string identifier)
        {
            try
            {
                var serviceItem = await _dbContext.ServiceItems.FirstOrDefaultAsync(sr => sr.Identifier == identifier);

                if (serviceItem is null)
                {
                    return null;
                }

                serviceItem.Status = status;
                //_dbContext.ServiceItems.Update(serviceItem);
                await _dbContext.SaveChangesAsync();

                return serviceItem;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ServiceItem> GetServiceItem(string identifier)
        {
            return await _dbContext.ServiceItems.FirstOrDefaultAsync(sr => sr.Identifier == identifier);
        }

        public async Task<ServiceItem> UpdateServiceItem(Statuses status, string detail, string id)
        {
            try
            {
                var serviceItem = await _dbContext.ServiceItems.FirstOrDefaultAsync(sr => sr.Identifier == id);

                if (serviceItem is null)
                {
                    return null;
                }

                serviceItem.Status = status;
                serviceItem.Detail = detail;

                await _dbContext.SaveChangesAsync();

                return serviceItem;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<string> AddServiceItem(ServiceItem serviceItem)
        {
            try
            {
                serviceItem.Identifier = Guid.NewGuid().ToString();
                var newServiceItem = await _dbContext.ServiceItems.AddAsync(serviceItem);
                await _dbContext.SaveChangesAsync();

                return serviceItem.Identifier;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}