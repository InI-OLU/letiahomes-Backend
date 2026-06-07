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
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<CancelBookingCommandHandler> _logger;

        public CancelBookingCommandHandler(IRepositoryManager repositoryManager,ILogger<CancelBookingCommandHandler> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }
        public async Task<ApiResult<string>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _repositoryManager.BookingRepository.GetBookingByBookingId(request.BookingId);
            if (booking is null)
                return ApiResult<string>.Failure(new CustomError("404", "Booking not found"));
            if (booking.Status != BookingStatus.Pending)
                return ApiResult<string>.Failure(new CustomError("400", $"Booking cannot be confirmed. Current status: {booking.Status}"));
            var checkIn = await _repositoryManager.UnavailableDateRepository.GetCheckInDate(booking.Id);
            if (DateTime.UtcNow > checkIn)
                return ApiResult<string>.Failure(new CustomError("400", "booking cannot be cancelled after the check-in date has passed"));
            var tenant = await _repositoryManager.Tenants.GetTenant(request.UserId);
            if (tenant is null)
                return ApiResult<string>.Failure(new CustomError("404", "Tenant not found"));

           if(booking.Status == BookingStatus.Pending)
            {
                var transaction = await _repositoryManager.BeginTransactionAsync();
                try
                {
                    await _repositoryManager.UnavailableDateRepository.ReleaseBookingDatesAsync(booking.Id);
                    booking.Status = BookingStatus.Cancelled;
                    booking.RejectionReason = request.Reason;
                    await _repositoryManager.SaveChangesAsync();
                    await _repositoryManager.CommitTransactionAsync(transaction);
                }
                catch (Exception ex)
                {
                    await _repositoryManager.RollbackTransactionAsync(transaction);
                    _logger.LogError(ex, $"Failed to cancel Booking{booking.Id} by ");
                    throw;
                }
                //Notifies landlord of TenantCancellation through email
            }
           if (booking.Status == BookingStatus.Confirmed)
            {
                var transaction = await _repositoryManager.BeginTransactionAsync();
                try
                {
                    await _repositoryManager.UnavailableDateRepository.ReleaseBookingDatesAsync(booking.Id);
                    booking.Status = BookingStatus.Cancelled;
                    booking.RejectionReason = request.Reason;
                    //A customer refund service would be called here 
                    await _repositoryManager.SaveChangesAsync();
                    await _repositoryManager.CommitTransactionAsync(transaction);
                }
                catch (Exception ex)
                {
                    await _repositoryManager.RollbackTransactionAsync(transaction);
                    _logger.LogError(ex, $"Failed to cancel Booking{booking.Id} by ");
                    throw;
                }
                //Notifies landlord of TenantCancellation through email
            }


            throw new NotImplementedException();
        }
    }
}
