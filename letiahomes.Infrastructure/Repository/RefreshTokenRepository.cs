using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace letiahomes.Infrastructure.Repository
{
    public class RefreshTokenRepository(ApplicationDbContext context) : BaseRepository<RefreshToken>(context), IRefreshTokenRepository
    {
            private readonly ApplicationDbContext _dbContext = context;
        public async Task<RefreshToken?> GetRefreshToken(string token,CancellationToken cancellationToken)
        {
           return await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
        }
        public async Task<List<RefreshToken>>GetAllRefreshTokens(string UserId,CancellationToken cancellationToken)
        {
            return await _dbContext.RefreshTokens.Where(x => x.UserId == UserId).ToListAsync(cancellationToken);
        }
    }
   
}
