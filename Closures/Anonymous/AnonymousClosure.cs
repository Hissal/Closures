namespace Closures;

/// <summary>
/// Whether the closure should mutate the context or reset it after invocation.
/// </summary>
public enum MutatingBehaviour : byte {
    Mutate,
    Reset
}

/// <summary>
/// Policy for handling exceptions during delegate invocation.
/// </summary>
public enum ExceptionHandlingPolicy : byte {
    HandleExpected,
    HandleAll,
    HandleNone
}

/// <summary>
/// Represents an Anonymous Closure that encapsulates a delegate and a context.
/// </summary>
public interface IAnonymousClosure : IClosure<AnonymousValue, Delegate> {
    MutatingBehaviour MutatingBehaviour { get; init; }
    /// <summary>
    /// Determines if this closure can be converted to the specified closure type.
    /// </summary>
    bool Is<TClosureType>() where TClosureType : IClosure;
}

// TODO: Use source generators to generate all used delegate types and their invokers

/// <summary>
/// Represents an anonymous closure that encapsulates a delegate and a context,
/// along with a <see cref="MutatingBehaviour"/> indicating whether the context should be mutated or reset after invocation.
/// <br></br>
/// <para>
/// This struct provides a unified, type-erased way to store and invoke closures with arbitrary context and delegate types,
/// supporting both action and function delegates, with or without arguments, and handling mutating or resetting context behaviour.
/// </para>
/// <remarks>
/// Use with caution, as it is a type-erased structure that does not enforce type safety at compile time.
/// </remarks>
/// </summary>
public partial record struct AnonymousClosure : IAnonymousClosure {
    public Delegate Delegate { get; init; }
    public AnonymousValue Context {
        get => anonymousContext; 
        init => anonymousContext = value;
    }
    AnonymousValue anonymousContext;

    public MutatingBehaviour MutatingBehaviour { get; init; }
    
    Delegate? cachedInvoker;

    public bool Is<TClosureType>() where TClosureType : IClosure => 
        AnonymousHelper.CanConvert<AnonymousClosure, TClosureType>(this);

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
    /// Invokes the encapsulated delegate with the current context, argument, and mutating behaviour.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
    /// <param name="arg">The argument to pass to the delegate.</param>
    public void Invoke<TArg>(TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.ActionInvoker<TArg> invoker) {
            invoker = AnonymousInvokers.GetActionInvoker<TArg>(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    /// <summary>
    /// Invokes the encapsulated delegate with the current context, argument (by ref), and mutating behaviour.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    public void Invoke<TArg>(ref TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.ActionInvoker<TArg> invoker) {
            invoker = AnonymousInvokers.GetActionInvoker<TArg>(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    /// <summary>
    /// Invokes the encapsulated delegate with the current context and mutating behaviour, returning a value.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the delegate.</typeparam>
    /// <returns>The return value from the delegate.</returns>
    public TReturn Invoke<TReturn>() {
        if (cachedInvoker is not AnonymousInvokers.FuncInvoker<TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }
    /// <summary>
    /// Invokes the encapsulated delegate with the current context, argument, and mutating behaviour, returning a value.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
    /// <typeparam name="TReturn">The return type of the delegate.</typeparam>
    /// <param name="arg">The argument to pass to the delegate.</param>
    /// <returns>The return value from the delegate.</returns>
    public TReturn Invoke<TArg, TReturn>(TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.FuncInvoker<TArg, TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    /// <summary>
    /// Invokes the encapsulated delegate with the current context, argument (by ref), and mutating behaviour, returning a value.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
    /// <typeparam name="TReturn">The return type of the delegate.</typeparam>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    /// <returns>The return value from the delegate.</returns>
    public TReturn Invoke<TArg, TReturn>(ref TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.FuncInvoker<TArg, TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }

    /// <summary>
    /// Attempts to invoke the delegate, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result TryInvoke(ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            Invoke();
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy))
                throw;

            return Result.Failure(e);
        }
    }
    /// <summary>
    /// Attempts to invoke the delegate with the specified argument, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
    /// <param name="arg">The argument to pass to the delegate.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result TryInvoke<TArg>(TArg arg, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            Invoke(arg);
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy))
                throw;

            return Result.Failure(e);
        }
    }
    /// <summary>
    /// Attempts to invoke the delegate with the specified argument (by ref), handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result TryInvoke<TArg>(ref TArg arg, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            Invoke(ref arg);
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy))
                throw;

            return Result.Failure(e);
        }
    }
    /// <summary>
    /// Attempts to invoke the delegate, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>, returning a value.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the delegate.</typeparam>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result{TReturn}"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result<TReturn> TryInvoke<TReturn>(ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            var result = Invoke<TReturn>();
            return Result<TReturn>.Success(result!);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy))
                throw;

            return Result<TReturn>.Failure(e);
        }
    }
    /// <summary>
    /// Attempts to invoke the delegate with the specified argument, handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>, returning a value.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
    /// <typeparam name="TReturn">The return type of the delegate.</typeparam>
    /// <param name="arg">The argument to pass to the delegate.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result{TReturn}"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result<TReturn> TryInvoke<TArg, TReturn>(TArg arg, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            var result = Invoke<TArg, TReturn>(arg);
            return Result<TReturn>.Success(result!);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy))
                throw;

            return Result<TReturn>.Failure(e);
        }
    }
    /// <summary>
    /// Attempts to invoke the delegate with the specified argument (by ref), handling exceptions according to the specified <see cref="ExceptionHandlingPolicy"/>, returning a value.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the delegate.</typeparam>
    /// <typeparam name="TReturn">The return type of the delegate.</typeparam>
    /// <param name="arg">The argument to pass to the delegate by reference.</param>
    /// <param name="exceptionHandlingPolicy">The exception handling policy.</param>
    /// <returns>A <see cref="Result{TReturn}"/> indicating success or failure.</returns>
    /// <remarks>
    /// Exceptions thrown within the delegate invocation should be handled during invocation,
    /// the exception handling here is for incorrect delegate types.
    /// </remarks>
    public Result<TReturn> TryInvoke<TArg, TReturn>(ref TArg arg, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
        try {
            var result = Invoke<TArg, TReturn>(ref arg);
            return Result<TReturn>.Success(result!);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e, exceptionHandlingPolicy))
                throw;

            return Result<TReturn>.Failure(e);
        }
    }

    /// <inheritdoc/>
    public bool Equals(AnonymousClosure other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) && MutatingBehaviour == other.MutatingBehaviour;
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}