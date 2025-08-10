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

public interface IAnonymousClosure : IClosure<AnonymousValue, Delegate> {
    MutatingBehaviour MutatingBehaviour { get; init; }
    bool Is<TClosureType>() where TClosureType : IClosure;
}

public partial record struct AnonymousClosure {
    public static TClosure Create<TClosure>(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset)
        where TClosure : struct, IAnonymousClosure {
        return new TClosure {
            Context = context,
            Delegate = @delegate,
            MutatingBehaviour = mutatingBehaviour,
        };
    }

    public static AnonymousClosure Create(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) => 
        Create<AnonymousClosure>(context, @delegate, mutatingBehaviour);
    
    public static AnonymousClosure Create<TDelegate>(AnonymousValue context, TDelegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset)
        where TDelegate : Delegate => 
        Create<AnonymousClosure>(context, @delegate, mutatingBehaviour);

    public static AnonymousClosureAction Action(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) {
        if (!AnonymousHelper.IsAction(@delegate))
            throw new ArgumentException("Delegate must not have a return value.", nameof(@delegate));
        
        if (AnonymousHelper.HasArg(@delegate))
            throw new ArgumentException("Delegate must not have an argument.", nameof(@delegate));

        return Create<AnonymousClosureAction>(context, @delegate, mutatingBehaviour);
    }
    public static AnonymousClosureAction<TArg> Action<TArg>(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) {
        if (!AnonymousHelper.IsAction(@delegate))
            throw new ArgumentException("Delegate must not have a return value.", nameof(@delegate));
        
        if (!AnonymousHelper.HasArgOfType<TArg>(@delegate))
            throw new ArgumentException($"Delegate must have an argument of type {typeof(TArg).Name}.", nameof(@delegate));
        
        return Create<AnonymousClosureAction<TArg>>(context, @delegate, mutatingBehaviour);
    }
    
    public static AnonymousClosureFunc<TReturn> Func<TReturn>(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) {
        if (!AnonymousHelper.IsFunc(@delegate))
            throw new ArgumentException("Delegate must have a return value.", nameof(@delegate));
        
        if (AnonymousHelper.HasArg(@delegate))
            throw new ArgumentException("Delegate must not have an argument.", nameof(@delegate));
        
        return Create<AnonymousClosureFunc<TReturn>>(context, @delegate, mutatingBehaviour);
    }
    public static AnonymousClosureFunc<TArg, TReturn> Func<TArg, TReturn>(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) {
        if (!AnonymousHelper.IsFunc(@delegate))
            throw new ArgumentException("Delegate must have a return value.", nameof(@delegate));
        
        if (!AnonymousHelper.HasArgOfType<TArg>(@delegate))
            throw new ArgumentException($"Delegate must have an argument of type {typeof(TArg).Name}.", nameof(@delegate));
        
        return Create<AnonymousClosureFunc<TArg, TReturn>>(context, @delegate, mutatingBehaviour);
    }

