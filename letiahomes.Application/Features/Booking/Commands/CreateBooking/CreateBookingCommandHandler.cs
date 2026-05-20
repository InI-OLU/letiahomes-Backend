using letiahomes.Application.Common;
using MediatR;

namespace letiahomes.Application.Features.Booking.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResult<string>>
    {
        public Task<ApiResult<string>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
