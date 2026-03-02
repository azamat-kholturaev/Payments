using Payments.Application.Common.Interfaces;
using System.Security.Claims;

namespace Payments.API.Services
{
    public sealed class CurrentUserProvider(IHttpContextAccessor _httpContextAccessor) : ICurrentUserProvider
    {
        public Guid GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException();

            var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (raw is null || !Guid.TryParse(raw, out var id))
                throw new UnauthorizedAccessException("Invalid user id claim");

            return id;
        }
    }
}
