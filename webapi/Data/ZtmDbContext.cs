using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Data
{
    public class ZtmDbContext : DbContext
    {

        public ZtmDbContext(DbContextOptions<ZtmDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true");
            options.EnableSensitiveDataLogging();
        }
    }
}
