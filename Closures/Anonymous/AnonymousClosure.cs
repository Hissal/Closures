namespace Closures;

public interface IAnonymousClosure : IClosure<AnonymousValue, Delegate>, IMutatingClosure {
    bool Is<TClosureType>() where TClosureType : IClosure;
}

public partial struct AnonymousClosure {
    public static TClosure Create<TClosure>(AnonymousValue context, Delegate @delegate, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain)
        where TClosure : struct, IAnonymousClosure {
        return new TClosure() {
            Context = context,
            Delegate = @delegate,
            MutatingBehaviour = mutatingBehaviour
        };
    }

    public static AnonymousClosure Create(AnonymousValue context, Delegate @delegate, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) => 
        Create<AnonymousClosure>(context, @delegate, mutatingBehaviour);

    public static AnonymousClosureAction Action(AnonymousValue context, Delegate @delegate, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        if (!AnonymousHelper.IsAction(@delegate))
            throw new ArgumentException("Delegate must not have a return value.", nameof(@delegate));

        return Create<AnonymousClosureAction>(context, @delegate, mutatingBehaviour);
    }
    public static AnonymousClosureAction<TArg> Action<TArg>(AnonymousValue context, Delegate @delegate, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        if (!AnonymousHelper.IsAction(@delegate))
            throw new ArgumentException("Delegate must not have a return value.", nameof(@delegate));
        
        if (!AnonymousHelper.HasArgOfType<TArg>(@delegate))
            throw new ArgumentException($"Delegate must have an argument of type {typeof(TArg).Name}.", nameof(@delegate));
        
        return Create<AnonymousClosureAction<TArg>>(context, @delegate, mutatingBehaviour);
    }
    
    public static AnonymousClosureFunc<TReturn> Func<TReturn>(AnonymousValue context, Delegate @delegate, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        if (!AnonymousHelper.IsFunc(@delegate))
            throw new ArgumentException("Delegate must have a return value.", nameof(@delegate));
        
        return Create<AnonymousClosureFunc<TReturn>>(context, @delegate, mutatingBehaviour);
    }
    public static AnonymousClosureFunc<TArg, TReturn> Func<TArg, TReturn>(AnonymousValue context, Delegate @delegate, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        if (!AnonymousHelper.IsFunc(@delegate))
            throw new ArgumentException("Delegate must have a return value.", nameof(@delegate));
        
        if (!AnonymousHelper.HasArgOfType<TArg>(@delegate))
            throw new ArgumentException($"Delegate must have an argument of type {typeof(TArg).Name}.", nameof(@delegate));
        
        return Create<AnonymousClosureFunc<TArg, TReturn>>(context, @delegate, mutatingBehaviour);
    }
}

public partial struct AnonymousClosure : IAnonymousClosure {
    // TODO: in the future use source generators to generate all used delegate types and their invokers
    // This would allow us to avoid the overhead of reflection and expression trees.
    // Also generate all AnonymousValue type to contain all needed values only.
    // For now use Expression trees to generate invokers for types when first used.
    
    // static readonly Dictionary<Type, Action<AnonymousClosure>> delegateTypeInvokers =
    //     new Dictionary<Type, Action<AnonymousClosure>>() {
    //         { typeof(Action<int>), closure => (closure.Delegate as Action<int>)?.Invoke(closure.Context) },
    //         { typeof(Action<long>), closure => (closure.Delegate as Action<long>)?.Invoke(closure.Context) },
    //         { typeof(Action<float>), closure => (closure.Delegate as Action<float>)?.Invoke(closure.Context) },
    //         { typeof(Action<double>), closure => (closure.Delegate as Action<double>)?.Invoke(closure.Context) },
    //         { typeof(Action<AnonymousValue>), closure => (closure.Delegate as Action<AnonymousValue>)?.Invoke(closure.Context) },
    //         
    //         { typeof(Func<int, bool>), closure => (closure.Delegate as Func<int, bool>)?.Invoke(closure.Context.As<int>()) },
    //         { typeof(Func<long, bool>), closure => (closure.Delegate as Func<long, bool>)?.Invoke(closure.Context.As<long>()) },
    //         { typeof(Func<float, bool>), closure => (closure.Delegate as Func<float, bool>)?.Invoke(closure.Context.As<float>()) },
    //         { typeof(Func<double, bool>), closure => (closure.Delegate as Func<double, bool>)?.Invoke(closure.Context.As<double>()) },
    //         { typeof(Func<AnonymousValue, bool>), closure => (closure.Delegate as Func<AnonymousValue, bool>)?.Invoke(closure.Context) }
    //     };

