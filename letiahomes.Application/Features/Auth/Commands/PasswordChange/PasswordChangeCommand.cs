using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Auth.Commands.PasswordChange
{
    public record PasswordChangeCommand(string UserId, ChangePasswordRequest Request) : IRequest<ApiResult<string>>;

}
