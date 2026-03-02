using System.Text.RegularExpressions;
using FluentValidation;

namespace Payments.Application.Authentication.Commands
{
    public partial class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        private static readonly Regex PasswordRegex = StrongPasswordRegex();

        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .Must(PasswordRegex.IsMatch)
                .WithMessage("Password must contain upper, lower, digit, special char and be at least 8 chars.");
        }

        [GeneratedRegex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", RegexOptions.Compiled)]
        private static partial Regex StrongPasswordRegex();
    }
}
