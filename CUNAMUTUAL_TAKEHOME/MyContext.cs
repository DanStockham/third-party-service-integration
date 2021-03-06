using CUNAMUTUAL_TAKEHOME.Controllers;
using Microsoft.EntityFrameworkCore;

namespace CUNAMUTUAL_TAKEHOME
{
    public class MyContext : DbContext
    {
        public DbSet<ServiceItem> ServiceItems { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ThirdPartyDB.db;");
        }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceItem>().ToTable("ServiceItems");
        }
    }
    
    
}