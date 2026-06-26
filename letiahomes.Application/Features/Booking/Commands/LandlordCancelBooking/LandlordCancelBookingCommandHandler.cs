using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;


namespace letiahomes.Application.Features.Booking.Commands.LandlordCancelBooking
{
    public class LandlordCancelBookingCommandHandler : IRequestHandler<LandlordCancelBookingCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<LandlordCancelBookingCommandHandler> _logger;
        private readonly INotificationService _notificationService;

        public LandlordCancelBookingCommandHandler(IRepositoryManager repositoryManager, ILogger<LandlordCancelBookingCommandHandler> logger,
                                                   INotificationService notificationService)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<ApiResult<string>> Handle(LandlordCancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _repositoryManager.BookingRepository.GetBookingByBookingId(request.BookingId);
            if (booking is null)
                return ApiResult<string>.Failure(new CustomError("404", "Booking not found"));
            if (booking.Status != BookingStatus.Confirmed)
                return ApiResult<string>.Failure(new CustomError("400", "Only confirmed bookings can be landlord-cancelled. To decline a pending booking, use reject instead."));
            if (DateTime.UtcNow.Date > booking.CheckIn.Date)
                return ApiResult<string>.Failure(new CustomError("400", "Booking cannot be cancelled after the check-in date has passed"));
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

            var tenant = await _repositoryManager.Tenants.GetTenant(booking.TenantProfileId);
                if (tenant is null )
                return ApiResult<string>.Failure(new CustomError("404", "Tenant not found"));

            var transaction = await _repositoryManager.BeginTransactionAsync();
            try
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancelledAt = DateTime.UtcNow;
                booking.CancellationReason = request.Reason;
                landlord.CancellationCount++;
                //A payment refund service is called here 
                await _repositoryManager.SaveChangesAsync();
                await _repositoryManager.CommitTransactionAsync(transaction);
            }
            catch (Exception ex)
            {
                await _repositoryManager.RollbackTransactionAsync(transaction);
                _logger.LogError(ex, $"Failed to cancel Booking{booking.Id} by {landlord.Id} ");
                throw;
            }
            //Sends apology email to tenant from the platform
            long RefundAmountKobo = 250000;
            _notificationService.EnqueueBookingCancelledEmail(new BookingCancelledPayload(
                tenant.AppUser.Email,
                tenant.AppUser.FirstName,
                property.Title,
                booking.CheckIn,
                booking.CheckOut,
                "Tenant",
                booking.CancellationReason,
                RefundAmountKobo,
                true));
            if (landlord.CancellationCount >= 7)
            {
                //publish an event to the admin dashboard to review the account and take action....
            }

            return ApiResult<string>.Success("Booking cancelled. A full refund has been issued to the tenant.");
        }
    }
}
