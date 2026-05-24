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
                    //other business rules validation are done .....
          /*I begin a transaction here and fetch The unavailable dates and do a check
           * I then implement Optimistic Locking implementing Read Committed isolation since that is what ef core offers and Pessimistic 
           * locking should be reserved for when it is absolutely needed in this case it isn't becase i am not expecting thousands of concurrent users
           * at once yet so the load doesn't justify explicit locking and risking Deadlocks and more code to solve that edgecase.
           * I Implement a Row version in Postgres (cant verify if postgress has it though) in my Unavailable date table 
           * So in the event that the row has been  by another transaction before the transaction has finished i then i catch the DB efcore exception 
           * and instead of retrying it i send a userFriendly error message to the user that the Property isn't available To let in that date range .
           * Then as a final line of Defense i use a multiple column UNIQUE CONSTRAINTS on the UnavailableData table , on Date and propertyId to be specific 
           * this ensures that two rows on these tables can't be the same . This protects against the odds that the the user bypasses this locks and they both
           * read the unavailable date at exactlu the same millisecond i.e Non-Repeatable reads.so i catch the Constraint Exception thrown by the Db and send a
           * message to the user that the property isnt available 
           * 
           * this is my plan 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           * 
           */
            throw new NotImplementedException();
        }
    }
}
