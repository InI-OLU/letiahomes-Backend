using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.RequestFeatures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Queries.TenantBookings
{
    public class TenantBookingHandler(IRepositoryManager repositoryManager) : IRequestHandler<TenantBookingRequest, ApiResult<PagedList<BookingResponse>>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public async  Task<ApiResult<PagedList<BookingResponse>>> Handle(TenantBookingRequest request, CancellationToken cancellationToken)
        {
            var tenantExists = await _repositoryManager.Tenants
                                    .Get(t => t.AppUser.Id == request.userId
                                      && t.AppUser.IsActive
                                     && t.AppUser.IsVerified, false)
                                    .AnyAsync();
            if (!tenantExists)
                return ApiResult<PagedList<BookingResponse>>.Failure(new CustomError("404" ,"Tenant not found or inactive"));
            var query = _repositoryManager.BookingRepository
                 .Get(x => x.Tenant.AppUser.Id == request.userId, false);

            if (request.Filter.BookingStatus.HasValue)
                query = query.Where(x => x.Status == request.Filter.BookingStatus.Value);

            if (request.Filter.StartDate != default)
                query = query.Where(x => x.CheckIn >= request.Filter.StartDate);

            if (request.Filter.EndDate != default)
                query = query.Where(x => x.CheckOut <= request.Filter.EndDate);

            var projected = query.Select(x => new BookingResponse
            {
                Id = x.Id,
                PropertyTitle = x.Property.Title,
                CoverImageUrl = x.Property.Images
                    .Where(img => img.IsCoverImage)
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault(),
                CheckIn = x.CheckIn,
                CheckOut = x.CheckOut,
                NightsCount = x.NightsCount,
                NumberOfGuests = x.NumberOfGuests,
                SubtotalKobo = x.SubtotalKobo,
                TotalAmountKobo = x.TotalAmountKobo,
                CreatedAt = x.CreatedAt,
                ExpiresAt = x.ExpiresAt
            });

            var pagedResult = await PagedList<BookingResponse>
                .ToPagedList(projected, request.Filter.pageNumber, request.Filter.pageSize);

            return ApiResult<PagedList<BookingResponse>>.Success(pagedResult);
        }
    }
}
