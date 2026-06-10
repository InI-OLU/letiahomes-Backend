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
            var tenant = await _repositoryManager.Tenants.Get(t => t.AppUser.IsActive == true && t.AppUser.IsVerified== true && t.AppUser.Id == request.userId, false)
                                                         .FirstOrDefaultAsync();
  
            var bookings = await _repositoryManager.BookingRepository.Get(x => x.Id == request.bookingId, false)
                                                                     .Select(x => new BookingResponse
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
                                                                     })
                                                                     .ToListAsync();
            

            throw new NotImplementedException();
        }
    }
}
