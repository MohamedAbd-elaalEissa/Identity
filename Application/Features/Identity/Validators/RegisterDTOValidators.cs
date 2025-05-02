using Application.Features.Identity.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Validators
{
    public class RegisterDTOValidators : AbstractValidator<RegisterCommand>
    {
        public RegisterDTOValidators()
        {
            //{PropertyName} =>mean getthe name of the property u focus on 
            RuleFor(n => n.Register.UserName)
                .NotEmpty()
                .MinimumLength(3).WithMessage("For {PropertyName} At Least 3 Char")
                .MaximumLength(20).WithMessage("For {PropertyName} Max Char 20");

            RuleFor(n => n.Register.Email)
               .NotEmpty().WithMessage("{PropertyName} is required")
               .EmailAddress().WithMessage("{PropertyName} must be a valid email address")
               .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters");

            RuleFor(n => n.Register.Password)
              .NotEmpty().WithMessage("Password is required.")
              .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
              .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
              .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
              .Matches("[0-9]").WithMessage("Password must contain at least one number.")
              .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

            RuleFor(n => n.Register.PhoneNumber)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .Matches(@"^01[0-2,5]{1}[0-9]{8}$").WithMessage("{PropertyName} must be a valid phone number (e.g., +1234567890)")
            .Length(7, 15).WithMessage("{PropertyName} must be between 7 and 15 characters");
        }
    }
}
