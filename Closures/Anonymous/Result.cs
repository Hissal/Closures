// ReSharper disable ConvertToPrimaryConstructor

namespace Closures;

public readonly record struct Result {
    public readonly bool IsSuccess;
    public readonly Exception? Exception;
    
    public static implicit operator bool(Result result) => result.IsSuccess;
    
    Result(bool isSuccess, Exception? ex = null) {
        IsSuccess = isSuccess;
        Exception = ex;
    }

    public static Result Success() => new(true);
    public static Result Failure(Exception ex) => new(false, ex);
}

public readonly record struct Result<T> {
    public readonly bool IsSuccess;
    public readonly T? Value;
    public readonly Exception? Exception;
    
    public static implicit operator bool(Result<T> result) => result.IsSuccess;
    
    Result(bool isSuccess, T? value = default, Exception? ex = null) {
        IsSuccess = isSuccess;
        Value = value;
        Exception = ex;
    }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(Exception ex) => new(false, default, ex);
}