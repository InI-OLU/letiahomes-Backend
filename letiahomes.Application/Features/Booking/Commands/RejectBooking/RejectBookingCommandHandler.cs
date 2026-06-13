using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Commands.RejectBooking
{
    public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<RejectBookingCommandHandler> _logger;

        public RejectBookingCommandHandler(IRepositoryManager repositoryManager,ILogger<RejectBookingCommandHandler> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }
        public async Task<ApiResult<string>> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _repositoryManager.BookingRepository.GetBookingByBookingId(request.BookingId);

            if (booking is null)
                return ApiResult<string>.Failure(new CustomError("404", "Booking not found"));
            if (booking.Status != BookingStatus.Pending)
                return ApiResult<string>.Failure(new CustomError("400", $"Only pending bookings can be rejected. Current status: {booking.Status}"));
            var landlord = await _repositoryManager.Landlords.GetLandlord(request.UserId);
            if (landlord == null || landlord.IsVerified == false)
            {
                return ApiResult<string>.Failure(new CustomError("404", "User not found or IsNotVerified"));
            }
            var property = await _repositoryManager.Properties.GetByIdAsync(booking.PropertyId);
            if (property is null)
                return ApiResult<string>.Failure(new CustomError("404", "Property not found"));
            if (property.LandlordProfileId != landlord.Id)
                return ApiResult<string>.Failure(new CustomError("403", "User not authorized to confirm booking on property"));
           
            var transaction = await _repositoryManager.BeginTransactionAsync();
            try
            {
                booking.Status = BookingStatus.Rejected;
                booking.RejectionReason = request.Reason;
                await _repositoryManager.SaveChangesAsync();
                await _repositoryManager.CommitTransactionAsync(transaction);
            }
            catch(Exception ex)
            {
                await _repositoryManager.RollbackTransactionAsync(transaction);
                _logger.LogError(ex, $"Failed to reject Booking{booking.Id}");
                throw;
            }

            //Send rejection email to Tenant with landlord reasons for rejecting.
            // await _mediator.Publish(new BookingRejectedNotification(booking.Id), cancellationToken);
            return ApiResult<string>.Success("Booking Rejected. Tenant has been notified .");
        }
    }
}
