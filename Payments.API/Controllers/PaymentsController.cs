using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Contracts;
using Payments.Application.Payments.Commands;

namespace Payments.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/payments")]
    public sealed class PaymentsController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<CreatePaymentResponse>> Create([FromBody] CreatePaymentRequest request, CancellationToken ct)
        {
            var paymentId = await sender.Send(new CreatePaymentCommand(request.OrderId, request.IdempotencyKey), ct);
            return Ok(new CreatePaymentResponse(paymentId));
        }

        [HttpPost("{paymentId:guid}/confirm")]
        public async Task<IActionResult> Confirm([FromRoute] Guid paymentId, [FromBody] ConfirmPaymentRequest request, CancellationToken ct)
        {
            await sender.Send(new ConfirmPaymentCommand(paymentId, request.ProviderPaymentId), ct);
            return NoContent();
        }
    }
}
