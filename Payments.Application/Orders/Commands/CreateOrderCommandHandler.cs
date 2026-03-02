using Payments.Application.Common.Interfaces;
using Payments.Domain.ValueObjects;
using Payments.Domain.Entities;
using Payments.Domain.Common;
using MediatR;

namespace Payments.Application.Orders.Commands
{
    public class CreateOrderCommandHandler(IOrdersWriteRepository repo,
                                           ICurrencyCatalog currencyCatalog,
                                           ICurrentUserProvider currentUser) : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (!await currencyCatalog.IsSupportedAsync(request.Currency, cancellationToken))
                return Result<Guid>.Fail(new Error("currency.unsupported", "Unsupported currency"));

            var userId = currentUser.GetCurrentUser();

            var moneyResult = Money.Create(request.Amount, request.Currency);
            if (!moneyResult.IsSuccess)
                return Result<Guid>.Fail(moneyResult.Error);

            var orderResult = Order.Create(userId, moneyResult.Value!);
            if (!orderResult.IsSuccess)
                return Result<Guid>.Fail(orderResult.Error);

            var order = orderResult.Value!;

            await repo.AddAsync(order, cancellationToken);

            return Result<Guid>.Ok(order.Id);
        }
    }
}
