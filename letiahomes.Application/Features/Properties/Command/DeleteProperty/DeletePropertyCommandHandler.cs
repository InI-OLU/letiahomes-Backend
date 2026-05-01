using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.DeleteProperty
{
    public class DeletePropertyCommandHandler(IRepositoryManager repositoryManager ,ILogger<DeletePropertyCommandHandler> logger ) : IRequestHandler<DeletePropertyCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;
        private readonly ILogger<DeletePropertyCommandHandler> _logger = logger;

        public async Task<ApiResult<string>> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _repositoryManager.Properties
            .FindAll(x => x.Id == request.PropertyId
                       && x.Landlord.AppUserId == request.userId,
                     true)
            .FirstOrDefaultAsync(cancellationToken);
            if (property is null)
            {
                return ApiResult<string>.Failure(
                    new CustomError("404", "Property not found or access denied"));
            }

            _repositoryManager.Properties.Delete(property);
              await _repositoryManager.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Property with Id: {PropertyId} deleted by User: {UserId}",
                                     request.PropertyId,
                                     request.userId
                                 );
            //There should be a deleted count for admin to be able to track how many users pulled their properties off the platform 

            return ApiResult<string>.Success("Property successfully deleted");
        }
    }
}
