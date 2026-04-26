using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.RequestFeatures;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Query.FilterProperty
{
    public class FilterPropertiesHandler(IRepositoryManager repositoryManager) : IRequestHandler<FilterPropertiesRequest, ApiResult<PagedList<PropertyResponse>>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public async Task<ApiResult<PagedList<PropertyResponse>>> Handle(FilterPropertiesRequest request, CancellationToken cancellationToken)
        {
            var response = await _repositoryManager.Properties.FilterBy(request.request);
            if (!response.Any())
                return ApiResult<PagedList<PropertyResponse>>.Failure(new CustomError("404", "Properties not found"));
            var PropertyResult = response.Select(property => new PropertyResponse
            {
                Id = property.Id,
                Title = property.Title,
                State = property.State,
                City = property.City,
                Description = property.Description,
                Address = property.Address,
                PricePerNightKobo = property.PricePerNightKobo,
                MaxGuests = property.MaxGuests,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                PropertyType = property.PropertyType,
                ListingType = property.ListingType,
                IsAvailable = property.IsAvailable
            }).ToList();

            var PagedResult = new PagedList<PropertyResponse>(
                 PropertyResult,
                 response.MetaData.TotalCount,
                 response.MetaData.CurrentPage,
                 response.MetaData.PageSize
             );

            return ApiResult<PagedList<PropertyResponse>>.Success(PagedResult);
      
        }
    }
}
