using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.ImageUpload
{
    public sealed class UploadedImageDto
    {
        public required string ImageUrl { get; init; }
        public required string PublicId { get; init; }
        public bool IsCoverImage { get; init; }
    }
}
