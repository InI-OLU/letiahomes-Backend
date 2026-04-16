using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.CreateProperty
{
    public class CreatePropertyCommandValidator:AbstractValidator<CreatePropertyCommand>
    {
        public CreatePropertyCommandValidator()
        {
            RuleFor(x => x.request.Address)
                .NotEmpty().WithMessage("A valid Address is required");
            RuleFor(x => x.request.City)
                .NotEmpty().WithMessage("A valid city is required");
            RuleFor(x => x.request.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(50).WithMessage("Description should not exceed 50 characters");
            RuleFor(x => x.request.Title)
                .NotEmpty().WithMessage("A valid title is required");
            RuleFor(x => x.request.)
        }
    }
}
