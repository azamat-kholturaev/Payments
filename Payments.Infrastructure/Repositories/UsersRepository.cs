using Payments.Infrastructure.Common.Persistence;
using Payments.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Repositories
{
    internal sealed class UsersRepository(AppDbContext db) : IUsersRepository
    {
        public Task<User?> FindByEmailAsync(string email, CancellationToken ct)
        {
            var normalized = email.Trim().ToLowerInvariant();
            return db.Users.FirstOrDefaultAsync(x => x.Email.Value == normalized, ct);
        }

        public Task AddAsync(User user, CancellationToken ct)
            => db.Users.AddAsync(user, ct).AsTask();
    }
}
