using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using MediatR;

namespace letiahomes.Application.Features.Booking.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager  _repositoryManager;

        public CreateBookingCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<ApiResult<string>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _repositoryManager.Tenants.GetTenant(request.UserId);
            var property = await _repositoryManager.Properties.GetByIdAsync(request.Request.PropertyId);
            if (tenant == null || tenant.AppUser.IsActive == false || tenant.AppUser.IsVerified == false)
            {
                return ApiResult<string>.Failure(new CustomError("400", "User not Permitted to make booking"));
            }
            if(property == null ||property.IsAvailable == false || property.IsApproved == false )
            {
                return ApiResult<string>.Failure(new CustomError("400", "Property not available for booking"));
            }
            if(tenant.Bookings.Count > 3)
            {
                return ApiResult<string>.Failure(new CustomError("400", "User can only have 3 pending booking at a time"));
            }
           await _repositoryManager.BeginTransactionAsync();
            var unavailableDate = await _repositoryManager.UnavailableDateRepository.


            throw new NotImplementedException();
        }
    }
}
