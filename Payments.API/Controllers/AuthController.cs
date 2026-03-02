using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Contracts;
using Payments.Application.Authentication.Commands;
using Payments.Application.Authentication.Queries;

namespace Payments.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController(ISender sender) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var token = await sender.Send(new RegisterCommand(request.Email, request.Password), ct);
            return Ok(new AuthResponse(token));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var token = await sender.Send(new LoginQuery(request.Email, request.Password), ct);
            return Ok(new AuthResponse(token));
        }
    }
}
