using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.Externals
{
    public interface IEmailService
    {
        Task SendAsync(string recipient, string message, string subject);
        Task SendAccountVerifiedAsync(string recipient, string firstName, string loginLink);
        Task SendPasswordResetAsync(string recipient, string firstName, string resetLink);
    }
}
