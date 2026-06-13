using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using letiahomes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Infrastructure.Repository
{
    public sealed class BookingRepository(ApplicationDbContext context) : BaseRepository<Booking>(context), IBookingRepository
    {
        private readonly ApplicationDbContext _dbContext = context;

        public async Task<Booking?> GetBookingByBookingId (Guid BookingId)
        {
            return await _dbContext.Bookings.Where(x => x.Id == BookingId)
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync();
        }
        
        public async Task<bool> HasConflictBookingAsync (Guid propertyId, DateTime Checkin, DateTime CheckOut)
        {
            return await _dbContext.Bookings
                  .AnyAsync(b =>
                        b.PropertyId == propertyId &&
                        b.CheckIn >= Checkin && b.CheckOut < CheckOut &&
                        b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Rejected);
        }
    }
}
