using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Commands.CancelBooking
{
    public class TenantCancelBookingCommandHandler : IRequestHandler<TenantCancelBookingCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<TenantCancelBookingCommandHandler> _logger;

        public TenantCancelBookingCommandHandler(IRepositoryManager repositoryManager,ILogger<TenantCancelBookingCommandHandler> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }
        public async Task<ApiResult<string>> Handle(TenantCancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _repositoryManager.BookingRepository.GetBookingByBookingId(request.BookingId);
            if (booking is null)
                return ApiResult<string>.Failure(new CustomError("404", "Booking not found"));
            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
                return ApiResult<string>.Failure(new CustomError("400", $"Booking cannot be cancelled. Current status: {booking.Status}"));
            if (DateTime.UtcNow.Date > booking.CheckIn.Date)
                return ApiResult<string>.Failure(new CustomError("400", "Booking cannot be cancelled after the check-in date has passed"));
            var tenant = await _repositoryManager.Tenants.GetTenant(request.UserId);
            if (tenant is null)
                return ApiResult<string>.Failure(new CustomError("404", "Tenant not found"));
            if (booking.TenantProfileId != tenant.Id)
                return ApiResult<string>.Failure(new CustomError("403", "You are not authorised to cancel this booking"));

            //Refund Calculation --------------------------------------------
            long refundAmountKobo = 0;
            if (booking.Status == BookingStatus.Confirmed)
            {
                var daysUntilCheckIn = (booking.CheckIn.Date - DateTime.UtcNow.Date).Days;
                refundAmountKobo = daysUntilCheckIn switch
                {
                    > 7 => booking.SubtotalKobo,                    // 100% refund
                    >= 3 => (long)(booking.SubtotalKobo * 0.5m),    // 50% refund
                    _ => 0                                         // no refund
                };
            }
            var transaction = await _repositoryManager.BeginTransactionAsync();
            try
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancelledAt = DateTime.UtcNow;
                booking.CancellationReason = request.Reason;

                await _repositoryManager.SaveChangesAsync();
                await _repositoryManager.CommitTransactionAsync(transaction);
            }
            catch (Exception ex)
            {
                await _repositoryManager.RollbackTransactionAsync(transaction);
                _logger.LogError(ex, "Failed to cancel booking {BookingId} by tenant {TenantId}", booking.Id, tenant.Id);
                throw;
            }


            if (booking.Status == BookingStatus.Confirmed && refundAmountKobo > 0)
            {
                // await _paymentService.RefundAsync(booking.PaymentReference, refundAmountKobo);
            }

            //Notifies landlord of TenantCancellation through email



            return ApiResult<string>.Success("Booking cancelled successfully.");
        }
    }
}
