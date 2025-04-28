using Application.Features.Identity.Commands;
using ApplicationContract.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Validators
{
    public class LoginDTOValidators : AbstractValidator<LoginCommand>
    {
        public LoginDTOValidators()
        {
            RuleFor(l => l.Login.Email)
               .NotEmpty().WithMessage("{PropertyName} is required")
               .EmailAddress().WithMessage("{PropertyName} must be a valid email address")
               .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters");

            RuleFor(n => n.Login.Password)
             .NotEmpty().WithMessage("Password is required.")
             .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
             .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
             .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
             .Matches("[0-9]").WithMessage("Password must contain at least one number.")
             .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
        }
    }
}
