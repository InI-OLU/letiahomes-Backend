using FluentValidation;
using letiahomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Booking.Commands.CreateBooking
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User identity is required");

            RuleFor(x => x.Request.PropertyId)
                .NotEmpty()
                .WithMessage("PropertyId cannot be empty");

            RuleFor(x => x.Request.NumberOfGuests)
                .GreaterThan(0)
                .WithMessage("Number of guests must be greater than zero");

            RuleFor(x => x.Request.CheckIn)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Check-in date is required")
                .GreaterThan(_ => DateTime.UtcNow.Date)  
                .WithMessage("Check-in date must be at least 1 day in the future");

            RuleFor(x => x.Request.CheckOut)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Check-out date is required")
                .GreaterThan(x => x.Request.CheckIn)
                .WithMessage("Check-out date must be after check-in date")
                .Must((command, checkOut) =>
                    (checkOut - command.Request.CheckIn).TotalDays <= 90)
                .WithMessage("Booking duration cannot exceed 90 nights");
        }
    }
}
