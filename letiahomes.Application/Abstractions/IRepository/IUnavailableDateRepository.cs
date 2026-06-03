using letiahomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.IRepository
{
    public interface IUnavailableDateRepository:IBaseRepository<UnavailableDate>
    {
        Task ReleaseBookingDatesAsync(Guid BookingId);
        Task<bool> IsDateAvailableAsync(Guid propertyId, DateTime Checkin, DateTime Checkout);
        Task<DateTime?> GetCheckInDate(Guid bookingId);
    }
}
