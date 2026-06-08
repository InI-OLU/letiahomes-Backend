using letiahomes.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Commands.CancelBooking
{
        public record TenantCancelBookingCommand(Guid BookingId, string UserId, string? Reason) :IRequest<ApiResult<string>>;
}
