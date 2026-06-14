using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.Features.Booking.Queries.LanlordBookings;
using letiahomes.Application.RequestFeatures;
using letiahomes.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace letiahomes.Application.Features.Booking.Queries.LandlordBookings
{
    public class LandlordBookingHandler(IRepositoryManager repositoryManager)
        : IRequestHandler<LandlordBookingRequest, ApiResult<PagedList<LandlordBookingResponse>>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public async Task<ApiResult<PagedList<LandlordBookingResponse>>> Handle(
            LandlordBookingRequest request,
            CancellationToken cancellationToken)
        {
            var landlordExists = await _repositoryManager.Landlords
                .Get(l => l.AppUser.Id == request.UserId
                       && l.AppUser.IsActive
                       && l.AppUser.IsVerified, false)
                .AnyAsync(cancellationToken);

            if (!landlordExists)
                return ApiResult<PagedList<LandlordBookingResponse>>
                    .Failure(new CustomError("404" ,"Landlord not found or account is inactive."));
            var query = _repositoryManager.BookingRepository
                .Get(x => x.Property.Landlord.AppUser.Id == request.UserId, false);

            if (request.Filter.BookingStatus.HasValue)
                query = query.Where(x => x.Status == request.Filter.BookingStatus.Value);

            if (request.Filter.StartDate.HasValue)
                query = query.Where(x => x.CheckIn >= request.Filter.StartDate.Value);

            if (request.Filter.EndDate.HasValue)
                query = query.Where(x => x.CheckOut <= request.Filter.EndDate.Value);
            var projected = query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new LandlordBookingResponse
                {
                    Id = x.Id,
                    PropertyTitle = x.Property.Title,
                    TenantFirstName = x.Tenant.AppUser.FirstName,
                    CheckIn = x.CheckIn,
                    CheckOut = x.CheckOut,
                    NightsCount = x.NightsCount,
                    TotalAmountKobo = x.TotalAmountKobo,
                    Status = x.Status.ToString(),
                    CreatedAt = x.CreatedAt
                });
            var pagedResult = await PagedList<LandlordBookingResponse>
                .ToPagedList(projected, request.Filter.pageNumber, request.Filter.pageSize);

            return ApiResult<PagedList<LandlordBookingResponse>>.Success(pagedResult);
        }
    }
}