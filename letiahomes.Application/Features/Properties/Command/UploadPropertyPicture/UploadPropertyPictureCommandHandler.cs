using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.ImageUpload;
using letiahomes.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace letiahomes.Application.Features.Properties.Command.UploadPropertyPicture
{
    public sealed class UploadPropertyPictureCommandHandler
        : IRequestHandler<UploadPropertyPictureCommand, ApiResult<UploadPropertyPictureResponse>>
    {
        private const int MaxImagesPerProperty = 15;
        private const int MaxConcurrentUploads = 5;
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/jpg" };

        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepositoryManager _repositoryManager;

        public UploadPropertyPictureCommandHandler(
            ICloudinaryService cloudinaryService,
            IRepositoryManager repositoryManager)
        {
            _cloudinaryService = cloudinaryService;
            _repositoryManager = repositoryManager;
        }

        public async Task<ApiResult<UploadPropertyPictureResponse>> Handle(
            UploadPropertyPictureCommand request, CancellationToken cancellationToken)
        {
         
            var property = await _repositoryManager.Properties
                .Get(x => x.Id == request.PropertyId, false)
                .FirstOrDefaultAsync(cancellationToken);

            if (property is null)
                return ApiResult<UploadPropertyPictureResponse>.Failure(
                    new CustomError("404", "Property not found."));

            var existingCount = await _repositoryManager.PropertyImage
                .Get(x => x.PropertyId == request.PropertyId, false)
                .CountAsync(cancellationToken);

            if (existingCount + request.request.PictureFiles.Count > MaxImagesPerProperty)
                return ApiResult<UploadPropertyPictureResponse>.Failure(
                    new CustomError("400",
                        $"Maximum {MaxImagesPerProperty} images allowed. This property already has {existingCount} image(s)."));

            // 3. Validate files — collect failures, keep valid ones for upload
            var validFiles = new List<IFormFile>();
            var validationFailures = new List<FailedImageDto>();

            foreach (var file in request.request.PictureFiles)
            {
                var validationError = ValidateFile(file);
                if (validationError is not null)
                {
                    validationFailures.Add(new FailedImageDto
                    {
                        FileName = file?.FileName ?? "unknown",
                        Reason = validationError
                    });
                    continue;
                }

                validFiles.Add(file!);
            }

            // 4. Upload valid files with bounded concurrency
            var hasCoverImage = await _repositoryManager.PropertyImage
                .Get(x => x.PropertyId == request.PropertyId && x.IsCoverImage, false)
                .AnyAsync(cancellationToken);

            using var semaphore = new SemaphoreSlim(MaxConcurrentUploads);

            var uploadTasks = validFiles.Select(file =>
                UploadSingleFileAsync(file, semaphore));

            var uploadResults = await Task.WhenAll(uploadTasks);

            // 5. Split results
            var successfulUploads = uploadResults.Where(r => r.IsSuccess)
                .Select(r => r.Success!)
                .ToList();

            var uploadFailures = uploadResults.Where(r => !r.IsSuccess)
                .Select(r => r.Failure!)
                .ToList();

            // 6. Assign cover image to first success if property has none
            if (!hasCoverImage && successfulUploads.Count > 0)
            {
                var first = successfulUploads[0];
                successfulUploads[0] = new UploadedImageDto
                {
                    ImageUrl = first.ImageUrl,
                    PublicId = first.PublicId,
                    IsCoverImage = true
                };
            }

            // 7. Persist successful images
            if (successfulUploads.Count > 0)
            {
                foreach (var image in successfulUploads)
                {
                    await _repositoryManager.PropertyImage.AddAsync(new PropertyImage
                    {
                        ImageUrl = image.ImageUrl,
                        PropertyId = request.PropertyId,
                        PublicId = image.PublicId,
                        IsCoverImage = image.IsCoverImage
                    });
                }

                await _repositoryManager.SaveChangesAsync(cancellationToken);
            }

            var response = new UploadPropertyPictureResponse
            {
                SuccessfulImages = successfulUploads,
                FailedImages = validationFailures.Concat(uploadFailures).ToList()
            };

            return ApiResult<UploadPropertyPictureResponse>.Success(response);
        }

        private static string? ValidateFile(IFormFile? file)
        {
            if (file is null || file.Length == 0)
                return "File is empty or invalid.";

            if (file.Length > MaxFileSizeBytes)
                return "Exceeds the 5MB limit.";

            if (!AllowedContentTypes.Contains(file.ContentType))
                return "Unsupported format. Only JPEG and PNG are allowed.";

            return null;
        }

        private async Task<UploadResult> UploadSingleFileAsync(IFormFile file, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();

            try
            {
                var uploadResult = await _cloudinaryService.UploadPhoto(file);

                if (uploadResult.IsFailure)
                {
                    return UploadResult.Failed(new FailedImageDto
                    {
                        FileName = file.FileName,
                        Reason = "Upload to Cloudinary failed."
                    });
                }

                return UploadResult.Succeeded(new UploadedImageDto
                {
                    ImageUrl = uploadResult.Value.Url,
                    PublicId = uploadResult.Value.PublicId,
                    IsCoverImage = false
                });
            }
            catch (Exception)
            {
                return UploadResult.Failed(new FailedImageDto
                {
                    FileName = file.FileName,
                    Reason = "Unexpected error during upload."
                });
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}