using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.RequestFeatures;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Queries.TenantBookings
{
    public class TenantBookingHandler(ApplicationDbContext applicationDbContext) : IRequestHandler<TenantBookingRequest, ApiResult<PagedList<BookingResponse>>>
    {
        

        public Task<ApiResult<PagedList<BookingResponse>>> Handle(TenantBookingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
