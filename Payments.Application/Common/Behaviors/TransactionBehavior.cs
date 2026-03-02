using MediatR;
using Payments.Application.Common.Abstractions;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Common;

namespace Payments.Application.Common.Behaviors
{
    public sealed class TransactionBehavior<TRequest, TResponse>(IUnitOfWork uow)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IUnitOfWork _uow = uow;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
                                            CancellationToken ct)
        {
            if (request is not ICommand<TResponse>)
                return await next(ct);

            return await _uow.InTransactionAsync(async txCt =>
            {
                var response = await next(txCt);

                if (response is Result r && !r.IsSuccess)
                    return response;

                return response;
            }, ct);
        }
    }
}
