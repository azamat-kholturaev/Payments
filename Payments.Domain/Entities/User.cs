using Payments.Domain.ValueObjects;
using Payments.Domain.Common;

namespace Payments.Domain.Entities
{
    public sealed class User : Entity
    {
        public Email Email { get; private set; } = default!;
        public PasswordHash PasswordHash { get; private set; } = default!;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        private User() { }

        private User(Email email, PasswordHash passwordHash)
        {
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Result<User> Register(string email, string passwordHash)
        {
            var e = Email.Create(email);
            if (!e.IsSuccess)
                return Result<User>.Fail(e.Error);

            var p = PasswordHash.Create(passwordHash);
            if (!p.IsSuccess) 
                return Result<User>.Fail(p.Error);

            return Result<User>.Ok(new User(e.Value!, p.Value!));
        }

        public Result ChangePasswordHash(string newHash)
        {
            var p = PasswordHash.Create(newHash);

            if (!p.IsSuccess)
                return Result.Fail(p.Error);

            PasswordHash = p.Value!;
            return Result.Ok();
        }
    }
}
