using Payments.Application.Common.Interfaces;
using Payments.Application.Common.Exceptions;
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
                throw new AppException("auth.invalid_credentials", "Invalid credentials", 401);

            if (!hasher.Verify(request.Password, user.PasswordHash.Value))
                throw new AppException("auth.invalid_credentials", "Invalid credentials", 401);

            return jwt.Generate(user);
        }
    }
}
