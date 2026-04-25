using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Query.GetAllProperty
{
    public class GetAllPropertiesHandler(IRepositoryManager repositoryManager) : IRequestHandler<GetAllPropertiesRequest, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public async Task<ApiResult<PropertyResponse>> Handle(GetAllPropertiesRequest request, CancellationToken cancellationToken)
        {
            var response = await _repositoryManager.Properties.GetAllProperties(request.Parameters);
            if (response == null)
                return ApiResultIEnumerable<<PropertyResponse>.Failure(new CustomError("404", "Properties not found"));
            foreach(Property property in response)
            {
                var PropertyResult = new PropertyResponse
                {
                    Title = property.Title,
                    State = property.State,
                    City = property.City,
                    Description = property.Description,
                    Address = property.Address,
                    PricePerNightKobo = property.PricePerNightKobo,
                    MaxGuests = property.MaxGuests,
                    Bedrooms = property.Bedrooms,
                    Bathrooms = property.Bathrooms,
                    PropertyType = property.PropertyType
                };
            }
            return ApiResult<PropertyResponse>.Success(PropertyResult);
           
        }
    }
}
