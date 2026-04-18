using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.UploadPropertyPicture
{
    public sealed class UploadPropertyPictureCommandHandler:IRequestHandler<UploadPropertyPictureCommand, ApiResult<string>>
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepositoryManager _repositoryManager;

        public UploadPropertyPictureCommandHandler(ICloudinaryService cloudinaryService,
                                                   IRepositoryManager repositoryManager)
        {
            _cloudinaryService = cloudinaryService;
            _repositoryManager = repositoryManager;
        }
        public async Task<ApiResult<string>> Handle(UploadPropertyPictureCommand request, CancellationToken cancellationToken)
        {

            var property = await _repositoryManager.Properties
                          .FindAll(x => x.Id == request.request.PropertyId, false)
                          .FirstOrDefaultAsync();
            if (property == null)
            {
                return ApiResult<string>.Failure(
                    new CustomError("404", "Property not found"));
            }
            var uploadedImages = new List<string>();
            foreach (var file in request.request.PictureFiles)
            {
                var result = await _cloudinaryService.UploadPhoto(file);

                if (result.IsFailure)
                {
                    foreach (var PublicId in uploadedImages)
                    {
                        await _cloudinaryService.DeleteFile(PublicId);
                    }

                    return ApiResult<string>.Failure(result.Error!);
                }
                uploadedImages.Add(result.Data.PublicId);
                var PropertyImage = new PropertyImage
                {
                    ImageUrl = result.Value.Url,
                    PropertyId = request.request.PropertyId
                };

                await _repositoryManager.PropertyImage.AddAsync(PropertyImage);
                await _repositoryManager.SaveChangesAsync();
            }
            return ApiResult<string>.Success("Image upload successful");

        }
    }
}