using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.ImageUpload
{
    public sealed class FailedImageDto
    {
        public required string FileName { get; init; }
        public required string Reason { get; init; }
    }
}
