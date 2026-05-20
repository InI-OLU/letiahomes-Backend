using letiahomes.Application.Common;
using MediatR;

namespace letiahomes.Application.Features.Booking.Commands.RejectBooking
{
    public record RejectBookingCommand(Guid BookingId, string UserId, string? Reason) :IRequest<ApiResult<string>>;
}
