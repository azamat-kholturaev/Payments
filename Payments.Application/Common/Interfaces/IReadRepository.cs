using Payments.Domain.Common;

namespace Payments.Application.Common.Interfaces
{
    public interface IReadRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
        IQueryable<TEntity> Query();
    }
}
