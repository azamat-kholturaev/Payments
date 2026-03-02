using Microsoft.EntityFrameworkCore;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;
using Payments.Domain.ValueObjects;
using Payments.Infrastructure.Common.Persistence;

namespace Payments.Infrastructure.Repositories
{
    internal sealed class UsersRepository(AppDbContext db) : IUsersRepository
    {
        public Task<User?> FindByEmailAsync(string email, CancellationToken ct)
        {
            var normalized = email.Trim().ToLowerInvariant();
            var emailResult = Email.Create(normalized);

            if (!emailResult.IsSuccess)
                return Task.FromResult<User?>(null);

            return db.Users.FirstOrDefaultAsync(x => x.Email == emailResult.Value!, ct);
        }

        public Task AddAsync(User user, CancellationToken ct)
            => db.Users.AddAsync(user, ct).AsTask();
    }
}
