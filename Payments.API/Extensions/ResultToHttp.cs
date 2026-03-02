using Payments.Domain.Common;

namespace Payments.API.Extensions
{
    public static class ResultToHttp
    {
        public static IResult ToHttp(this Result result)
        {
            if (result.IsSuccess)
                return Results.NoContent();

            var e = result.Error;

            if (e.Code.StartsWith("validation.", StringComparison.OrdinalIgnoreCase))
                return Results.BadRequest(new { error = e });

            if (e.Code.StartsWith("auth.", StringComparison.OrdinalIgnoreCase) ||
                e.Code.StartsWith("unauthorized.", StringComparison.OrdinalIgnoreCase))
                return Results.Unauthorized();

            if (e.Code.StartsWith("forbidden.", StringComparison.OrdinalIgnoreCase))
                return Results.Forbid();

            if (e.Code.StartsWith("notfound.", StringComparison.OrdinalIgnoreCase))
                return Results.NotFound(new { error = e });

            if (e.Code.StartsWith("concurrency.", StringComparison.OrdinalIgnoreCase) ||
                e.Code.StartsWith("conflict.", StringComparison.OrdinalIgnoreCase))
                return Results.Conflict(new { error = e });

            // Ожидаемая бизнес-ошибка по умолчанию
            return Results.BadRequest(new { error = e });
        }

        public static IResult ToHttp<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return Results.Ok(result.Value);

            return ((Result)result).ToHttp();
        }
    }
}
