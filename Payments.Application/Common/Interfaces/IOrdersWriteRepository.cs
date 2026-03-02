using Payments.Domain.Entities;

namespace Payments.Application.Common.Interfaces
{
    public interface IOrdersWriteRepository : IWriteRepository<Order>
    {
        Task<Order?> GetForUpdateAsync(Guid orderId, CancellationToken ct);
    }
}
