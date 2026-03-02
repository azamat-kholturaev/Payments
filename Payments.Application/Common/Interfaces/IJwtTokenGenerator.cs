using Payments.Domain.Entities;

namespace Payments.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string Generate(User user);
    }
}
