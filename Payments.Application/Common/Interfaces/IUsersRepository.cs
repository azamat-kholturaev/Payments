using Payments.Domain.Entities;

namespace Payments.Application.Common.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> FindByEmailAsync(string email, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
    }
}
