using Hangfire;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Abstractions.Jobs;
using letiahomes.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace letiahomes.Infrastructure.ExternalServices
{
    public class AwaitingPaymentExpiryJob: IAwaitingPaymentExpiryJob
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<AwaitingPaymentExpiryJob> _logger;

        public AwaitingPaymentExpiryJob(
            IRepositoryManager repositoryManager,
            ILogger<AwaitingPaymentExpiryJob> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        public async Task ExpireUnpaidBookingsAsync()
        {
            var now = DateTime.UtcNow;
            var expiredBookingIds = await _repositoryManager.BookingRepository
                .Get(b => b.Status == BookingStatus.AwaitingConfirmation && b.ExpiresAt < now, trackChanges: false)
                .Select(b => b.Id)
                .ToListAsync();

            if (expiredBookingIds.Count == 0)
            {
                _logger.LogInformation("AwaitingPaymentExpiryJob: no expired unpaid bookings found.");
                return;
            }

            _logger.LogInformation(
                "AwaitingPaymentExpiryJob: found {Count} candidates to expire.",
                expiredBookingIds.Count);

            var expiredCount = 0;

            foreach (var bookingId in expiredBookingIds)
            {
                var rowsAffected = await _repositoryManager.BookingRepository
                    .Get(b => b.Id == bookingId && b.Status == BookingStatus.AwaitingConfirmation, trackChanges: false)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Status, BookingStatus.Cancelled)
                        .SetProperty(b => b.CancelledAt, now)
                        .SetProperty(b => b.CancellationReason, "Payment was not completed within the 2-hour window."));

                if (rowsAffected == 0)
                {
                    _logger.LogInformation(
                        "AwaitingPaymentExpiryJob: booking {BookingId} was confirmed " +
                        "just before expiry — skipping.",
                        bookingId);
                    continue;
                }

                expiredCount++;

                // TODO: notify tenant — "Your payment window expired and the
                // booking has been cancelled. Please rebook if you're still interested."
                // e.g. await _mediator.Publish(new BookingPaymentExpiredNotification(bookingId), ct);

                // TODO: notify landlord — booking they confirmed is now
                // cancelled, dates are free again.
                // e.g. await _mediator.Publish(new LandlordBookingExpiredNotification(bookingId), ct);
            }

            _logger.LogInformation(
                "AwaitingPaymentExpiryJob: expired {ExpiredCount} of {CandidateCount} candidates.",
                expiredCount, expiredBookingIds.Count);
        }
    }
}