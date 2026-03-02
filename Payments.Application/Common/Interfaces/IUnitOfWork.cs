namespace Payments.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitChangesAsync(CancellationToken cancellationToken = default);
        Task<T> InTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct);
    }
}
