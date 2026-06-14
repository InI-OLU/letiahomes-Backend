using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.RequestFeatures;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Queries.LanlordBookings
{
    public record LandlordBookingRequest(
      string UserId,
      LandlordBookingFilter Filter) : IRequest<ApiResult<PagedList<LandlordBookingResponse>>>;
}