    AnonymousValue anonymousContext;

    public AnonymousValue Context {
        get => anonymousContext; 
        set => anonymousContext = value;
    }
    public Delegate Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    public bool Is<TClosureType>() where TClosureType : IClosure => 
        AnonymousHelper.CanConvert<TClosureType, AnonymousClosure>(this);
    
    public void Add(Delegate @delegate) => Delegate = Delegate.Combine(Delegate, @delegate);
    
    public void Remove(Delegate @delegate) => Delegate = Delegate.Remove(Delegate, @delegate);
     
    public void Invoke() {
        if (Delegate is null)
            return;
        
        AnonymousInvokers.GetActionInvoker(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }
    public void Invoke<TArg>(TArg arg) {
        if (Delegate is null)
            return;
        
        AnonymousInvokers.GetActionInvoker<TArg>(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    public void Invoke<TArg>(ref TArg arg) {
        if (Delegate is null)
            return;
        
        AnonymousInvokers.GetActionInvoker<TArg>(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    
    public TReturn? Invoke<TReturn>() {
        if (Delegate is null)
            return default;
        
        return AnonymousInvokers.GetFuncInvoker<TReturn>(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }
    
    public TReturn? Invoke<TArg, TReturn>(TArg arg) {
        if (Delegate is null)
            return default;
        
        return AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    
    public TReturn? Invoke<TArg, TReturn>(ref TArg arg) {
        if (Delegate is null)
            return default;

        return AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);

    }

    public Result TryInvoke() {
        try {
            Invoke();
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e))
                throw;

            return Result.Failure(e);
        }
    }
    
    public Result TryInvoke<TArg>(TArg arg) {
        try {
            Invoke(arg);
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e))
                throw;

            return Result.Failure(e);
        }
    }
    
    public Result TryInvoke<TArg>(ref TArg arg) {
        try {
            Invoke(ref arg);
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e))
                throw;

            return Result.Failure(e);
        }
    }
    
    public Result<TReturn> TryInvoke<TReturn>() {
        if (Delegate is null)
            return Result<TReturn>.Failure(new InvalidOperationException("Delegate is null."));
        
        try {
            var result = Invoke<TReturn>();
            return Result<TReturn>.Success(result!);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e))
                throw;

            return Result<TReturn>.Failure(e);
        }
    }
    
    public Result<TReturn> TryInvoke<TArg, TReturn>(TArg arg) {
        if (Delegate is null)
            return Result<TReturn>.Failure(new InvalidOperationException("Delegate is null."));
        
        try {
            var result = Invoke<TArg, TReturn>(arg);
            return Result<TReturn>.Success(result!);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e))
                throw;

            return Result<TReturn>.Failure(e);
        }
    }
    
    public Result<TReturn> TryInvoke<TArg, TReturn>(ref TArg arg) {
        if (Delegate is null)
            return Result<TReturn>.Failure(new InvalidOperationException("Delegate is null."));
        
        try {
            var result = Invoke<TArg, TReturn>(ref arg);
            return Result<TReturn>.Success(result!);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e))
                throw;

            return Result<TReturn>.Failure(e);
        }
    }
}

public struct AnonymousClosureAction : IAnonymousClosure, IClosureAction<AnonymousValue, Delegate> {
    AnonymousValue anonymousContext;
    public AnonymousValue Context { get; set; }
    public Delegate Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    
    public bool Is<TClosureType>() where TClosureType : IClosure => 
        AnonymousHelper.CanConvert<TClosureType, AnonymousClosureAction>(this);

