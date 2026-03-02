namespace Payments.Application.Common.Interfaces
{
    public interface ICurrentUserProvider
    {
        Guid GetCurrentUser();
    }
}
