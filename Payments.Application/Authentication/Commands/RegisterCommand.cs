using Payments.Application.Common.Abstractions;

namespace Payments.Application.Authentication.Commands
{
    public record RegisterCommand(string Email, string Password) : ICommand<string>;
}
