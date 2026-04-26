using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Infrastructure.Data;
using letiahomes.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace letiahomes.Infrastructure.RepsitoryManager
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly ApplicationDbContext _dbContext;

       
        private ILandlordRepository? _landlordRepository;
        private ITenantRepository? _tenantRepository;
        private IRefreshTokenRepository? _refreshTokenRepository;
        private IPropertyRepository? _propertyRepository;
        private IPropertyImageRepository? _propertyImageRepository;
        private IPropertyAmenityRepository? _propertyAmenityRepository;

        public RepositoryManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ILandlordRepository Landlords =>
            _landlordRepository ??= new LandlordRepository(_dbContext);

        public ITenantRepository Tenants =>
            _tenantRepository ??= new TenantRepository(_dbContext);
        public IRefreshTokenRepository RefreshTokens =>
            _refreshTokenRepository ??= new RefreshTokenRepository(_dbContext);

        public IPropertyRepository Properties => 
            _propertyRepository ??= new PropertyRepository(_dbContext);
        public IPropertyImageRepository PropertyImage =>
            _propertyImageRepository ??= new PropertyImageRepository(_dbContext);
        public IPropertyAmenityRepository PropertyAmenity =>
            _propertyAmenityRepository ??= new PropertyAmenityRepository(_dbContext);
        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.RollbackAsync();
        }
    }
}
