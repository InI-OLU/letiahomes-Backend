using letiahomes.Application.Common;
using letiahomes.Application.DTOs.Auth;
using letiahomes.Application.Services.Interface;
using Microsoft.Extensions.Hosting;

namespace letiahomes.Application.Services
{
   public class AccountService
    {
        private readonly IHostEnvironment _host;

        public AccountService(IHostEnvironment host)
        {
            _host = host;
        }

       


        private string GetAccountVerificationMessage(string firstName, string otp)
        {
            var path = Path.Combine(_host.ContentRootPath, "wwwroot", "EmailTemplate", "AccountVerification.html");
            if (File.Exists(path))
            {
                var template = File.ReadAllText(path);
                return template.Replace("{{FirstName}}", firstName)
                    .Replace("{{OTP}}", otp);
            }

            throw new Exception($"Path not found: {path}");
        }
    }
}
