using ASC.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace ASC.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public DbSet<MasterDataKey> MasterDataKeys { get; set; }

        public DbSet<MasterDataValue> MasterDataValues { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}