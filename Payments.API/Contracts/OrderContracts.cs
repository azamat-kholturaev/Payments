namespace Payments.API.Contracts
{
    public sealed record CreateOrderRequest(decimal Amount, string Currency);
    public sealed record CreateOrderResponse(Guid OrderId);
}
