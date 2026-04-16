using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.IRepository
{
    public interface IRepositoryManager
    {
        ILandlordRepository Landlords { get; }
        ITenantRepository Tenants { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IPropertyRepository Properties { get; }
        IPropertyImageRepository PropertyImage { get; }

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task RollbackTransactionAsync(IDbContextTransaction transaction);
    }
}
