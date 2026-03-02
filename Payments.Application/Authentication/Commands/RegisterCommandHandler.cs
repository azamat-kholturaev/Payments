using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;
using MediatR;

namespace Payments.Application.Authentication.Commands
{
    public sealed class RegisterCommandHandler(IUsersRepository users,
                                               IPasswordHasher hasher,
                                               IJwtTokenGenerator jwt) : IRequestHandler<RegisterCommand, string>
    {
        public async Task<string> Handle(RegisterCommand request, CancellationToken ct)
        {
            var emailNorm = request.Email.Trim().ToLowerInvariant();

            if (await users.FindByEmailAsync(emailNorm, ct) is not null)
                throw new ApplicationException("user.email_taken");

            var hash = hasher.Hash(request.Password);

            var userRes = User.Register(emailNorm, hash);

            if (!userRes.IsSuccess)
                throw new ApplicationException(userRes.Error.Message);

            var user = userRes.Value!;
            await users.AddAsync(user, ct);

            return jwt.Generate(user);
        }
    }
}
