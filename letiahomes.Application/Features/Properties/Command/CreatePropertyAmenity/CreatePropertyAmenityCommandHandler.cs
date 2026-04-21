using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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


        public CreatePropertyAmenityCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<ApiResult<string>> Handle(CreatePropertyAmenityCommand request, CancellationToken cancellationToken)
        {

            var landlord = await _repositoryManager.Landlords.FindAll(x => x.AppUserId == request.userId,false)
                                                             .FirstOrDefaultAsync(cancellationToken);
            var Property = await _repositoryManager.Landlords.FindAll(x => x.Id == request.request.PropertyId,false)
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
                return ApiResult<string>.Failure(new CustomError("404", "Landlord can't be found"));
            }
            var PropertyAmenity = new PropertyAmenity
            {
                PropertyId = landlord.Id,
                Name = request.request.Name
            };
            return ApiResult<string>.Success("Property Amenity has been successfully added");

            throw new NotImplementedException();
        }
    }
}
