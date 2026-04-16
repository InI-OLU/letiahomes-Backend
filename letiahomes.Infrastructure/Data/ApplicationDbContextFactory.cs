using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace letiahomes.Infrastructure.Data
{
        public class ApplicationDbContextFactory
      : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

              

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=letiahomes;Username=postgres;Password=ini@15555");

                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }
    
}