using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Infrastructure.Repository
{
    public class PropertyAmenityRepository(ApplicationDbContext context) : BaseRepository<PropertyAmenity>(context), IPropertyAmenityRepository
    {
        private readonly ApplicationDbContext _dbContext = context;
    }
}
