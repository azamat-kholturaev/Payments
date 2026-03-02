namespace Payments.Contracts
{
    public sealed record CreateOrderRequest(decimal Amount, string Currency);
}
