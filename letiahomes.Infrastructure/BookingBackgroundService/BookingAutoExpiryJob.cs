using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Abstractions.Jobs;
using letiahomes.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace letiahomes.Infrastructure.ExternalServices
{
    public class BookingAutoExpiryJob: IBookingAutoExpiryJob
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<BookingJobService> _logger;
        private readonly INotificationService _notificationService;

        public BookingAutoExpiryJob(
            IRepositoryManager repositoryManager,
            ILogger<BookingJobService> logger,
            INotificationService notificationService)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
           _notificationService = notificationService;
        }

        public async Task ExpirePendingBookingsAsync()
        {
            var now = DateTime.UtcNow;
            var candidates = await _repositoryManager.BookingRepository
                   .Get(b => b.Status == BookingStatus.Pending && b.ExpiresAt < now, trackChanges: false)
                   .Select(b => new BookingNotificationContext(
                    b.Id,
                    b.Tenant.AppUser.Email,
                    b.Tenant.AppUser.FirstName,
                    b.Property.Landlord.AppUser.Email,
                    b.Property.Landlord.AppUser.FirstName,
                    b.Property.Title,
                    b.CheckIn,
                    b.CheckOut,
                   "Landlord did not respond within 24 hours."))
                   .ToListAsync();
            if (candidates.Count == 0)
            {
                _logger.LogInformation("BookingAutoExpiryJob: no expired pending bookings found.");
                return;
            }

            _logger.LogInformation(
                "BookingAutoExpiryJob: found {Count} expired pending bookings to cancel.",
                candidates.Count);

            var expiredCount = 0;



            foreach (var ctx in candidates)
                {
                var rowsAffected = await _repositoryManager.BookingRepository
                    .Get(b => b.Id == ctx.BookingId && b.Status == BookingStatus.Pending, trackChanges: false)
                    .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.Status, BookingStatus.Cancelled)
                    .SetProperty(b => b.CancelledAt, now)
                    .SetProperty(b => b.CancellationReason, ctx.CancellationReason));

                if (rowsAffected == 0)
                {
                    _logger.LogInformation(
                        "BookingAutoExpiryJob: booking {BookingId} was confirmed just before expiry — skipping.",
                        ctx.BookingId);
                    continue;
                }

                expiredCount++;

                _notificationService.EnqueueBookingCancelledEmail(new BookingCancelledPayload(
                        ctx.TenantEmail,
                        ctx.TenantFirstName,
                        ctx.PropertyTitle,
                        ctx.CheckIn,
                        ctx.CheckOut,
                        CancelledBy: "System",
                        ctx.CancellationReason,
                        RefundAmountKobo: 0,
                        IsRecipientTenant: true));

                    // TODO: notify landlord too on the dashboard
                }
         

          
            await _repositoryManager.SaveChangesAsync();

            _logger.LogInformation(
                "BookingAutoExpiryJob: cancelled {Count} expired pending bookings.",
                candidates.Count);
        }
    }
}
