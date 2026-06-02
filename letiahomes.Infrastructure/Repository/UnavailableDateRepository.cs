using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Infrastructure.Repository
{
    public class UnavailableDateRepository(ApplicationDbContext context) : BaseRepository<UnavailableDate>(context), IUnavailableDateRepository
    {
        private readonly ApplicationDbContext _dbContext = context;
        public async Task<bool> IsDateAvailableAsync(Guid  propertyId,DateTime Checkin , DateTime Checkout)
        {
            var hasConflict=  await _dbContext.UnavailableDates
                                                    .AsNoTracking()
                                                    .AnyAsync(x => x.PropertyId == propertyId && 
                                                     x.Date>= Checkin && 
                                                     x.Date < Checkout);
            return !hasConflict;
           
        }
    }

}
