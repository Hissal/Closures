namespace Closures;

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