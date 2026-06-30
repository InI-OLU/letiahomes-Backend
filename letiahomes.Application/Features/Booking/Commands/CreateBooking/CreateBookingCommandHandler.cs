using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace letiahomes.Application.Features.Booking.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager  _repositoryManager;
        private readonly INotificationService _notificationService;

        public CreateBookingCommandHandler(IRepositoryManager repositoryManager,INotificationService notificationService)
        {
            _repositoryManager = repositoryManager;
            _notificationService = notificationService;
        }
        public async Task<ApiResult<string>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {

            var today = DateTime.UtcNow.Date;
            var checkIn = request.Request.CheckIn.Date;
            var checkOut = request.Request.CheckOut.Date;

            if (checkIn <= today)
                return ApiResult<string>.Failure(new CustomError("400", "Check-in must be at least 1 day in the future"));

            if (checkOut <= checkIn)
                return ApiResult<string>.Failure(new CustomError("400", "Check-out must be after check-in"));
            var nights = (int)(checkOut - checkIn).TotalDays;

            if (nights > 90)
                return ApiResult<string>.Failure(new CustomError("400", "Maximum booking duration is 90 nights"));

            var tenant = await _repositoryManager.Tenants.GetTenant(request.UserId);
            var property = await _repositoryManager.Properties.GetByIdAsync(request.Request.PropertyId);
            if (tenant == null || tenant.AppUser.IsActive == false || tenant.AppUser.IsVerified == false)
            {
                return ApiResult<string>.Failure(new CustomError("400", "User not Permitted to make booking"));
            }
            if (property == null ||property.IsAvailable == false || property.IsApproved == false )
            {
                return ApiResult<string>.Failure(new CustomError("400", "Property not available for booking"));
            }
            var landlord = await _repositoryManager.Landlords.GetByIdAsync(property.LandlordProfileId);
            if (landlord is null )
                return ApiResult<string>.Failure(new CustomError("404", "Landlord not found"));
            var pendingCount = tenant.Bookings.Count(b => b.Status == BookingStatus.Pending);
            if (pendingCount >= 3)
                return ApiResult<string>.Failure(new CustomError("400", "You cannot have more than 3 pending bookings at a time"));
            var isDateAvailable = await _repositoryManager.BookingRepository.HasConflictBookingAsync(request.Request.PropertyId, request.Request.CheckIn, request.Request.CheckOut);
                if (isDateAvailable)
                return ApiResult<string>.Failure(new CustomError("400", "These dates have been booked"));

            const decimal platformFeePercent = 0.10m; 

            var subtotalKobo = nights * property.PricePerNightKobo;
            var platformFeeKobo = (long)(subtotalKobo * platformFeePercent);
            var totalAmountKobo = subtotalKobo + platformFeeKobo;

            // ─── 8. CREATE THE BOOKING ────────────────────────────────────────
            var booking = new Domain.Entities.Booking
            {
                PropertyId = request.Request.PropertyId,
                TenantProfileId = tenant.Id,
                CheckIn = checkIn,
                CheckOut = checkOut,
                NumberOfGuests = request.Request.NumberOfGuests,
                NightsCount = nights,
                SubtotalKobo = subtotalKobo,
                PlatformFeeKobo = platformFeeKobo,
                TotalAmountKobo = totalAmountKobo,
                Status = BookingStatus.Pending,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            var transaction = await _repositoryManager.BeginTransactionAsync();
            try
            {
                var isDateAvailableCheck = await _repositoryManager.BookingRepository.HasConflictBookingAsync(request.Request.PropertyId, request.Request.CheckIn, request.Request.CheckOut);
                if (isDateAvailableCheck)
                    return ApiResult<string>.Failure(new CustomError("400", "These dates have been booked"));

                await _repositoryManager.BookingRepository.AddAsync(booking);

                for (var date = checkIn; date < checkOut; date = date.AddDays(1))
                {
                    await _repositoryManager.UnavailableDateRepository.AddAsync(new UnavailableDate
                    {
                        PropertyId = request.Request.PropertyId,
                        BookingId = booking.Id,
                        Date = date
                    });
                }
                await _repositoryManager.SaveChangesAsync();
                await _repositoryManager.CommitTransactionAsync(transaction);
            }
            catch
            {
                await _repositoryManager.RollbackTransactionAsync(transaction);
                throw;
            }
            //The link below to be replaced by the landlordDashboardLink
            var link = "https://INIHomes.com";
            _notificationService.EnqueueBookingRequestedLandlordEmail(new BookingRequestedLandlordPayload(
                 tenant.AppUser.Email,
                landlord.AppUser.FirstName,
                tenant.AppUser.FirstName,
                 property.Title,
                 booking.CheckIn,
                booking.CheckOut,
                link));

            _notificationService.EnqueueBookingRequestedTenantEmail(new BookingRequestedTenantPayload(
                tenant.AppUser.Email,
                tenant.AppUser.FirstName,
                  property.Title,
                   booking.CheckIn,
                booking.CheckOut,
                booking.TotalAmountKobo));
            return ApiResult<string>.Success(booking.Id.ToString());
        }
        
    }
}
