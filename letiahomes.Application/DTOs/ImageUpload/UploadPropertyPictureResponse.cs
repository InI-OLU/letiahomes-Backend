using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.ImageUpload
{
    public sealed class UploadPropertyPictureResponse
    {
        public required List<UploadedImageDto> SuccessfulImages { get; init; }
        public required List<FailedImageDto> FailedImages { get; init; }

        public int SuccessCount => SuccessfulImages.Count;
        public int FailureCount => FailedImages.Count;
    }
}
