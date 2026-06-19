using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Commands.ConfirmBooking
{
    public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly INotificationService _notificationService;

        public ConfirmBookingCommandHandler(IRepositoryManager repositoryManager,INotificationService notificationService)
        {
            _repositoryManager = repositoryManager;
            _notificationService = notificationService;
        }
        public async Task<ApiResult<string>> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _repositoryManager.BookingRepository.GetBookingByBookingId(request.BookingId);

            if (booking is null)
                return ApiResult<string>.Failure(new CustomError("404", "Booking not found"));
            if (booking.Status != BookingStatus.Pending)
                return ApiResult<string>.Failure(new CustomError("400", $"Booking cannot be confirmed. Current status: {booking.Status}"));
            if(DateTime.UtcNow > booking.ExpiresAt)
                return ApiResult<string>.Failure(new CustomError("400", "This booking has expired. The 24-hour response window has passed."));
            var landlord = await _repositoryManager.Landlords.GetLandlord(request.UserId);
            if (landlord == null || landlord.IsVerified == false )
            {
                return ApiResult<string>.Failure(new CustomError("404", "User not found or IsNotVerified"));
            }
            var tenant = await _repositoryManager.Tenants.GetByIdAsync(booking.TenantProfileId);
            if (tenant is null || tenant.AppUser.IsVerified == false)
                return ApiResult<string>.Failure(new CustomError("404", "User not found or IsNotVerified"));

            var property = await _repositoryManager.Properties.GetByIdAsync(booking.PropertyId);
            if (property is null)
                return ApiResult<string>.Failure(new CustomError("404", "Property not found"));
            if (property.LandlordProfileId != landlord.Id)
                return ApiResult<string>.Failure(new CustomError("403", "User not authorized to confirm booking on property"));
            var isDateAvailable = await _repositoryManager.BookingRepository.HasConflictBookingAsync(booking.PropertyId, booking.CheckIn, booking.CheckOut);
            if (!isDateAvailable)
                return ApiResult<string>.Failure(new CustomError("400", "These dates have been booked"));
            booking.Status = BookingStatus.AwaitingConfirmation;
            booking.ExpiresAt = DateTime.UtcNow.AddHours(2);

            _repositoryManager.BookingRepository.Update(booking);
            await _repositoryManager.SaveChangesAsync();

            // NOTIFICATIONS — after save only 
             _notificationService.EnqueueBookingConfirmedLandlordEmail(new BookingConfirmedLandlordPayload
            (
               tenant.AppUser.Email,
                landlord.AppUser.FirstName,
                tenant.AppUser.FirstName,
                 property.Title,
                 booking.CheckIn,
                booking.CheckOut
            ));
            //The link below to be replaced by paystacklink
            var link = "https://google.come";
            _notificationService.EnqueueBookingConfirmedTenantEmail(new BookingConfirmedTenantPayload(
                tenant.AppUser.Email,
                tenant.AppUser.FirstName,
                  property.Title,
                   booking.CheckIn,
                booking.CheckOut,
                booking.TotalAmountKobo,
                link,
                booking.ExpiresAt
                ));
            // The notification handler will:
            //   - Generate Paystack payment link
            //   - Email tenant: "Booking confirmed — complete payment within 2 hours"
            //   - Email landlord: "You confirmed a booking — tenant will pay shortly"

            return ApiResult<string>.Success("Booking confirmed. Tenant has been notified to complete payment.");
        }
    }
}
