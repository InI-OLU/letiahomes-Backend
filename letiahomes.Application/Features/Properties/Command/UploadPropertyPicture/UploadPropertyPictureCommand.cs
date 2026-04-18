using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Property;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.UploadPropertyPicture
{
    public sealed record UploadPropertyPictureCommand(UploadMultiplePropertyPicture request):IRequest<ApiResult<string>>;
    
}