    public void Add(Delegate @delegate) => Delegate = Delegate.Combine(Delegate, @delegate);
    public void Remove(Delegate @delegate) => Delegate = Delegate.Remove(Delegate, @delegate)!;
    
    public void Invoke() {
        if (Delegate is null)
            return;
        
        AnonymousInvokers.GetActionInvoker(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }

    public Result TryInvoke() {
        try {
            Invoke();
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e)) {
                throw;
            }

            return Result.Failure(e);
        }
    }
}

public struct AnonymousClosureAction<TArg> : IAnonymousClosure, IClosureAction<AnonymousValue, TArg, Delegate> {
    AnonymousValue anonymousContext;
    public AnonymousValue Context { get; set; }
    public Delegate Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    
    public bool Is<TClosureType>() where TClosureType : IClosure => 
        AnonymousHelper.CanConvert<TClosureType, AnonymousClosureAction<TArg>>(this);

    public void Add(Delegate @delegate) => Delegate = Delegate.Combine(Delegate, @delegate);
    public void Remove(Delegate @delegate) => Delegate = Delegate.Remove(Delegate, @delegate)!;
    
    public void Invoke(TArg arg) {
        if (Delegate is null)
            return;
        
        AnonymousInvokers.GetActionInvoker<TArg>(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }
    
    public Result TryInvoke(TArg arg) {
        try {
            Invoke(arg);
            return Result.Success();
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e)) {
                throw;
            }

            return Result.Failure(e);
        }
    }
}

public struct AnonymousClosureFunc<TReturn> : IAnonymousClosure, IClosureFunc<AnonymousValue, TReturn, Delegate> {
    AnonymousValue anonymousContext;
    public AnonymousValue Context { get; set; }
    public Delegate Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    
    public bool Is<TClosureType>() where TClosureType : IClosure => 
        AnonymousHelper.CanConvert<TClosureType, AnonymousClosureFunc<TReturn>>(this);

    public static implicit operator bool(AnonymousClosureFunc<TReturn> closure) => closure.DelegateIsNull;

    public void Add(Delegate @delegate) => Delegate = Delegate.Combine(Delegate, @delegate);
    public void Remove(Delegate @delegate) => Delegate = Delegate.Remove(Delegate, @delegate)!;
    
    public TReturn Invoke() {
        if (Delegate is null)
            return default!;
        
        return AnonymousInvokers.GetFuncInvoker<TReturn>(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour);
    }

    public Result<TReturn> TryInvoke() {
        try {
            var result = Invoke();
            return Result<TReturn>.Success(result);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e)) {
                throw;
            }

            return Result<TReturn>.Failure(e);
        }
    }
}

public struct AnonymousClosureFunc<TArg, TReturn> : IAnonymousClosure, IClosureFunc<AnonymousValue, TArg, TReturn, Delegate> {
    AnonymousValue anonymousContext;
    public AnonymousValue Context { get; set; }
    public Delegate Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    
    public bool Is<TClosureType>() where TClosureType : IClosure => 
        AnonymousHelper.CanConvert<TClosureType, AnonymousClosureFunc<TArg, TReturn>>(this);

    public void Add(Delegate @delegate) => Delegate = Delegate.Combine(Delegate, @delegate);
    public void Remove(Delegate @delegate) => Delegate = Delegate.Remove(Delegate, @delegate)!;
    
    public TReturn Invoke(TArg arg) {
        if (Delegate is null)
            return default!;
        
        return AnonymousInvokers.GetFuncInvoker<TArg, TReturn>(Delegate)
            .Invoke(Delegate, ref anonymousContext, MutatingBehaviour, ref arg);
    }

    public Result<TReturn> TryInvoke(TArg arg) {
        try {
            var result = Invoke(arg);
            return Result<TReturn>.Success(result);
        }
        catch (Exception e) {
            // Ignore exceptions that are expected from delegate invocation
            if (AnonymousHelper.ShouldThrow(e)) {
                throw;
            }

            return Result<TReturn>.Failure(e);
        }
    }
}

