using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Auth.Commands.RegisterTenant
{
    public class RegisterTenantCommandValidator:AbstractValidator<RegisterTenantCommand>
    {
        public RegisterTenantCommandValidator()
        {
            RuleFor(v => v.Request.FirstName)
               .NotEmpty().WithMessage("First Name is required.")
               .MaximumLength(100).WithMessage("First Name can not exceed 100 characters");
            RuleFor(v => v.Request.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(100).WithMessage("Last Name can not exceed 100 characters");
            RuleFor(v => v.Request.Email)
                .NotEmpty().WithMessage("Email Address is required.")
                .EmailAddress().WithMessage("Please enter a valid email address");
            RuleFor(v => v.Request.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
            RuleFor(v => v)
               .Must(args => IsAnAllowedEmail(args.Request.Email))
               .WithMessage("Email address not allowed");
        }

        private static bool IsAnAllowedEmail(string email)
        {
            var forbiddenDomains = new[] { "yahoo.com", "yahoomail.com" };
            var domain = email.Split('@').LastOrDefault();

            return !string.IsNullOrWhiteSpace(domain) ? !forbiddenDomains.Contains(domain.ToLower()) : true;
        }

        private static bool PasswordMatches(string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword)) return false;

            return password.Equals(confirmPassword);
        }
    }
}
