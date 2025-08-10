namespace Closures.Anonymous;

/// <summary>
/// Represents an anonymous closure that encapsulates a delegate with no arguments and a return value,
/// along with an <see cref="AnonymousValue"/> context and <see cref="MutatingBehaviour"/>.
/// </summary>
/// <typeparam name="TReturn">The return type of the delegate.</typeparam>
public record struct AnonymousClosureFunc<TReturn> : IClosureFunc<AnonymousValue, TReturn, Delegate>, IAnonymousClosure {
    public Delegate Delegate { get; init; }
    public AnonymousValue Context {
        get => anonymousContext; 
        init => anonymousContext = value;
    }
    AnonymousValue anonymousContext;
    
    public MutatingBehaviour MutatingBehaviour { get; init; }
    
    Delegate? cachedInvoker;
    
    public bool Is<TClosureType>() where TClosureType : IClosure => 
        AnonymousHelper.CanConvert<AnonymousClosureFunc<TReturn>, TClosureType>(this);
    
    /// <summary>
    /// Invokes the encapsulated delegate with the current context and mutating behaviour.
    /// </summary>
    /// <returns>The return value from the delegate.</returns>
    public TReturn Invoke() {
        if (cachedInvoker is not AnonymousInvokers.FuncInvoker<TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }
    
    /// <summary>
    /// Attempts to invoke the delegate, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="returnValue">The return value from the delegate if successful; otherwise, default.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>True if invocation succeeded; otherwise, false.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public bool TryInvoke(out TReturn returnValue, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            returnValue = Invoke();
            return true;
        }
        catch (Exception e) {
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }
            
            returnValue = default!;
            return false;
        }
    }

    /// <summary>
    /// Attempts to invoke the delegate, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result{TReturn}"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result<TReturn> TryInvoke(ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            var result = Invoke();
            return Result<TReturn>.Success(result);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }

            return Result<TReturn>.Failure(e);
        }
    }
    
    /// <inheritdoc/>
    public bool Equals(AnonymousClosureFunc<TReturn> other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) && MutatingBehaviour == other.MutatingBehaviour;
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}

/// <summary>
/// Represents an anonymous closure that encapsulates a delegate with a single argument and a return value,
/// along with an <see cref="AnonymousValue"/> context and <see cref="MutatingBehaviour"/>.
/// </summary>
/// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
/// <typeparam name="TReturn">The return type of the delegate.</typeparam>
public record struct AnonymousClosureFunc<TArg, TReturn> : IClosureRefFunc<AnonymousValue, TArg, TReturn, Delegate>, IAnonymousClosure {
    public Delegate Delegate { get; init; }
    public AnonymousValue Context {
        get => anonymousContext;
        init => anonymousContext = value;
    }

    AnonymousValue anonymousContext;
    
    public MutatingBehaviour MutatingBehaviour { get; init; }

    Delegate? cachedInvoker;
    
    public bool Is<TClosureType>() where TClosureType : IClosure =>
        AnonymousHelper.CanConvert<AnonymousClosureFunc<TArg, TReturn>, TClosureType>(this);

    /// <summary>
    /// Invokes the encapsulated delegate with the current context, argument, and mutating behaviour.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate.</param>
    /// <returns>The return value from the delegate.</returns>
    public TReturn Invoke(TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.FuncInvoker<TArg, TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate);
            cachedInvoker = invoker;
        }

        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }

    /// <summary>
    /// Invokes the encapsulated delegate with the current context, argument (by ref), and mutating behaviour.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    /// <returns>The return value from the delegate.</returns>
    public TReturn Invoke(ref TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.FuncInvoker<TArg, TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate);
            cachedInvoker = invoker;
        }

        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }

    /// <summary>
    /// Attempts to invoke the delegate with the specified argument, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate.</param>
    /// <param name="returnValue">The return value from the delegate if successful; otherwise, default.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>True if invocation succeeded; otherwise, false.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public bool TryInvoke(TArg arg, out TReturn returnValue,
        ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            returnValue = Invoke(arg);
            return true;
        }
        catch (Exception e) {
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }

            returnValue = default!;
            return false;
        }
    }

    /// <summary>
    /// Attempts to invoke the delegate with the specified argument, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result{TReturn}"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result<TReturn> TryInvoke(TArg arg,
        ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            var result = Invoke(ref arg);
            return Result<TReturn>.Success(result);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }

            return Result<TReturn>.Failure(e);
        }
    }

    /// <summary>
    /// Attempts to invoke the delegate with the specified argument (by ref), handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    /// <param name="returnValue">The return value from the delegate if successful; otherwise, default.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>True if invocation succeeded; otherwise, false.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public bool TryInvoke(ref TArg arg, out TReturn returnValue,
        ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            returnValue = Invoke(arg);
            return true;
        }
        catch (Exception e) {
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }

            returnValue = default!;
            return false;
        }
    }

    /// <summary>
    /// Attempts to invoke the delegate with the specified argument (by ref), handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result{TReturn}"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result<TReturn> TryInvoke(ref TArg arg,
        ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            var result = Invoke(ref arg);
            return Result<TReturn>.Success(result);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }

            return Result<TReturn>.Failure(e);
        }
    }

    /// <inheritdoc/>
    public bool Equals(AnonymousClosureFunc<TArg, TReturn> other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) &&
               MutatingBehaviour == other.MutatingBehaviour;
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}