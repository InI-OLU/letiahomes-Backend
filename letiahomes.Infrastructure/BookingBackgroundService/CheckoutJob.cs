using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Abstractions.Jobs;
using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Infrastructure.BookingBackgroundService
{
    public class CheckoutJob:ICheckoutJob
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<CheckoutJob> _logger;
        private readonly INotificationService _notificationService;

        public CheckoutJob(IRepositoryManager repositoryManager,ILogger<CheckoutJob> logger,
                           INotificationService notificationService)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task MarkAsCheckOutJob()
        {
            var now = DateTime.UtcNow;

            var expiredBookings = await _repositoryManager.BookingRepository
                .Get(x => x.Status == BookingStatus.Confirmed && x.CheckOut <= now, trackChanges: true)
                .Include(x => x.Property).ThenInclude(p => p.Landlord).ThenInclude(l => l.AppUser)
                .Include(x => x.Tenant).ThenInclude(t => t.AppUser)
                .ToListAsync();

            if (expiredBookings.Count == 0)
            {
                _logger.LogInformation("CheckoutJob: no expired confirmed bookings found to checkout.");
                return;
            }

            _logger.LogInformation(
                "CheckoutJob: found {Count} expired confirmed bookings to checkout.",
                expiredBookings.Count);

            var contexts = new List<BookingNotificationContext>();

            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.Completed;

                var payout = new Payout
                {
                    LandlordProfileId = booking.Property.LandlordProfileId,
                    BookingId = booking.Id,
                    AmountKobo = booking.SubtotalKobo,
                    PlatformFeeKobo = booking.PlatformFeeKobo,
                    Status = PayoutStatus.Pending
                };

                await _repositoryManager.PayoutRepository.AddAsync(payout);

                contexts.Add(new BookingNotificationContext(
                    booking.Id,
                    booking.Tenant.AppUser.Email,
                    booking.Tenant.AppUser.FirstName,
                    booking.Property.Landlord.AppUser.Email,
                    booking.Property.Landlord.AppUser.FirstName,
                    booking.Property.Title,
                    booking.CheckIn,
                    booking.CheckOut,
                    CancellationReason: null));
            }

            await _repositoryManager.SaveChangesAsync();

            foreach (var ctx in contexts)
            {
                _notificationService.EnqueueBookingCompletedTenantEmail(new BookingCompletedTenantPayload(
                    ctx.TenantEmail,
                    ctx.TenantFirstName,
                    ctx.PropertyTitle,
                    ctx.CheckOut));

                _notificationService.EnqueueBookingCompletedLandlordEmail(new BookingCompletedLandlordPayload(
                    ctx.LandlordEmail,
                    ctx.LandlordFirstName,
                    ctx.PropertyTitle,
                    ctx.CheckOut,
                    PayoutAmountKobo:0));
            }

            _logger.LogInformation(
                "CheckoutJob: checked out {Count} bookings.",
                expiredBookings.Count);
        }

    }
}
