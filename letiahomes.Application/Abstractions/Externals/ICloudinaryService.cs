using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.Externals
{
    public interface ICloudinaryService
    {
        Task<bool> DeleteFile(string id);
        Task<ApiResult<PropertyUploadDto>> UploadPhoto(IFormFile photo);
    }
}
