using letiahomes.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Commands.RejectBooking
{
    public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, ApiResult<string>>
    {
        public Task<ApiResult<string>> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
