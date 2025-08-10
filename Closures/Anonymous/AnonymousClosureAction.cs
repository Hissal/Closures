namespace Closures;

/// <summary>
/// Represents an anonymous closure that encapsulates a delegate with no arguments and no return value,
/// along with an <see cref="AnonymousValue"/> context and <see cref="MutatingBehaviour"/>.
/// </summary>
public record struct AnonymousClosureAction : IClosureAction<AnonymousValue, Delegate>, IAnonymousClosure {
    public Delegate Delegate { get; init; }
    public AnonymousValue Context {
        get => anonymousContext; 
        init => anonymousContext = value;
    }
    AnonymousValue anonymousContext;
    
    public MutatingBehaviour MutatingBehaviour { get; init; }
    
    Delegate? cachedInvoker;
    
    public bool Is<TClosureType>() where TClosureType : IClosure => 
        AnonymousHelper.CanConvert<AnonymousClosureAction, TClosureType>(this);
    
    /// <summary>
    /// Invokes the encapsulated delegate with the current context and mutating behaviour.
    /// </summary>
    public void Invoke() {
        if (cachedInvoker is not AnonymousInvokers.ActionInvoker invoker) {
            invoker = AnonymousInvokers.GetActionInvoker(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }

    /// <summary>
    /// Attempts to invoke the delegate, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    /// <remarks>Exceptions thrown within the delegate invocation should be handled during invocation, the exception handling here is for incorrect delegate types</remarks>
    public Result TryInvoke(ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            Invoke();
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }

            return Result.Failure(e);
        }
    }
    
    /// <inheritdoc/>
    public bool Equals(AnonymousClosureAction other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) && MutatingBehaviour == other.MutatingBehaviour;
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}

/// <summary>
/// Represents an anonymous closure that encapsulates a delegate with a single argument and no return value,
/// along with an <see cref="AnonymousValue"/> context and <see cref="MutatingBehaviour"/>.
/// </summary>
/// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
public record struct AnonymousClosureAction<TArg> : IClosureRefAction<AnonymousValue, TArg, Delegate>, IAnonymousClosure {
    public Delegate Delegate { get; init; }
    public AnonymousValue Context {
        get => anonymousContext;
        init => anonymousContext = value;
    }

    AnonymousValue anonymousContext;
    
    public MutatingBehaviour MutatingBehaviour { get; init; }

    Delegate? cachedInvoker;
    
    public bool Is<TClosureType>() where TClosureType : IClosure =>
        AnonymousHelper.CanConvert<AnonymousClosureAction<TArg>, TClosureType>(this);

    /// <summary>
    /// Invokes the encapsulated delegate with the current context, argument, and mutating behaviour.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate.</param>
    public void Invoke(TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.ActionInvoker<TArg> invoker) {
            invoker = AnonymousInvokers.GetActionInvoker<TArg>(Delegate);
            cachedInvoker = invoker;
        }

        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }

    /// <summary>
    /// Invokes the encapsulated delegate with the current context, argument (by ref), and mutating behaviour.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    public void Invoke(ref TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.ActionInvoker<TArg> invoker) {
            invoker = AnonymousInvokers.GetActionInvoker<TArg>(Delegate);
            cachedInvoker = invoker;
        }

        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }

    /// <summary>
    /// Attempts to invoke the delegate with the specified argument, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    /// <remarks>Exceptions thrown within the delegate invocation should be handled during invocation, the exception handling here is for incorrect delegate types</remarks>
    public Result TryInvoke(TArg arg, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            Invoke(arg);
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }

            return Result.Failure(e);
        }
    }

    /// <summary>
    /// Attempts to invoke the delegate with the specified argument (by ref), handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    /// <remarks>Exceptions thrown within the delegate invocation should be handled during invocation, the exception handling here is for incorrect delegate types</remarks>

    public Result TryInvoke(ref TArg arg,
        ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            Invoke(ref arg);
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy)) {
                throw;
            }

            return Result.Failure(e);
        }
    }

    /// <inheritdoc/>
    public bool Equals(AnonymousClosureAction<TArg> other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) &&
               MutatingBehaviour == other.MutatingBehaviour;
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}