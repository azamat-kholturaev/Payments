using Payments.Application.Common.Interfaces;
using Payments.Application.Common.Exceptions;
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
                throw new AppException("currency.unsupported", "Unsupported currency", 400);

            var userId = currentUser.GetCurrentUser();

            var moneyResult = Money.Create(request.Amount, request.Currency);
            if (!moneyResult.IsSuccess)
                throw new AppException(moneyResult.Error.Code, moneyResult.Error.Message, 400);

            var orderResult = Order.Create(userId, moneyResult.Value!);
            if (!orderResult.IsSuccess)
                throw new AppException(orderResult.Error.Code, orderResult.Error.Message, 400);

            var order = orderResult.Value!;

            await repo.AddAsync(order, cancellationToken);

            return order.Id;
        }
    }
}
