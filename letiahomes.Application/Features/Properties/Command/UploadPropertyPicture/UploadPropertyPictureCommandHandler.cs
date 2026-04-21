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
        public async Task<ApiResult<string>> Handle(
     UploadPropertyPictureCommand request, CancellationToken cancellationToken)
        {
            // 1. Check property exists
            var property = await _repositoryManager.Properties
                .FindAll(x => x.Id == request.request.PropertyId, false)
                .FirstOrDefaultAsync(cancellationToken);

            if (property is null)
                return ApiResult<string>.Failure(new CustomError("404", "Property not found."));

            // 2. Check max image limit
            var existingCount = await _repositoryManager.PropertyImage
                .FindAll(x => x.PropertyId == request.request.PropertyId, false)
                .CountAsync(cancellationToken);

            if (existingCount + request.request.PictureFiles.Count > 5)
                return ApiResult<string>.Failure(new CustomError("400",
                    $"Maximum 5 images allowed. This property already has {existingCount} image(s)."));

            // 3. Validate all files before uploading anything
            foreach (var file in request.request.PictureFiles)
            {
                if (file is null || file.Length == 0)
                    return ApiResult<string>.Failure(new CustomError("400", "One or more files are invalid."));

                if (file.Length > 5 * 1024 * 1024)
                    return ApiResult<string>.Failure(new CustomError("400",
                        $"{file.FileName} exceeds the 5MB limit."));

                string[] allowed = { "image/jpeg", "image/png", "image/jpg" };
                if (!allowed.Contains(file.ContentType))
                    return ApiResult<string>.Failure(new CustomError("400",
                        $"{file.FileName} is not a supported format. Only JPEG and PNG are allowed."));
            }

            // 4. Upload all concurrently
            var uploadTasks = request.request.PictureFiles
                .Select(file => _cloudinaryService.UploadPhoto(file))
                .ToList();

            var uploadResults = await Task.WhenAll(uploadTasks);

            // 5. Check for upload failures — rollback all if any failed
            var failedUploads = uploadResults.Where(r => r.IsFailure).ToList();
            if (failedUploads.Any())
            {
                var cleanupTasks = uploadResults
                    .Where(r => r.IsSuccess)
                    .Select(r => _cloudinaryService.DeleteFile(r.Value.PublicId));
                await Task.WhenAll(cleanupTasks);

                return ApiResult<string>.Failure(
                    new CustomError("500", "One or more uploads failed. No images were saved."));
            }

            // 6. Check image quality
            var lowQuality = uploadResults
                .FirstOrDefault(r => r.Value.QualityAnalysis <= 0.8);

            if (lowQuality is not null)
            {
                var cleanupTasks = uploadResults
                    .Select(r => _cloudinaryService.DeleteFile(r.Value.PublicId));
                await Task.WhenAll(cleanupTasks);

                return ApiResult<string>.Failure(
                    new CustomError("400", "One or more images have low quality. Please upload higher quality images."));
            }

            // 7. Check if property already has a cover image
            var hasCoverImage = await _repositoryManager.PropertyImage
                .FindAll(x => x.PropertyId == request.request.PropertyId
                           && x.IsCoverImage, false)
                .AnyAsync(cancellationToken);

            // 8. Build image entities
            var propertyImages = uploadResults
                .Select((result, index) => new PropertyImage
                {
                    ImageUrl = result.Value.Url,
                    PropertyId = request.request.PropertyId,
                    IsCoverImage = !hasCoverImage && index == 0
                }).ToList();

            // 9. Save all in one DB call
            foreach (var image in propertyImages)
                await _repositoryManager.PropertyImage.AddAsync(image);

            await _repositoryManager.SaveChangesAsync(cancellationToken);

            var publicIds = string.Join(", ", uploadResults.Select(r => r.Value.PublicId));
            return ApiResult<string>.Success(
                $"Successfully uploaded {uploadResults.Length} image(s).");
        }
    }
}