using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;
using MediatR;
using Payments.Application.Common.Exceptions;

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
            throw new AppException("user.email_taken", "Email already taken", 409);

            var hash = hasher.Hash(request.Password);

            var userRes = User.Register(emailNorm, hash);

            if (!userRes.IsSuccess)
            throw new AppException(userRes.Error.Code, userRes.Error.Message, 400);

            var user = userRes.Value!;
            await users.AddAsync(user, ct);

            return jwt.Generate(user);
        }
    }
}

