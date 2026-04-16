using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Infrastructure.Data;


namespace letiahomes.Infrastructure.Repository
{
    public class LandlordRepository(ApplicationDbContext context):BaseRepository<LandlordProfile>(context), ILandlordRepository
    {
        private readonly ApplicationDbContext _dbContext = context;
    }
}
