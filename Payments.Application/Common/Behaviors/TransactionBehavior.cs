using Payments.Application.Common.Interfaces;
using MediatR;
using Payments.Application.Common.Abstractions;

namespace Payments.Application.Common.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork uow) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IUnitOfWork _uow = uow;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not ICommand<TResponse>)
                return await next(cancellationToken);

            return await _uow.InTransactionAsync(async ct =>
            {
                return await next(ct);
            }, cancellationToken);
        }
    }
}