public static class AnonymousClosureExtensions {
    // Conversion to AnonymousClosure
    // Normal Actions
    public static AnonymousClosure AsAnonymous<TContext>(this ClosureAction<TContext> closure) => 
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosure AsAnonymous<TContext, TArg>(this ClosureAction<TContext, TArg> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosure AsAnonymous<TContext, TArg>(this ClosureRefAction<TContext, TArg> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    
    // Mutating Actions
    public static AnonymousClosure AsAnonymous<TContext>(this MutatingClosureAction<TContext> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosure AsAnonymous<TContext, TArg>(this MutatingClosureAction<TContext, TArg> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosure AsAnonymous<TContext, TArg>(this MutatingClosureRefAction<TContext, TArg> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    
    // Normal Functions
    public static AnonymousClosure AsAnonymous<TContext, TReturn>(this ClosureFunc<TContext, TReturn> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosure AsAnonymous<TContext, TArg, TReturn>(this ClosureFunc<TContext, TArg, TReturn> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosure AsAnonymous<TContext, TArg, TReturn>(this ClosureRefFunc<TContext, TArg, TReturn> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate);
    
    // Mutating Functions
    public static AnonymousClosure AsAnonymous<TContext, TReturn>(this MutatingClosureFunc<TContext, TReturn> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosure AsAnonymous<TContext, TArg, TReturn>(this MutatingClosureFunc<TContext, TArg, TReturn> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosure AsAnonymous<TContext, TArg, TReturn>(this MutatingClosureRefFunc<TContext, TArg, TReturn> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    
    // Conversion to Closure Actions and Functions
    // Normal Actions
    public static ClosureAction<TContext> AsClosureAction<TContext>(this AnonymousClosure closure) => 
        Closure.Action(closure.Context.As<TContext>(), (Action<TContext>)closure.Delegate);
    public static ClosureAction<TContext, TArg> AsClosureAction<TContext, TArg>(this AnonymousClosure closure) => 
        Closure.Action(closure.Context.As<TContext>(), (Action<TContext, TArg>)closure.Delegate);
    public static ClosureRefAction<TContext, TArg> AsClosureRefAction<TContext, TArg>(this AnonymousClosure closure) => 
        Closure.Action(closure.Context.As<TContext>(), (RefActionWithNormalContext<TContext, TArg>)closure.Delegate);
    
    // Mutating Actions
    public static MutatingClosureAction<TContext> AsMutatingClosureAction<TContext>(this AnonymousClosure closure) => 
        Closure.Action(closure.Context.As<TContext>(), (RefAction<TContext>)closure.Delegate, closure.MutatingBehaviour);
    public static MutatingClosureAction<TContext, TArg> AsMutatingClosureAction<TContext, TArg>(this AnonymousClosure closure) => 
        Closure.Action(closure.Context.As<TContext>(), (ActionWithRefContext<TContext, TArg>)closure.Delegate, closure.MutatingBehaviour);
    public static MutatingClosureRefAction<TContext, TArg> AsMutatingClosureRefAction<TContext, TArg>(this AnonymousClosure closure) => 
        Closure.Action(closure.Context.As<TContext>(), (RefAction<TContext, TArg>)closure.Delegate, closure.MutatingBehaviour);
    
    // Normal Functions
    public static ClosureFunc<TContext, TReturn> AsClosureFunc<TContext, TReturn>(this AnonymousClosure closure) => 
        Closure.Func(closure.Context.As<TContext>(), (Func<TContext, TReturn>)closure.Delegate);
    public static ClosureFunc<TContext, TArg, TReturn> AsClosureFunc<TContext, TArg, TReturn>(this AnonymousClosure closure) => 
        Closure.Func(closure.Context.As<TContext>(), (Func<TContext, TArg, TReturn>)closure.Delegate);
    public static ClosureRefFunc<TContext, TArg, TReturn> AsClosureRefFunc<TContext, TArg, TReturn>(this AnonymousClosure closure) => 
        Closure.Func(closure.Context.As<TContext>(), (RefFuncWithNormalContext<TContext, TArg, TReturn>)closure.Delegate);
    
    // Mutating Functions
    public static MutatingClosureFunc<TContext, TReturn> AsMutatingClosureFunc<TContext, TReturn>(this AnonymousClosure closure) => 
        Closure.Func(closure.Context.As<TContext>(), (RefFunc<TContext, TReturn>)closure.Delegate, closure.MutatingBehaviour);
    public static MutatingClosureFunc<TContext, TArg, TReturn> AsMutatingClosureFunc<TContext, TArg, TReturn>(this AnonymousClosure closure) => 
        Closure.Func(closure.Context.As<TContext>(), (FuncWithRefContext<TContext, TArg, TReturn>)closure.Delegate, closure.MutatingBehaviour);
    public static MutatingClosureRefFunc<TContext, TArg, TReturn> AsMutatingClosureRefFunc<TContext, TArg, TReturn>(this AnonymousClosure closure) => 
        Closure.Func(closure.Context.As<TContext>(), (RefFunc<TContext, TArg, TReturn>)closure.Delegate, closure.MutatingBehaviour);
    
    // Conversion to AnonymousClosureAction
    public static AnonymousClosureAction AsAnonymousAction<TContext>(this ClosureAction<TContext> closure) =>
        AnonymousClosure.Action(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TContext, TArg>(this ClosureAction<TContext, TArg> closure) =>
        AnonymousClosure.Action<TArg>(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TContext, TArg>(this ClosureRefAction<TContext, TArg> closure) =>
        AnonymousClosure.Action<TArg>(AnonymousValue.From(closure.Context), closure.Delegate);
    
    public static AnonymousClosureAction AsAnonymousAction<TContext>(this MutatingClosureAction<TContext> closure) =>
        AnonymousClosure.Action(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TContext, TArg>(this MutatingClosureAction<TContext, TArg> closure) =>
        AnonymousClosure.Action<TArg>(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosureAction<TArg> AsAnonymousAction<TContext, TArg>(this MutatingClosureRefAction<TContext, TArg> closure) =>
        AnonymousClosure.Action<TArg>(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    
    // Conversion to AnonymousClosureFunc
    public static AnonymousClosureFunc<TReturn> AsAnonymousFunc<TContext, TReturn>(this ClosureFunc<TContext, TReturn> closure) =>
        AnonymousClosure.Func<TReturn>(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TContext, TArg, TReturn>(this ClosureFunc<TContext, TArg, TReturn> closure) =>
        AnonymousClosure.Func<TArg, TReturn>(AnonymousValue.From(closure.Context), closure.Delegate);
    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TContext, TArg, TReturn>(this ClosureRefFunc<TContext, TArg, TReturn> closure) =>
        AnonymousClosure.Func<TArg, TReturn>(AnonymousValue.From(closure.Context), closure.Delegate);   
    
    public static AnonymousClosureFunc<TReturn> AsAnonymousFunc<TContext, TReturn>(this MutatingClosureFunc<TContext, TReturn> closure) =>
        AnonymousClosure.Func<TReturn>(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TContext, TArg, TReturn>(this MutatingClosureFunc<TContext, TArg, TReturn> closure) =>
        AnonymousClosure.Func<TArg, TReturn>(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosureFunc<TArg, TReturn> AsAnonymousFunc<TContext, TArg, TReturn>(this MutatingClosureRefFunc<TContext, TArg, TReturn> closure) =>
        AnonymousClosure.Func<TArg, TReturn>(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    
    // Conversion to AnonymousClosure
    public static AnonymousClosure AsAnonymous(this AnonymousClosureAction closure) => 
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosure AsAnonymous<TArg>(this AnonymousClosureAction<TArg> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    
    public static AnonymousClosure AsAnonymous<TReturn>(this AnonymousClosureFunc<TReturn> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    public static AnonymousClosure AsAnonymous<TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        AnonymousClosure.Create(AnonymousValue.From(closure.Context), closure.Delegate, closure.MutatingBehaviour);
    
    // Conversion to Typed Closures
    public static ClosureAction<TContext> AsClosureAction<TContext>(this AnonymousClosureAction closure) =>
        Closure.Action(closure.Context.As<TContext>(), (Action<TContext>)closure.Delegate);
    public static ClosureAction<TContext, TArg> AsClosureAction<TContext, TArg>(this AnonymousClosureAction<TArg> closure) =>
        Closure.Action<TContext, TArg>(closure.Context.As<TContext>(), (Action<TContext, TArg>)closure.Delegate);
    public static ClosureRefAction<TContext, TArg> AsClosureRefAction<TContext, TArg>(this AnonymousClosureAction<TArg> closure) =>
        Closure.Action<TContext, TArg>(closure.Context.As<TContext>(), (RefActionWithNormalContext<TContext, TArg>)closure.Delegate);
    
    public static MutatingClosureAction<TContext> AsMutatingClosureAction<TContext>(this AnonymousClosureAction closure) =>
        Closure.Action<TContext>(closure.Context.As<TContext>(), (RefAction<TContext>)closure.Delegate, closure.MutatingBehaviour);
    public static MutatingClosureAction<TContext, TArg> AsMutatingClosureAction<TContext, TArg>(this AnonymousClosureAction<TArg> closure) =>
        Closure.Action<TContext, TArg>(closure.Context.As<TContext>(), (ActionWithRefContext<TContext, TArg>)closure.Delegate, closure.MutatingBehaviour);
    public static MutatingClosureRefAction<TContext, TArg> AsMutatingClosureRefAction<TContext, TArg>(this AnonymousClosureAction<TArg> closure) =>
        Closure.Action<TContext, TArg>(closure.Context.As<TContext>(), (RefAction<TContext, TArg>)closure.Delegate, closure.MutatingBehaviour);
    
    public static ClosureFunc<TContext, TReturn> AsClosureFunc<TContext, TReturn>(this AnonymousClosureFunc<TReturn> closure) =>
        Closure.Func<TContext, TReturn>(closure.Context.As<TContext>(), (Func<TContext, TReturn>)closure.Delegate);
    public static ClosureFunc<TContext, TArg, TReturn> AsClosureFunc<TContext, TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        Closure.Func<TContext, TArg, TReturn>(closure.Context.As<TContext>(), (Func<TContext, TArg, TReturn>)closure.Delegate);
    public static ClosureRefFunc<TContext, TArg, TReturn> AsClosureRefFunc<TContext, TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        Closure.Func<TContext, TArg, TReturn>(closure.Context.As<TContext>(), (RefFuncWithNormalContext<TContext, TArg, TReturn>)closure.Delegate);
    
    public static MutatingClosureFunc<TContext, TReturn> AsMutatingClosureFunc<TContext, TReturn>(this AnonymousClosureFunc<TReturn> closure) =>
        Closure.Func<TContext, TReturn>(closure.Context.As<TContext>(), (RefFunc<TContext, TReturn>)closure.Delegate, closure.MutatingBehaviour);
    public static MutatingClosureFunc<TContext, TArg, TReturn> AsMutatingClosureFunc<TContext, TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        Closure.Func<TContext, TArg, TReturn>(closure.Context.As<TContext>(), (FuncWithRefContext<TContext, TArg, TReturn>)closure.Delegate, closure.MutatingBehaviour);
    public static MutatingClosureRefFunc<TContext, TArg, TReturn> AsMutatingClosureRefFunc<TContext, TArg, TReturn>(this AnonymousClosureFunc<TArg, TReturn> closure) =>
        Closure.Func<TContext, TArg, TReturn>(closure.Context.As<TContext>(), (RefFunc<TContext, TArg, TReturn>)closure.Delegate, closure.MutatingBehaviour);
}