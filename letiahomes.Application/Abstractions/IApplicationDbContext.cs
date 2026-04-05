using letiahomes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public interface IApplicationDbContext
{
    DbSet<TenantProfile> TenantProfiles { get; }
    DbSet<LandlordProfile> LandlordProfiles { get; }
    DbSet<OtpEntry> OtpEntries { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}