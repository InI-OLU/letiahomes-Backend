using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace letiahomes.Infrastructure.Repository
{
    public class LandlordRepository(ApplicationDbContext context):BaseRepository<LandlordProfile>(context), ILandlordRepository
    {
        private readonly ApplicationDbContext _dbContext = context;

        public async Task<LandlordProfile?> GetLandlord(string UserId)
        {
            return await _dbContext.LandlordProfiles.Where(x => x.AppUserId == UserId)
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync();
        }
    }
    
  
}
