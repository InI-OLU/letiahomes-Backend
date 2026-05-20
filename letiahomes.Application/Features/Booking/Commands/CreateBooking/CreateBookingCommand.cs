using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Booking;
using MediatR;

namespace letiahomes.Application.Features.Booking.Commands.CreateBooking
{
    public record CreateBookingCommand(CreateBookingRequest Request, string UserId):IRequest<ApiResult<string>>;
}
