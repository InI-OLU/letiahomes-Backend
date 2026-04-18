using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;


namespace letiahomes.Infrastructure.Repository
{
    public class PropertyImageRepository(ApplicationDbContext dbContext) : BaseRepository<PropertyImage>(dbContext), IPropertyImageRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
    }
}
