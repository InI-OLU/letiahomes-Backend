using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.SubmitPropertyForReview
{
    public class PropertyReviewCommandHandler(IRepositoryManager repositoryManager,ILogger<PropertyReviewCommandHandler> logger) : IRequestHandler<PropertyReviewCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;
        private readonly ILogger<PropertyReviewCommandHandler> _logger = logger;

        public async Task<ApiResult<string>> Handle(PropertyReviewCommand request, CancellationToken cancellationToken)
        {
            var property = await _repositoryManager.Properties.GetPropertyWithAmenityAndImage(request.Propertyid);
            var landlord = await _repositoryManager.Landlords.GetLandlord(request.UserId);
            if (property is null)
                return ApiResult<string>.Failure(new CustomError("404", "Property not found"));
            if (landlord is null)
                return ApiResult<string>.Failure(new CustomError("404", "Landlord not found"));
            if (!property.Images.Any())
                return ApiResult<string>.Failure(new CustomError("400", "Please upload at least one image before submitting for review."));
            if (property.IsApproved)
                return ApiResult<string>.Failure(
                    new CustomError("400", "This property is already approved."));
            _logger.LogInformation(
            "Property {PropertyId} submitted for review by landlord {UserId}",
            property.Id, request.UserId);
            //An email should be sent here to confirm the creation of the account to the Landlord Email
            //Notify Admin of the new propertyCreated
            return ApiResult<string>.Success(
           "Your property has been submitted for review. We will notify you once it is approved.");
        }
    }
}
