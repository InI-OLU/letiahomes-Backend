using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Infrastructure.ExternalServices
{
    public class CloudinaryService:ICloudinaryService
    {
        private readonly Cloudinary _cloud;

        public CloudinaryService(IOptions<CloudinarySettings> settings)
        {
            Account cloudinaryAccount = new Account(
                settings.Value.CloudName,
                settings.Value.ApiKey,
                settings.Value.ApiSecret
            );
            _cloud = new Cloudinary(cloudinaryAccount);
        }
        public async Task<bool> DeleteFile(string id)
        {
            var deletionParams = new DeletionParams(id);
            deletionParams.ResourceType = ResourceType.Image;
            var delRes = await _cloud.DestroyAsync(deletionParams);
            if (delRes.StatusCode == System.Net.HttpStatusCode.OK && delRes.Result.ToLower() == "ok")
                return true;
            return false;
        }
        public async Task<ApiResult<PropertyUploadDto>> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return ApiResult<PropertyUploadDto>.Failure(
                    new CustomError("400", "Invalid file"));
            }

            var imageUploadParams = new ImageUploadParams
            {
                File = new FileDescription(photo.FileName, photo.OpenReadStream()),
            };

            var res = await _cloud.UploadAsync(imageUploadParams);

            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return ApiResult<PropertyUploadDto>.Failure(
                    new CustomError("500", res.Error?.Message ?? "Upload failed"));
            }

            return ApiResult<PropertyUploadDto>.Success(
                new PropertyUploadDto(res.PublicId, res.Url.ToString()));
        }
    }
}
