namespace Closures;

public struct Result {
    public bool IsSuccess { get; }
    public Exception? Exception { get; }

    public static implicit operator bool(Result result) => result.IsSuccess;

    public Result(bool isSuccess, Exception? ex = null) {
        IsSuccess = isSuccess;
        Exception = ex;
    }

    public static Result Success() => new(true);
    public static Result Failure(Exception ex) => new(false, ex);
}

public struct Result<T> {
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Exception? Exception { get; }

    public static implicit operator bool(Result<T> result) => result.IsSuccess;

    public Result(bool isSuccess, T? value = default, Exception? ex = null) {
        IsSuccess = isSuccess;
        Value = value;
        Exception = ex;
    }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(Exception ex) => new(false, default, ex);
}