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

        public BookingAutoExpiryJob(
            IRepositoryManager repositoryManager,
            ILogger<BookingJobService> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        public async Task ExpirePendingBookingsAsync()
        {
            var now = DateTime.UtcNow;
            var expiredBookings = await _repositoryManager.BookingRepository
                .Get(b => b.Status == BookingStatus.Pending && b.ExpiresAt < now, trackChanges: true)
                .ToListAsync();

            if (expiredBookings.Count == 0)
            {
                _logger.LogInformation("BookingAutoExpiryJob: no expired pending bookings found.");
                return;
            }

            _logger.LogInformation(
                "BookingAutoExpiryJob: found {Count} expired pending bookings to cancel.",
                expiredBookings.Count);

            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancelledAt = now;
                booking.CancellationReason = "Landlord did not respond within 24 hours.";

                // TODO: notify tenant — "The landlord did not respond in time.
                // Please try another property." (booking.TenantProfileId)
                // e.g. await _mediator.Publish(new BookingExpiredNotification(booking.Id), ct);
            }

            // One SaveChangesAsync for the whole batch — not per booking.
            await _repositoryManager.SaveChangesAsync();

            _logger.LogInformation(
                "BookingAutoExpiryJob: cancelled {Count} expired pending bookings.",
                expiredBookings.Count);
        }
    }
}
