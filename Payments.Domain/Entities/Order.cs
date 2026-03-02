using Payments.Domain.ValueObjects;
using Payments.Domain.Common;

namespace Payments.Domain.Entities
{
    public sealed class Order : Entity
    {
        public Guid UserId { get; private set; }
        public Money Total { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Created;

        private Order() { }

        private Order(Guid userId, Money total)
        {
            UserId = userId;
            Total = total;
            Status = OrderStatus.Created;
        }

        public static Result<Order> Create(Guid userId, Money total)
        {
            if (userId == Guid.Empty)
                return Result<Order>.Fail(new Error("order.user_invalid", "UserId is invalid"));

            return Result<Order>.Ok(new Order(userId, total));
        }

        public Result EnsureOwner(Guid userId)
        {
            if (userId == Guid.Empty)
                return Result.Fail(new Error("auth.user_invalid", "UserId is invalid"));

            if (UserId != userId)
                return Result.Fail(new Error("auth.forbidden", "Not your order"));

            return Result.Ok();
        }

        public Result EnsurePayable()
        {
            if (Status != OrderStatus.Created)
                return Result.Fail(new Error("order.not_payable", "Only created order can be paid"));
            return Result.Ok();
        }

        public Result ApplySuccessfulPayment()
        {
            var payable = EnsurePayable();
            if (!payable.IsSuccess) return payable;

            Status = OrderStatus.Paid;
            return Result.Ok();
        }

        public Result Cancel()
        {
            if (Status == OrderStatus.Paid)
                return Result.Fail(new Error("order.cannot_cancel_paid", "Paid order cannot be cancelled"));

            Status = OrderStatus.Cancelled;
            return Result.Ok();
        }
    }
}
