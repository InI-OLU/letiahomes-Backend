using letiahomes.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, ApiResult<string>>
    {
        public Task<ApiResult<string>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
