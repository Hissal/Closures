// ReSharper disable ConvertToPrimaryConstructor

namespace Closures;

/// <summary>
/// Represents the result of an operation, indicating success or failure and an optional exception.
/// </summary>
public readonly record struct Result {
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public readonly bool IsSuccess;
    /// <summary>
    /// The exception associated with a failed operation, or <c>null</c> if successful.
    /// </summary>
    public readonly Exception? Exception;
    
    public static implicit operator bool(Result result) => result.IsSuccess;
    
    Result(bool isSuccess, Exception? ex = null) {
        IsSuccess = isSuccess;
        Exception = ex;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a failed result with the specified exception.
    /// </summary>
    /// <param name="ex">The exception representing the failure.</param>
    public static Result Failure(Exception ex) => new(false, ex);
}

/// <summary>
/// Represents the result of an operation that returns a value, indicating success or failure, the value, and an optional exception.
/// </summary>
/// <typeparam name="T">The type of the value returned by the operation.</typeparam>
public readonly record struct Result<T> {
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public readonly bool IsSuccess;
    /// <summary>
    /// The value returned by the operation if successful, or <c>default</c> if failed.
    /// </summary>
    public readonly T? Value;
    /// <summary>
    /// The exception associated with a failed operation, or <c>null</c> if successful.
    /// </summary>
    public readonly Exception? Exception;
    
    public static implicit operator bool(Result<T> result) => result.IsSuccess;
    
    Result(bool isSuccess, T? value = default, Exception? ex = null) {
        IsSuccess = isSuccess;
        Value = value;
        Exception = ex;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    public static Result<T> Success(T value) => new(true, value);

    /// <summary>
    /// Creates a failed result with the specified exception.
    /// </summary>
    /// <param name="ex">The exception representing the failure.</param>
    public static Result<T> Failure(Exception ex) => new(false, default, ex);
}