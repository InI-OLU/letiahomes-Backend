using letiahomes.Application.Common;
using MediatR;

namespace letiahomes.Application.Features.Booking.Commands.ConfirmBooking
{
    public record ConfirmBookingCommand(Guid BookingId, string UserId) :IRequest<ApiResult<string>>;
}
