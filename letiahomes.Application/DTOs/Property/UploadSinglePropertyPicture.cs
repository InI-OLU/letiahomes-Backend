using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.DTOs.Property
{
    public class UploadSinglePropertyPicture
    {
        public required Guid PropertyId { get; set; }
        public required IFormFile PictureFiles { get; set; }
    }
}
