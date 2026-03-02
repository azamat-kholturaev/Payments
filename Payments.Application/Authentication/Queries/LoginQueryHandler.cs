using Payments.Application.Common.Interfaces;
using MediatR;

namespace Payments.Application.Authentication.Queries
{

    public sealed class LoginQueryHandler(IUsersRepository users,
                                          IPasswordHasher hasher,
                                          IJwtTokenGenerator jwt) : IRequestHandler<LoginQuery, string>
    {
        public async Task<string> Handle(LoginQuery request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await users.FindByEmailAsync(email, ct);
            if (user is null)
                throw new ApplicationException("auth.invalid_credentials");

            if (!hasher.Verify(request.Password, user.PasswordHash.Value))
                throw new ApplicationException("auth.invalid_credentials");

            return jwt.Generate(user);
        }
    }
}
