using letiahomes.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Commands.ConfirmBooking
{
    public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, ApiResult<string>>
    {
        public Task<ApiResult<string>> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
