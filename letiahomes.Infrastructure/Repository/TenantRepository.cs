using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;

namespace letiahomes.Infrastructure.Repository
{
    public class TenantRepository(ApplicationDbContext context):BaseRepository<TenantProfile>(context),ITenantRepository
    {
        private readonly ApplicationDbContext _dbContext = context;
    }
}
