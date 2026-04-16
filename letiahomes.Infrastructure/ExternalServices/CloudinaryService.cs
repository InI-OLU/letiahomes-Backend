using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using letiahomes.Application.Abstractions.Externals;
using letiahomes.Application.DTOs.Property;
using letiahomes.Application.Settings;
using Microsoft.AspNetCore.Http;
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

        public CloudinaryService(CloudinarySettings settings)
        {
            Account cloudinaryAccount = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
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
        public async Task<PropertyUploadDto> UploadPhoto(IFormFile photo)
        {
            var imageUploadParams = new ImageUploadParams
            {
                File = new FileDescription(photo.FileName, photo.OpenReadStream()),
                Transformation = new Transformation().Width(300).Height(300).Gravity("faces").Crop("fill")
            };
            var res = await _cloud.UploadAsync(imageUploadParams);
            if (!(res.StatusCode == System.Net.HttpStatusCode.OK))
                return null;
                
            return new PropertyUploadDto(res.PublicId, res.Url.ToString());

        }
    }
}
