using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Features.Properties.Command.UpdateProperty
{
    public class UpdatePropertyValidator:AbstractValidator<UpdatePropertyCommand>
    {
        public UpdatePropertyValidator()
        {
            RuleFor(x=> x.request.Title)
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters");
            RuleFor(x => x.request.Description)
                 .MaximumLength(500).WithMessage("Description should not exceed 500 characters");
            RuleFor(x => x.request.PricePerNightKobo)
               .GreaterThan(0);
        }
        
    }
}
