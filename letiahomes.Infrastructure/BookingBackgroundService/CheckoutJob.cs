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

        public CheckoutJob(IRepositoryManager repositoryManager,ILogger<CheckoutJob> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        public async Task MarkAsCheckOutJob()
        {
            var now = DateTime.UtcNow;
            var booking = await _repositoryManager.BookingRepository.Get(x => x.Status == BookingStatus.Confirmed && x.CheckOut <= now,true)
                                                                    .Include(x => x.Property)
                                                                    .ToListAsync();
            if(booking.Count == 0)
            {
                _logger.LogInformation("No Booking past expiry found to checkout");
                return;
            }
            _logger.LogInformation(
              "CheckOutJob: found {Count} expired confirmed bookings to checkout.",
              booking.Count);
            foreach (var x in booking)
            {
                x.Status = BookingStatus.Completed;
                var payout = new Payout
                {
                    LandlordProfileId = x.Property.LandlordProfileId,
                    BookingId = x.Id,
                    AmountKobo = x.SubtotalKobo,
                    PlatformFeeKobo = x.PlatformFeeKobo,
                    Status = PayoutStatus.Pending
                };
                await _repositoryManager.PayoutRepository.AddAsync(payout);

                // TODO: notify tenant — "Your stay has ended. We hope you enjoyed it!"
                // e.g. await _mediator.Publish(new BookingCompletedNotification(booking.Id), ct);

                // TODO: notify landlord — "Checkout complete. Payout of {AmountKobo} is being processed."
                // e.g. await _mediator.Publish(new PayoutInitiatedNotification(payout), ct);
            }
        }
    }
}
