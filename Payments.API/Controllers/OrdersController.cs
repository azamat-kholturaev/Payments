using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Contracts;
using Payments.Application.Common.Interfaces;
using Payments.Application.Orders.Commands;
using Payments.Application.Payments.Queries;

namespace Payments.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders")]
    public sealed class OrdersController(ISender sender, ICurrentUserProvider currentUser) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
        {
            var userId = currentUser.GetCurrentUser();
            var orderId = await sender.Send(new CreateOrderCommand(userId, request.Amount, request.Currency), ct);
            return Ok(new CreateOrderResponse(orderId));
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<OrderDetailsDto>> GetById([FromRoute] Guid orderId, CancellationToken ct)
        {
            var order = await sender.Send(new GetOrderByIdQuery(orderId), ct);
            return Ok(order);
        }

        [HttpGet("{orderId:guid}/payments")]
        public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetPayments([FromRoute] Guid orderId, CancellationToken ct)
        {
            var payments = await sender.Send(new ListOrderPaymentsQuery(orderId), ct);
            return Ok(payments);
        }
    }
}
