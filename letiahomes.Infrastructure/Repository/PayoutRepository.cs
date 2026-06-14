using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;

namespace letiahomes.Infrastructure.Repository
{
    public class PayoutRepository(ApplicationDbContext context) : BaseRepository<Payout>(context), IPayoutRepository
    {
        private readonly ApplicationDbContext _dbContext = context;
    }
}