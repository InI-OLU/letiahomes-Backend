using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.RequestFeatures;
using letiahomes.Domain.Entities;
using MediatR;


namespace letiahomes.Application.Features.Properties.Query.GetAllProperty
{
    public class GetAllPropertiesHandler(IRepositoryManager repositoryManager) : IRequestHandler<GetAllPropertiesRequest, ApiResult<PagedList<PropertyResponse>>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public async Task<ApiResult<PagedList<PropertyResponse>>> Handle(GetAllPropertiesRequest request, CancellationToken cancellationToken)
        {
            var response = await _repositoryManager.Properties.GetAllProperties(request.Parameters);
            if (!response.Any())
                return  ApiResult<PagedList<PropertyResponse>>.Failure(new CustomError("404", "Properties not found"));


           var PropertyResult =  response.Select(property =>new PropertyResponse
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
                    IsAvailable = property.IsAvailable,
                    UnavailableDates = property.UnavailableDates,
                    Images = property.Images
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
