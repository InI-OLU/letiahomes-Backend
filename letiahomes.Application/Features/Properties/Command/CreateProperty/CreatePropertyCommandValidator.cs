using FluentValidation;
using letiahomes.Domain.Enums;

namespace letiahomes.Application.Features.Properties.Command.CreateProperty
{
    public class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
    {
        public CreatePropertyCommandValidator()
        {
            RuleFor(x => x.request.Title)
                .NotEmpty().WithMessage("A valid title is required")
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters");

            RuleFor(x => x.request.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters");

            RuleFor(x => x.request.Address)
                .NotEmpty().WithMessage("A valid address is required")
                .MaximumLength(200).WithMessage("Address should not exceed 200 characters");

            RuleFor(x => x.request.City)
                .NotEmpty().WithMessage("A valid city is required");

            RuleFor(x => x.request.State)
                .NotEmpty().WithMessage("State is required");

            RuleFor(x => x.request.MaxGuests)
                .GreaterThan(0).WithMessage("Max guests must be greater than 0")
                .LessThanOrEqualTo(50).WithMessage("Max guests cannot exceed 50");

            RuleFor(x => x.request.Bedrooms)
                .GreaterThan(0).WithMessage("Bedrooms must be at least 1")
                .LessThanOrEqualTo(20).WithMessage("Bedrooms cannot exceed 20");

            RuleFor(x => x.request.Bathrooms)
                .GreaterThan(0).WithMessage("Bathrooms must be at least 1")
                .LessThanOrEqualTo(20).WithMessage("Bathrooms cannot exceed 20");

            RuleFor(x => x.request.PropertyType)
                .IsInEnum().WithMessage("Invalid property type");
        }
    }
}