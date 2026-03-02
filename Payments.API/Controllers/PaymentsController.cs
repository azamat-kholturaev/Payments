using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Payments.Application.Common.Exceptions;
using Payments.Application.Payments.Commands;

namespace Payments.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public sealed class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("{paymentId:guid}/confirm")]
    [EnableRateLimiting("payments-strict")]
    [ProducesResponseType(typeof(ConfirmPaymentResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConfirmPaymentResultDto>> Confirm(Guid paymentId, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new AppException("idempotency.required", "Idempotency-Key header is required", 400);

        return Ok(await mediator.Send(new ConfirmPaymentCommand(paymentId, idempotencyKey), ct));
    }
}
