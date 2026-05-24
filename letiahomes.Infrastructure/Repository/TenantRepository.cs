using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace letiahomes.Infrastructure.Repository
{
    public class TenantRepository(ApplicationDbContext context):BaseRepository<TenantProfile>(context),ITenantRepository
    {
        private readonly ApplicationDbContext _dbContext = context;

        public async Task<TenantProfile?> GetTenant(string UserId)
        {
            return await _dbContext.TenantProfiles.Where(x => x.AppUserId == UserId)
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync();
        }
    }

}
