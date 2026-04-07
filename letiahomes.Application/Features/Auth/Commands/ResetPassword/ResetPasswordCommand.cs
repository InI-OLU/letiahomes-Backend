using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using MediatR;

namespace letiahomes.Application.Features.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<ApiResult<string>>;
}