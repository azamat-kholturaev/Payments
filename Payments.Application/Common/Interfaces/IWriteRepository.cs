using Payments.Domain.Common;

namespace Payments.Application.Common.Interfaces
{
    public interface IWriteRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity?> GetTrackingByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(TEntity entity, CancellationToken ct);
        void Remove(TEntity entity);
    }
}
