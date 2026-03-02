using MediatR;

namespace Payments.Application.Authentication.Queries
{
    public record LoginQuery(string Email, string Password) : IRequest<string>;
}
