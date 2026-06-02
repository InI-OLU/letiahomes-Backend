using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using letiahomes.Domain.Enums;
using MediatR;

namespace letiahomes.Application.Features.Booking.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager  _repositoryManager;

        public CreateBookingCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<ApiResult<string>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
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
            var pendingCount = tenant.Bookings.Count(b => b.Status == BookingStatus.Pending);
            if (pendingCount >= 3)
                return ApiResult<string>.Failure(new CustomError("400", "You cannot have more than 3 pending bookings at a time"));


            // ─── 3. VALIDATE DATES ────────────────────────────────────────────

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


            var isDateAvailable = await _repositoryManager.UnavailableDateRepository.IsDateAvailableAsync(request.Request.PropertyId, request.Request.CheckIn, request.Request.CheckOut);
                if (!isDateAvailable)
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

                await _repositoryManager.CommitTransactionAsync(transaction);
            }
            catch
            {
                await _repositoryManager.RollbackTransactionAsync(transaction);
                throw;
            }

            // ─── 11. FIRE NOTIFICATIONS ───────────────────────────────────────
            // These go AFTER the commit — don't send emails for data that hasn't been saved yet
            // You'll need a MediatR INotification + handler for email
            // await _mediator.Publish(new BookingRequestedNotification(booking.Id), cancellationToken);

            return ApiResult<string>.Success(booking.Id.ToString());
        }
    }
}
