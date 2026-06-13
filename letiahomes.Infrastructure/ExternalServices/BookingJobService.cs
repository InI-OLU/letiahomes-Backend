using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace letiahomes.Infrastructure.ExternalServices
{
    // ── The job itself ───────────────────────────────────────────────────────────
    // This is a plain DI-friendly class, NOT a MediatR handler — nothing is
    // "requesting" this, Hangfire calls it on a schedule. It has its own
    // narrow responsibility: expire stale Pending bookings and notify tenants.
    // No refund logic belongs here — nothing was ever paid at Pending stage.
    public class BookingAutoExpiryJob
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<BookingAutoExpiryJob> _logger;

        public BookingAutoExpiryJob(
            IRepositoryManager repositoryManager,
            ILogger<BookingAutoExpiryJob> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        // Hangfire calls this method on its configured schedule.
        // Public, no return value needed beyond Task — Hangfire just awaits it.
        public async Task ExpirePendingBookingsAsync()
        {
            var now = DateTime.UtcNow;

            // Find all Pending bookings whose response window has passed.
            // AsNoTracking would normally apply here, but we need EF to track
            // these entities since we're about to modify and save them.
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

                // NOTE: nothing to "release" in UnavailableDate — that table no
                // longer exists. Booking.Status = Cancelled is enough; your
                // overlap check already excludes Cancelled bookings.

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
 