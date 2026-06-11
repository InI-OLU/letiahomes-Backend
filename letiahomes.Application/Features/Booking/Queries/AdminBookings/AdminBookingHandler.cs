using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.RequestFeatures;
using letiahomes.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace letiahomes.Application.Features.Booking.Queries.AdminBookings
{
    public class AdminBookingHandler(IRepositoryManager repositoryManager)
        : IRequestHandler<AdminBookingQuery, ApiResult<PagedList<AdminBookingResponse>>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public async Task<ApiResult<PagedList<AdminBookingResponse>>> Handle(
            AdminBookingQuery request,
            CancellationToken cancellationToken)
        {
            var query = _repositoryManager.BookingRepository
                .Get(x => true, false);
            if (request.Filter.BookingStatus.HasValue)
                query = query.Where(x => x.Status == request.Filter.BookingStatus.Value);

            if (request.Filter.PropertyId.HasValue)
                query = query.Where(x => x.PropertyId == request.Filter.PropertyId.Value);

            if (!string.IsNullOrWhiteSpace(request.Filter.TenantEmail))
                query = query.Where(x =>
                    x.Tenant.AppUser.Email.ToLower()
                     .Contains(request.Filter.TenantEmail.ToLower()));

            if (!string.IsNullOrWhiteSpace(request.Filter.LandlordEmail))
                query = query.Where(x =>
                    x.Property.Landlord.AppUser.Email.ToLower()
                     .Contains(request.Filter.LandlordEmail.ToLower()));

            if (request.Filter.StartDate.HasValue)
                query = query.Where(x => x.CheckIn >= request.Filter.StartDate.Value);

            if (request.Filter.EndDate.HasValue)
                query = query.Where(x => x.CheckOut <= request.Filter.EndDate.Value);
            var projected = query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AdminBookingResponse
                {
                    Id = x.Id,
                    PropertyTitle = x.Property.Title,

                    TenantFullName = x.Tenant.AppUser.FirstName
                                   + " " + x.Tenant.AppUser.LastName,
                    TenantEmail = x.Tenant.AppUser.Email,

                    LandlordFullName = x.Property.Landlord.AppUser.FirstName
                                     + " " + x.Property.Landlord.AppUser.LastName,
                    LandlordEmail = x.Property.Landlord.AppUser.Email,

                    CheckIn = x.CheckIn,
                    CheckOut = x.CheckOut,
                    NightsCount = x.NightsCount,
                    SubtotalKobo = x.SubtotalKobo,
                    PlatformFeeKobo = x.PlatformFeeKobo,
                    TotalAmountKobo = x.TotalAmountKobo,
                    Status = x.Status.ToString(),
                    CancellationReason = x.CancellationReason,
                    RejectionReason = x.RejectionReason,
                    CancelledAt = x.CancelledAt,
                    CreatedAt = x.CreatedAt,
                    ExpiresAt = x.ExpiresAt
                });
            var pagedResult = await PagedList<AdminBookingResponse>
                .ToPagedList(projected, request.Filter.pageNumber, request.Filter.pageSize);

            return ApiResult<PagedList<AdminBookingResponse>>.Success(pagedResult);
        }
    }
}