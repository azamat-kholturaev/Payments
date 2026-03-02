using Payments.Domain.Entities;

namespace Payments.Application.Common.Interfaces
{
    public interface IPaymentsWriteRepository : IWriteRepository<Payment>
    {
        Task<List<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken ct);
    }
}
