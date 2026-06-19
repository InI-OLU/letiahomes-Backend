using letiahomes.Application.DTOs.ImageUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.UploadPropertyPicture
{
    public sealed class UploadResult
    {
        public bool IsSuccess { get; }
        public UploadedImageDto? Success { get; }
        public FailedImageDto? Failure { get; }

        private UploadResult(bool isSuccess, UploadedImageDto? success, FailedImageDto? failure)
        {
            IsSuccess = isSuccess;
            Success = success;
            Failure = failure;
        }

        public static UploadResult Succeeded(UploadedImageDto dto) =>
            new(true, dto, null);

        public static UploadResult Failed(FailedImageDto dto) =>
            new(false, null, dto);
    }
}
