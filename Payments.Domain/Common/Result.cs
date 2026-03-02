namespace Payments.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error Error { get; }

        protected Result(bool success, Error error)
        {
            IsSuccess = success;
            Error = error;
        }

        public static Result Ok() => new(true, Error.None);
        public static Result Fail(Error error) => new(false, error);
    }

    public sealed class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool success, Error error, T? value) : base(success, error)
            => Value = value;

        public static Result<T> Ok(T value) => new(true, Error.None, value);
        public static new Result<T> Fail(Error error) => new(false, error, default);
    }
}
