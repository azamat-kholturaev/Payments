using MediatR;

namespace Payments.Application.Common.Abstractions
{
    public interface ICommand<TResponse> : IRequest<TResponse>;
}
