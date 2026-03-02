using MediatR;
using Payments.Application.Common.Exceptions;
using Payments.Application.Common.Interfaces;
using Payments.Application.Orders.Queries;
using Payments.Domain.Entities;

namespace Payments.Application.Payments.Queries
{
    public sealed class ListPaymentsByOrderQueryHandler(IReadRepository<Order> orders,
                                                        IPaymentsWriteRepository payments,
                                                        ICurrentUserProvider currentUser)
                                                        : IRequestHandler<ListPaymentsByOrderQuery, OrderWithPaymentsDto>
    {
        public async Task<OrderWithPaymentsDto> Handle(ListPaymentsByOrderQuery request, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(request.OrderId, ct)
                ?? throw new AppException("order.not_found", "Order not found", 404);

            var userId = currentUser.GetCurrentUser();
            var owner = order.EnsureOwner(userId);
            if (!owner.IsSuccess)
                throw new AppException("order.forbidden", owner.Error.Message, 403);

            var orderDto = new OrderDto(order.Id,
                                        order.Total.Amount,
                                        order.Total.Currency.Code,
                                        order.Status.ToString().ToLowerInvariant());

            var list = await payments.GetByOrderIdAsync(order.Id, ct);

            var paymentDtos = list.Select(p => new PaymentListItemDto(p.Id,
                                                                      p.Status.ToString().ToLowerInvariant(),
                                                                      p.Amount,
                                                                      p.Currency,
                                                                      p.CreatedAt,
                                                                      p.ProviderPaymentId,
                                                                      p.FailureReason)).ToList();

            return new OrderWithPaymentsDto(orderDto, paymentDtos);
        }
    }
}
