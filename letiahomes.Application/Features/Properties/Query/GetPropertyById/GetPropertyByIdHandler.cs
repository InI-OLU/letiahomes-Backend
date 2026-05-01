using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using MediatR;


namespace letiahomes.Application.Features.Properties.Query.GetPropertyById
{
    public class GetPropertyByIdHandler(IRepositoryManager repositoryManager) : IRequestHandler<GetPropertyByIdRequest, ApiResult<PropertyResponse>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;

        public async Task<ApiResult<PropertyResponse>> Handle(GetPropertyByIdRequest request, CancellationToken cancellationToken)
        {
            var property = await _repositoryManager.Properties.GetByIdAsync(request.PropertyId);
            if (property is null)
                return ApiResult<PropertyResponse>.Failure(new CustomError("404", "Properties not found"));
            var PropertyResult = new PropertyResponse
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
            };

            return ApiResult<PropertyResponse>.Success(PropertyResult);

        }
    }
}
