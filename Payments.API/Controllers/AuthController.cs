using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Payments.Application.Authentication.Commands;
using Payments.Application.Authentication.Queries;
using Payments.Contracts;

namespace Payments.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    [EnableRateLimiting("payments-strict")]
    public sealed class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TokenResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var token = await mediator.Send(new RegisterCommand(request.Email, request.Password), ct);
            return Ok(new TokenResponse(token));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var token = await mediator.Send(new LoginQuery(request.Email, request.Password), ct);
            return Ok(new TokenResponse(token));
        }
    }
}
