using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.CreatePropertyAmenity
{
    public class CreatePropertyAmenityCommandHandler : IRequestHandler<CreatePropertyAmenityCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<CreatePropertyAmenityCommandHandler> _logger;

        public CreatePropertyAmenityCommandHandler(IRepositoryManager repositoryManager,
                                                    ILogger<CreatePropertyAmenityCommandHandler> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }
        public async Task<ApiResult<string>> Handle(CreatePropertyAmenityCommand request, CancellationToken cancellationToken)
        {

            var landlord = await _repositoryManager.Landlords.FindAll(x => x.AppUserId == request.userId,false)
                                                             .FirstOrDefaultAsync(cancellationToken);
            var Property = await _repositoryManager.Properties.FindAll(x => x.Id == request.request.PropertyId,false)
                                                             .FirstOrDefaultAsync(cancellationToken);
            if (Property == null)
            {
                return ApiResult<string>.Failure(new CustomError("404", "Property not found"));
            }
            if ( string.IsNullOrEmpty(request.request.Name))
            {
                return ApiResult<string>.Failure(new CustomError("400", "Property Amenity cannot be empty ."));
            }
            if (landlord == null || Property.LandlordProfileId == landlord.Id )
            {
                return ApiResult<string>.Failure(new CustomError("403", "You are not authorized to perform this action"));
            }

            var exists = await _repositoryManager.PropertyAmenity.FindAll(x => x.PropertyId == request.request.PropertyId &&
                                                                           x.Name.ToLower() == request.request.Name.ToLower(), false)
                                                                           .AnyAsync(cancellationToken);
            if (exists)
                return ApiResult<string>.Failure(new CustomError("404", $"{request.request.Name} has been added to this property"));
            var PropertyAmenity = new PropertyAmenity
            {
                PropertyId = landlord.Id,
                Name = request.request.Name
            };
            await _repositoryManager.PropertyAmenity.AddAsync(PropertyAmenity);
            await _repositoryManager.SaveChangesAsync();
            return ApiResult<string>.Success("Property Amenity has been successfully added");

            
        }
    }
}
