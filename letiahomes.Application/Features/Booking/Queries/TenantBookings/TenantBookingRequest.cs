using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.RequestFeatures;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Queries.TenantBookings
{
  public record TenantBookingRequest(TenantBookingFilter Filter,string userId,Guid bookingId): IRequest<ApiResult<PagedList<BookingResponse>>>;

}
