using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Common.Exceptions;
using Payments.Application.Orders.Commands;
using Payments.Application.Orders.Queries;
using Payments.Application.Payments.Commands;
using Payments.Application.Payments.Queries;
using Payments.Contracts;
using Payments.Domain.Common;

namespace Payments.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public sealed class OrdersController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new CreateOrderCommand(request.Amount, request.Currency), ct);

            if (!result.IsSuccess)
                throw new AppException(result.Error.Code, result.Error.Message, 400);

            var id = result.Value;
            return CreatedAtAction(nameof(GetById), new { orderId = id }, new CreateOrderResponse(id));
        }

        [HttpGet("{orderId:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(Guid orderId, CancellationToken ct)
            => Ok(await mediator.Send(new GetOrderQuery(orderId), ct));

        [HttpPost("{orderId:guid}/payments")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentDto>> CreatePayment(Guid orderId, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new AppException("idempotency.required", "Idempotency-Key header is required", 400);

            var payment = await mediator.Send(new CreatePaymentCommand(orderId, idempotencyKey), ct);
            return StatusCode(201, payment);
        }

        [HttpGet("{orderId:guid}/payments")]
        [ProducesResponseType(typeof(IReadOnlyList<PaymentListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PaymentListItemDto>>> ListPayments(Guid orderId, CancellationToken ct)
            => Ok(await mediator.Send(new ListPaymentsByOrderQuery(orderId), ct));
    }


}
