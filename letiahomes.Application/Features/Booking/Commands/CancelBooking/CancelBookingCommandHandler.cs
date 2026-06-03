using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Enums;
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
        private readonly IRepositoryManager _repositoryManager;
        public CancelBookingCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<ApiResult<string>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _repositoryManager.BookingRepository.GetBookingByBookingId(request.BookingId);

            if (booking is null)
                return ApiResult<string>.Failure(new CustomError("404", "Booking not found"));
            if (booking.Status != BookingStatus.Pending)
                return ApiResult<string>.Failure(new CustomError("400", $"Booking cannot be confirmed. Current status: {booking.Status}"));
            var checkIn = await _repositoryManager.UnavailableDateRepository.GetCheckInDate(booking.Id);
            if (DateTime.UtcNow > checkIn)
                return ApiResult<string>.Failure(new CustomError("400", "booking cannot be cancelled after the check-in date has passed"));
            
            throw new NotImplementedException();
        }
    }
}
