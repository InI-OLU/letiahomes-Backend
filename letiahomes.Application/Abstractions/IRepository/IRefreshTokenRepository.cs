using letiahomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.IRepository
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<List<RefreshToken>> GetAllRefreshTokens(string UserId, CancellationToken cancellationToken);
        Task<RefreshToken?> GetRefreshToken(string token, CancellationToken cancellationToken);
    }
}
