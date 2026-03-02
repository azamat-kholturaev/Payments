using FluentValidation;
using MediatR;
using Payments.Domain.Common;

namespace Payments.Application.Common.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct)));

            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count == 0)
                return await next(ct);

            var first = failures[0];

            var error = new Error(
                Code: $"validation.{first.PropertyName}",
                Message: first.ErrorMessage);

            // Поддерживаем только Result / Result<T>
            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Fail(error);

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                // Вызов Result<T>.Fail(Error) через reflection
                var t = typeof(TResponse).GenericTypeArguments[0];
                var fail = typeof(Result<>).MakeGenericType(t)
                    .GetMethod(nameof(Result<>.Fail), [typeof(Error)]);

                return (TResponse)fail!.Invoke(null, [error])!;
            }

            throw new InvalidOperationException(
                $"ValidationBehavior supports only Result or Result<T>. Actual: {typeof(TResponse).Name}");
        }
    }
}
