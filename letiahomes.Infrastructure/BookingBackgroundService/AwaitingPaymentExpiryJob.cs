using Hangfire;
using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Abstractions.Jobs;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace letiahomes.Infrastructure.ExternalServices
{
    public class AwaitingPaymentExpiryJob: IAwaitingPaymentExpiryJob
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<AwaitingPaymentExpiryJob> _logger;
        private readonly INotificationService _notificationService;

        public AwaitingPaymentExpiryJob(
            IRepositoryManager repositoryManager,
            ILogger<AwaitingPaymentExpiryJob> logger,
            INotificationService notificationService)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task ExpireUnpaidBookingsAsync()
        {
            var now = DateTime.UtcNow;
            var candidates = await _repositoryManager.BookingRepository
                .Get(b => b.Status == BookingStatus.AwaitingConfirmation && b.ExpiresAt < now, trackChanges: false)
                .Select(b => new BookingNotificationContext(
            b.Id,
            b.Tenant.AppUser.Email,
            b.Tenant.AppUser.FirstName,
            b.Property.Landlord.AppUser.Email,
            b.Property.Landlord.AppUser.FirstName,
            b.Property.Title,
            b.CheckIn,
            b.CheckOut,
            "Payment was not completed within the 2-hour window."))
                .ToListAsync();
           

            if (candidates.Count == 0)
            {
                _logger.LogInformation("AwaitingPaymentExpiryJob: no expired unpaid bookings found.");
                return;
            }

            _logger.LogInformation(
                "AwaitingPaymentExpiryJob: found {Count} candidates to expire.",
                candidates.Count);

            var expiredCount = 0;

            foreach (var ctx in candidates)
            {

                var rowsAffected = await _repositoryManager.BookingRepository
             .Get(b => b.Id == ctx.BookingId && b.Status == BookingStatus.AwaitingConfirmation, trackChanges: false)
             .ExecuteUpdateAsync(setters => setters
                 .SetProperty(b => b.Status, BookingStatus.Cancelled)
                 .SetProperty(b => b.CancelledAt, now)
                 .SetProperty(b => b.CancellationReason, ctx.CancellationReason));

                if (rowsAffected == 0)
                {
                    _logger.LogInformation(
                        "AwaitingPaymentExpiryJob: booking {BookingId} was confirmed just before expiry — skipping.",
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
                expiredCount++;

                _notificationService.EnqueueBookingCancelledEmail(new BookingCancelledPayload(
                     ctx.LandlordEmail,
                     ctx.LandlordFirstName,
                     ctx.PropertyTitle,
                     ctx.CheckIn,
                     ctx.CheckOut,
                     CancelledBy: "System",
                     ctx.CancellationReason,
                     RefundAmountKobo: 0,
                     IsRecipientTenant: false));
                expiredCount++;
            }

            _logger.LogInformation(
                "AwaitingPaymentExpiryJob: expired {ExpiredCount} of {CandidateCount} candidates.",
                expiredCount, candidates.Count);
        }
    }
}