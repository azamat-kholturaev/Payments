using Payments.Application.Common.Interfaces;
using Payments.Domain.ValueObjects;
using Payments.Domain.Entities;
using MediatR;

namespace Payments.Application.Orders.Commands
{
    public class CreateOrderCommandHandler(IOrdersWriteRepository repo,
                                           ICurrencyCatalog currencyCatalog, 
                                           ICurrentUserProvider currentUser) : IRequestHandler<CreateOrderCommand, Guid>
    {
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (!await currencyCatalog.IsSupportedAsync(request.Currency, cancellationToken))
                throw new ApplicationException("Unsupported currency");

            var userId = currentUser.GetCurrentUser();

            var moneyResult = Money.Create(request.Amount, request.Currency);
            if (!moneyResult.IsSuccess)
                throw new ApplicationException(moneyResult.Error.Message);

            var orderResult = Order.Create(userId, moneyResult.Value!);
            if (!orderResult.IsSuccess)
                throw new ApplicationException(orderResult.Error.Message);

            var order = orderResult.Value!;

            await repo.AddAsync(order, cancellationToken);

            return order.Id;
        }
    }
}