    // Targeted creation methods for specific delegate types
    public static AnonymousClosureAction Action<TContext>(TContext context, Action<TContext> @delegate) where TContext : notnull =>
        Create<AnonymousClosureAction>(AnonymousValue.From(context), @delegate);
    public static AnonymousClosureAction Action<TContext>(TContext context, RefAction<TContext> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull =>
        Create<AnonymousClosureAction>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    
    public static AnonymousClosureAction<TArg> Action<TContext, TArg>(TContext context, Action<TContext, TArg> @delegate) where TContext : notnull => 
        Create<AnonymousClosureAction<TArg>>(AnonymousValue.From(context), @delegate);
    public static AnonymousClosureAction<TArg> Action<TContext, TArg>(TContext context, RefActionWithNormalContext<TContext, TArg> @delegate) where TContext : notnull => 
        Create<AnonymousClosureAction<TArg>>(AnonymousValue.From(context), @delegate);
    public static AnonymousClosureAction<TArg> Action<TContext, TArg>(TContext context, ActionWithRefContext<TContext, TArg> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureAction<TArg>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    public static AnonymousClosureAction<TArg> Action<TContext, TArg>(TContext context, RefAction<TContext, TArg> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureAction<TArg>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    
    
    public static AnonymousClosureFunc<TReturn> Func<TContext, TReturn>(TContext context, Func<TContext, TReturn> @delegate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TReturn>>(AnonymousValue.From(context), @delegate);
    public static AnonymousClosureFunc<TReturn> Func<TContext, TReturn>(TContext context, RefFunc<TContext, TReturn> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TReturn>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    
    public static AnonymousClosureFunc<TArg, TReturn> Func<TContext, TArg, TReturn>(TContext context, Func<TContext, TArg, TReturn> @delegate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TArg, TReturn>>(AnonymousValue.From(context), @delegate);
    public static AnonymousClosureFunc<TArg, TReturn> Func<TContext, TArg, TReturn>(TContext context, RefFuncWithNormalContext<TContext, TArg, TReturn> @delegate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TArg, TReturn>>(AnonymousValue.From(context), @delegate);
    public static AnonymousClosureFunc<TArg, TReturn> Func<TContext, TArg, TReturn>(TContext context, FuncWithRefContext<TContext, TArg, TReturn> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TArg, TReturn>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    public static AnonymousClosureFunc<TArg, TReturn> Func<TContext, TArg, TReturn>(TContext context, RefFunc<TContext, TArg, TReturn> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TArg, TReturn>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
}

// TODO: Use source generators to generate all used delegate types and their invokers
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
    
    public void Invoke() {
        if (cachedInvoker is not AnonymousInvokers.AnonymousActionInvoker invoker) {
            invoker = AnonymousInvokers.GetActionInvoker(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }
    public void Invoke<TArg>(TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.AnonymousActionInvoker<TArg> invoker) {
            invoker = AnonymousInvokers.GetActionInvoker<TArg>(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    public void Invoke<TArg>(ref TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.AnonymousActionInvoker<TArg> invoker) {
            invoker = AnonymousInvokers.GetActionInvoker<TArg>(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    
    public TReturn Invoke<TReturn>() {
        if (cachedInvoker is not AnonymousInvokers.AnonymousFuncInvoker<TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }
    
    public TReturn Invoke<TArg, TReturn>(TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.AnonymousFuncInvoker<TArg, TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    
    public TReturn Invoke<TArg, TReturn>(ref TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.AnonymousFuncInvoker<TArg, TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }

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

    public bool Equals(AnonymousClosure other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) && MutatingBehaviour == other.MutatingBehaviour;
    }

    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}

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
    
    public void Invoke() {
        if (cachedInvoker is not AnonymousInvokers.AnonymousActionInvoker invoker) {
            invoker = AnonymousInvokers.GetActionInvoker(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }

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
    
    public bool Equals(AnonymousClosureAction other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) && MutatingBehaviour == other.MutatingBehaviour;
    }

    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}

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
    
    public void Invoke(TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.AnonymousActionInvoker<TArg> invoker) {
            invoker = AnonymousInvokers.GetActionInvoker<TArg>(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    public void Invoke(ref TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.AnonymousActionInvoker<TArg> invoker) {
            invoker = AnonymousInvokers.GetActionInvoker<TArg>(Delegate);
            cachedInvoker = invoker;
        }
        invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    
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
    public Result TryInvoke(ref TArg arg, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
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
    
    public bool Equals(AnonymousClosureAction<TArg> other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) && MutatingBehaviour == other.MutatingBehaviour;
    }

    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}

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
    
    public TReturn Invoke() {
        if (cachedInvoker is not AnonymousInvokers.AnonymousFuncInvoker<TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }
    
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
    
    public bool Equals(AnonymousClosureFunc<TReturn> other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) && MutatingBehaviour == other.MutatingBehaviour;
    }

    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}

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
    
    public TReturn Invoke(TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.AnonymousFuncInvoker<TArg, TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    public TReturn Invoke(ref TArg arg) {
        if (cachedInvoker is not AnonymousInvokers.AnonymousFuncInvoker<TArg, TReturn> invoker) {
            invoker = AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate);
            cachedInvoker = invoker;
        }
        return invoker.Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    
    public bool TryInvoke(TArg arg, out TReturn returnValue, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
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
    public Result<TReturn> TryInvoke(TArg arg, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
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
    
    public bool TryInvoke(ref TArg arg, out TReturn returnValue, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
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
    public Result<TReturn> TryInvoke(ref TArg arg, ExceptionHandlingPolicy exceptionHandlingPolicy = ExceptionHandlingPolicy.HandleExpected) {
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
    
    public bool Equals(AnonymousClosureFunc<TArg, TReturn> other) {
        return anonymousContext.Equals(other.anonymousContext) && Delegate.Equals(other.Delegate) && MutatingBehaviour == other.MutatingBehaviour;
    }

    public override int GetHashCode() {
        return HashCode.Combine(anonymousContext, Delegate, (int)MutatingBehaviour);
    }
}

public static class AnonymousClosureConversionExtensions {
    // Conversion to AnonymousClosure
    // Normal Actions
    public static AnonymousClosure AsAnonymous<TContext>(this ClosureAction<TContext> closure) where TContext : notnull => 
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosure AsAnonymous<TContext, TArg>(this ClosureAction<TContext, TArg> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosure AsAnonymous<TContext, TArg>(this ClosureRefAction<TContext, TArg> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    
    // Mutating Actions
    public static AnonymousClosure AsAnonymous<TContext>(this MutatingClosureAction<TContext> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    public static AnonymousClosure AsAnonymous<TContext, TArg>(this MutatingClosureAction<TContext, TArg> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    public static AnonymousClosure AsAnonymous<TContext, TArg>(this MutatingClosureRefAction<TContext, TArg> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    
    // Normal Functions
    public static AnonymousClosure AsAnonymous<TContext, TReturn>(this ClosureFunc<TContext, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosure AsAnonymous<TContext, TArg, TReturn>(this ClosureFunc<TContext, TArg, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosure AsAnonymous<TContext, TArg, TReturn>(this ClosureRefFunc<TContext, TArg, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    
    // Mutating Functions
    public static AnonymousClosure AsAnonymous<TContext, TReturn>(this MutatingClosureFunc<TContext, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    public static AnonymousClosure AsAnonymous<TContext, TArg, TReturn>(this MutatingClosureFunc<TContext, TArg, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    public static AnonymousClosure AsAnonymous<TContext, TArg, TReturn>(this MutatingClosureRefFunc<TContext, TArg, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    
    // Conversion to Closure Actions and Functions
    // Normal Actions
    public static ClosureAction<TContext> AsClosureAction<TContext>(this AnonymousClosure closure) =>
        Closure.Action(
            closure.Context.As<TContext>(),
            closure.Delegate as Action<TContext> ?? throw new InvalidCastException($"Delegate must be of type {typeof(Action<TContext>).Name} when converting to {nameof(ClosureAction<TContext>)}.")
        );
    public static ClosureAction<TContext, TArg> AsClosureAction<TContext, TArg>(this AnonymousClosure closure) =>
        Closure.Action(
            closure.Context.As<TContext>(),
            closure.Delegate as Action<TContext, TArg> ?? throw new InvalidCastException($"Delegate must be of type {typeof(Action<TContext, TArg>).Name} when converting to {nameof(ClosureAction<TContext, TArg>)}.")
        );
    public static ClosureRefAction<TContext, TArg> AsClosureRefAction<TContext, TArg>(this AnonymousClosure closure) =>
        Closure.RefAction(
            closure.Context.As<TContext>(),
            closure.Delegate as RefActionWithNormalContext<TContext, TArg> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefActionWithNormalContext<TContext, TArg>).Name} when converting to {nameof(ClosureRefAction<TContext, TArg>)}.")
        );
    
    // Mutating Actions
    public static MutatingClosureAction<TContext> AsMutatingClosureAction<TContext>(this AnonymousClosure closure) =>
        MutatingClosure.Action(
            closure.Context.As<TContext>(),
            closure.Delegate as RefAction<TContext> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefAction<TContext>).Name} when converting to {nameof(MutatingClosureAction<TContext>)}.")
        );
    public static MutatingClosureAction<TContext, TArg> AsMutatingClosureAction<TContext, TArg>(this AnonymousClosure closure) =>
        MutatingClosure.Action(
            closure.Context.As<TContext>(),
            closure.Delegate as ActionWithRefContext<TContext, TArg> ?? throw new InvalidCastException($"Delegate must be of type {typeof(ActionWithRefContext<TContext, TArg>).Name} when converting to {nameof(MutatingClosureAction<TContext, TArg>)}.")
        );
    public static MutatingClosureRefAction<TContext, TArg> AsMutatingClosureRefAction<TContext, TArg>(this AnonymousClosure closure) =>
        MutatingClosure.RefAction(
            closure.Context.As<TContext>(), 
            closure.Delegate as RefAction<TContext, TArg> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefAction<TContext, TArg>).Name} when converting to {nameof(MutatingClosureRefAction<TContext, TArg>)}.")
        );

    // Normal Functions
    public static ClosureFunc<TContext, TReturn> AsClosureFunc<TContext, TReturn>(this AnonymousClosure closure) =>
        Closure.Func(
            closure.Context.As<TContext>(),
            closure.Delegate as Func<TContext, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(Func<TContext, TReturn>).Name} when converting to {nameof(ClosureFunc<TContext, TReturn>)}.")
        );
    public static ClosureFunc<TContext, TArg, TReturn> AsClosureFunc<TContext, TArg, TReturn>(this AnonymousClosure closure) =>
        Closure.Func(
            closure.Context.As<TContext>(),
            closure.Delegate as Func<TContext, TArg, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(Func<TContext, TArg, TReturn>).Name} when converting to {nameof(ClosureFunc<TContext, TArg, TReturn>)}.")
        );
    public static ClosureRefFunc<TContext, TArg, TReturn> AsClosureRefFunc<TContext, TArg, TReturn>(this AnonymousClosure closure) =>
        Closure.RefFunc(
            closure.Context.As<TContext>(),
            closure.Delegate as RefFuncWithNormalContext<TContext, TArg, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefFuncWithNormalContext<TContext, TArg, TReturn>).Name} when converting to {nameof(ClosureRefFunc<TContext, TArg, TReturn>)}.")
        );

    // Mutating Functions
    public static MutatingClosureFunc<TContext, TReturn> AsMutatingClosureFunc<TContext, TReturn>(this AnonymousClosure closure) =>
        MutatingClosure.Func(
            closure.Context.As<TContext>(),
            closure.Delegate as RefFunc<TContext, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefFunc<TContext, TReturn>).Name} when converting to {nameof(MutatingClosureFunc<TContext, TReturn>)}.")
        );
    public static MutatingClosureFunc<TContext, TArg, TReturn> AsMutatingClosureFunc<TContext, TArg, TReturn>(this AnonymousClosure closure) =>
        MutatingClosure.Func(
            closure.Context.As<TContext>(),
            closure.Delegate as FuncWithRefContext<TContext, TArg, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(FuncWithRefContext<TContext, TArg, TReturn>).Name} when converting to {nameof(MutatingClosureFunc<TContext, TArg, TReturn>)}.")
        );
    public static MutatingClosureRefFunc<TContext, TArg, TReturn> AsMutatingClosureRefFunc<TContext, TArg, TReturn>(this AnonymousClosure closure) =>
        MutatingClosure.RefFunc(
            closure.Context.As<TContext>(), 
            closure.Delegate as RefFunc<TContext, TArg, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefFunc<TContext, TArg, TReturn>).Name} when converting to {nameof(MutatingClosureRefFunc<TContext, TArg, TReturn>)}."
            )
        );

    // Conversion to AnonymousClosureAction
    public static AnonymousClosureAction AsAnonymousAction<TContext>(this ClosureAction<TContext> closure) where TContext : notnull =>
        AnonymousClosure.Action(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TContext, TArg>(this ClosureAction<TContext, TArg> closure) where TContext : notnull =>
        AnonymousClosure.Action<TArg>(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TContext, TArg>(this ClosureRefAction<TContext, TArg> closure) where TContext : notnull =>
        AnonymousClosure.Action<TArg>(AnonymousValue.From(closure.Context), closure.Delegate);

    public static AnonymousClosureAction AsAnonymousAction<TContext>(this MutatingClosureAction<TContext> closure) where TContext : notnull =>
        AnonymousClosure.Action(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TContext, TArg>(this MutatingClosureAction<TContext, TArg> closure) where TContext : notnull =>
        AnonymousClosure.Action<TArg>(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TContext, TArg>(this MutatingClosureRefAction<TContext, TArg> closure) where TContext : notnull =>
        AnonymousClosure.Action<TArg>(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);

    // Conversion to AnonymousClosureFunc
    public static AnonymousClosureFunc<TReturn> AsAnonymousFunc<TContext, TReturn>(this ClosureFunc<TContext, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Func<TReturn>(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TContext, TArg, TReturn>(this ClosureFunc<TContext, TArg, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Func<TArg, TReturn>(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TContext, TArg, TReturn>(this ClosureRefFunc<TContext, TArg, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Func<TArg, TReturn>(AnonymousValue.From(closure.Context), closure.Delegate);

    public static AnonymousClosureFunc<TReturn> AsAnonymousFunc<TContext, TReturn>(this MutatingClosureFunc<TContext, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Func<TReturn>(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TContext, TArg, TReturn>(this MutatingClosureFunc<TContext, TArg, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Func<TArg, TReturn>(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);
    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TContext, TArg, TReturn>(this MutatingClosureRefFunc<TContext, TArg, TReturn> closure) where TContext : notnull =>
        AnonymousClosure.Func<TArg, TReturn>(AnonymousValue.From(closure.Context), closure.Delegate, MutatingBehaviour.Mutate);

    // Conversion between anonymous closures
    public static AnonymousClosure AsAnonymous(this AnonymousClosureAction closure) =>
        AnonymousClosure.Create(closure.Context, closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosureAction AsAnonymousAction(this AnonymousClosure closure) => 
        AnonymousClosure.Action(closure.Context, closure.Delegate, closure.MutatingBehaviour);
    
    public static AnonymousClosure AsAnonymous<TArg>(this AnonymousClosureAction<TArg> closure) =>
        AnonymousClosure.Create(closure.Context, closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TArg>(this AnonymousClosure closure) => 
        AnonymousClosure.Action<TArg>(closure.Context, closure.Delegate, closure.MutatingBehaviour);

    public static AnonymousClosure AsAnonymous<TReturn>(this AnonymousClosureFunc<TReturn> closure) =>
        AnonymousClosure.Create(closure.Context, closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosureFunc<TReturn> AsAnonymousFunc<TReturn>(this AnonymousClosure closure) => 
        AnonymousClosure.Func<TReturn>(closure.Context, closure.Delegate, closure.MutatingBehaviour);
    
    public static AnonymousClosure AsAnonymous<TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        AnonymousClosure.Create(closure.Context, closure.Delegate, closure.MutatingBehaviour);

    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TArg, TReturn>(this AnonymousClosure closure) =>
        AnonymousClosure.Func<TArg, TReturn>(closure.Context, closure.Delegate, closure.MutatingBehaviour);

    // Conversion to Typed Closures
    public static ClosureAction<TContext> AsClosureAction<TContext>(this AnonymousClosureAction closure) =>
        Closure.Action(
            closure.Context.As<TContext>(),
            closure.Delegate as Action<TContext> ?? throw new InvalidCastException($"Delegate must be of type {typeof(Action<TContext>).Name} when converting to {nameof(ClosureAction<TContext>)}.")
        );
    public static ClosureAction<TContext, TArg> AsClosureAction<TContext, TArg>(this AnonymousClosureAction<TArg> closure) =>
        Closure.Action(
            closure.Context.As<TContext>(),
            closure.Delegate as Action<TContext, TArg> ?? throw new InvalidCastException($"Delegate must be of type {typeof(Action<TContext, TArg>).Name} when converting to {nameof(ClosureAction<TContext, TArg>)}.")
        );
    public static ClosureRefAction<TContext, TArg> AsClosureRefAction<TContext, TArg>(this AnonymousClosureAction<TArg> closure) =>
        Closure.RefAction(
            closure.Context.As<TContext>(),
            closure.Delegate as RefActionWithNormalContext<TContext, TArg> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefActionWithNormalContext<TContext, TArg>).Name} when converting to {nameof(ClosureRefAction<TContext, TArg>)}.")
        );

    public static MutatingClosureAction<TContext> AsMutatingClosureAction<TContext>(this AnonymousClosureAction closure) =>
        MutatingClosure.Action(
            closure.Context.As<TContext>(),
            closure.Delegate as RefAction<TContext> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefAction<TContext>).Name} when converting to {nameof(MutatingClosureAction<TContext>)}.")
        );
    public static MutatingClosureAction<TContext, TArg> AsMutatingClosureAction<TContext, TArg>(this AnonymousClosureAction<TArg> closure) =>
        MutatingClosure.Action(
            closure.Context.As<TContext>(),
            closure.Delegate as ActionWithRefContext<TContext, TArg> ?? throw new InvalidCastException($"Delegate must be of type {typeof(ActionWithRefContext<TContext, TArg>).Name} when converting to {nameof(MutatingClosureAction<TContext, TArg>)}.")
        );
    public static MutatingClosureRefAction<TContext, TArg> AsMutatingClosureRefAction<TContext, TArg>(this AnonymousClosureAction<TArg> closure) =>
        MutatingClosure.RefAction(
            closure.Context.As<TContext>(),
            closure.Delegate as RefAction<TContext, TArg> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefAction<TContext, TArg>).Name} when converting to {nameof(MutatingClosureRefAction<TContext, TArg>)}.")
        );

    public static ClosureFunc<TContext, TReturn> AsClosureFunc<TContext, TReturn>(this AnonymousClosureFunc<TReturn> closure) =>
        Closure.Func(
            closure.Context.As<TContext>(),
            closure.Delegate as Func<TContext, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(Func<TContext, TReturn>).Name} when converting to {nameof(ClosureFunc<TContext, TReturn>)}.")
        );
    public static ClosureFunc<TContext, TArg, TReturn> AsClosureFunc<TContext, TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        Closure.Func(
            closure.Context.As<TContext>(),
            closure.Delegate as Func<TContext, TArg, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(Func<TContext, TArg, TReturn>).Name} when converting to {nameof(ClosureFunc<TContext, TArg, TReturn>)}.")
        );
    public static ClosureRefFunc<TContext, TArg, TReturn> AsClosureRefFunc<TContext, TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        Closure.RefFunc(
            closure.Context.As<TContext>(),
            closure.Delegate as RefFuncWithNormalContext<TContext, TArg, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefFuncWithNormalContext<TContext, TArg, TReturn>).Name} when converting to {nameof(ClosureRefFunc<TContext, TArg, TReturn>)}.")
        );

    public static MutatingClosureFunc<TContext, TReturn> AsMutatingClosureFunc<TContext, TReturn>(this AnonymousClosureFunc<TReturn> closure) =>
        MutatingClosure.Func(
            closure.Context.As<TContext>(),
            closure.Delegate as RefFunc<TContext, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefFunc<TContext, TReturn>).Name} when converting to {nameof(MutatingClosureFunc<TContext, TReturn>)}.")
        );
    public static MutatingClosureFunc<TContext, TArg, TReturn> AsMutatingClosureFunc<TContext, TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        MutatingClosure.Func(
            closure.Context.As<TContext>(),
            closure.Delegate as FuncWithRefContext<TContext, TArg, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(FuncWithRefContext<TContext, TArg, TReturn>).Name} when converting to {nameof(MutatingClosureFunc<TContext, TArg, TReturn>)}.")
        );
    public static MutatingClosureRefFunc<TContext, TArg, TReturn> AsMutatingClosureRefFunc<TContext, TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        MutatingClosure.RefFunc(
            closure.Context.As<TContext>(),
            closure.Delegate as RefFunc<TContext, TArg, TReturn> ?? throw new InvalidCastException($"Delegate must be of type {typeof(RefFunc<TContext, TArg, TReturn>).Name} when converting to {nameof(MutatingClosureRefFunc<TContext, TArg, TReturn>)}.")
        );
}