using Microsoft.EntityFrameworkCore;

namespace CUNAMUTUAL_TAKEHOME.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly DbContext _dbContext;

        
        public ServiceRequestRepository(MyContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}