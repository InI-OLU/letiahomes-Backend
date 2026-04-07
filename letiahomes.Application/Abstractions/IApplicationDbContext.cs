using letiahomes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public interface IApplicationDbContext
{
    DbSet<TenantProfile> TenantProfiles { get; }
    DbSet<LandlordProfile> LandlordProfiles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}