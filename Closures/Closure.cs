namespace Lh.Closures;

public enum MutatingClosureBehaviour {
    Retain,
    Reset,
}

public interface IMutatingClosure {
    MutatingClosureBehaviour MutatingBehaviour { get; set; }
}

public interface IClosure {
    
}
public interface IClosure<TContext> : IClosure {
    TContext Context { get; set; }
}

public interface IClosure<TContext, TDelegate> : IClosure<TContext> where TDelegate : Delegate {
    TDelegate Delegate { get; set; }
    void Add(TDelegate action);
    void Remove(TDelegate action);
}

/// <summary>
/// Provides factory methods for creating closure structs that encapsulate context and delegate logic.
/// </summary>
public partial struct Closure {
    public static TClosure Create<TContext, TDelegate, TClosure>(TContext context, TDelegate del)
        where TClosure : struct, IClosure<TContext, TDelegate> where TDelegate : Delegate 
        => new TClosure() {
            Delegate = del,
            Context = context
        };

    public static TClosure Create<TContext, TDelegate, TClosure>(TContext context, TDelegate action, MutatingClosureBehaviour mutatingBehaviour)
        where TClosure : struct, IClosure<TContext, TDelegate>, IMutatingClosure
        where TDelegate : Delegate 
        => new TClosure {
            Delegate = action,
            Context = context,
            MutatingBehaviour = mutatingBehaviour
        };
}

// Action Closures
public partial struct Closure {
    /// <summary> Creates an <see cref="ClosureAction{TContext}"/> with the specified context and action. </summary>
    public static ClosureAction<TContext> Action<TContext>(TContext context, Action<TContext> action) =>
        Create<TContext, Action<TContext>, ClosureAction<TContext>>(context, action);
    
    /// <summary> Creates an <see cref="ClosureAction{TContext, TArg}"/> with the specified context and action. </summary>
    public static ClosureAction<TContext, TArg> Action<TContext, TArg>(TContext context, Action<TContext, TArg> action) =>
        Create<TContext, Action<TContext, TArg>, ClosureAction<TContext, TArg>>(context, action);

    /// <summary> Creates a <see cref="ClosureRefAction{TContext, TArg}"/> with the specified context and action. </summary>
    public static ClosureRefAction<TContext, TArg> Action<TContext, TArg>(TContext context, RefActionWithNormalContext<TContext, TArg> action) =>
        Create<TContext, RefActionWithNormalContext<TContext, TArg>, ClosureRefAction<TContext, TArg>>(context, action);
    
    /// <summary> Creates an <see cref="MutatingClosureAction{TContext}"/> with the specified context and action. </summary>
    public static MutatingClosureAction<TContext> Action<TContext>(TContext context, RefAction<TContext> action, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) =>
        Create<TContext, RefAction<TContext>, MutatingClosureAction<TContext>>(context, action, mutatingBehaviour);
    
    /// <summary> Creates an <see cref="MutatingClosureAction{TContext, TArg}"/> with the specified context and action. </summary>
    public static MutatingClosureAction<TContext, TArg> Action<TContext, TArg>(TContext context, ActionWithRefContext<TContext, TArg> action, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) =>
        Create<TContext, ActionWithRefContext<TContext, TArg>, MutatingClosureAction<TContext, TArg>>(context, action, mutatingBehaviour);
    
    /// <summary> Creates a <see cref="MutatingClosureRefAction{TContext, TArg}"/> with the specified context and action. </summary>
    public static MutatingClosureRefAction<TContext, TArg> Action<TContext, TArg>(TContext context, RefAction<TContext, TArg> action, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) =>
        Create<TContext, RefAction<TContext, TArg>, MutatingClosureRefAction<TContext, TArg>>(context, action, mutatingBehaviour);
    
    /// <summary> Creates a <see cref="RefClosureAction{TContext}"/> with the specified ref context and action. </summary>
    public static RefClosureAction<TContext> Action<TContext>(ref TContext context, RefAction<TContext> action) =>
        new RefClosureAction<TContext>(ref context, action);

    /// <summary> Creates a <see cref="RefClosureAction{TContext, TArg}"/> with the specified ref context and action. </summary>
    public static RefClosureAction<TContext, TArg> Action<TContext, TArg>(ref TContext context, ActionWithRefContext<TContext, TArg> action) =>
        new RefClosureAction<TContext, TArg>(ref context, action);

    /// <summary> Creates a <see cref="RefClosureRefAction{TContext, TArg}"/> with the specified ref context and action. </summary>
    public static RefClosureRefAction<TContext, TArg> Action<TContext, TArg>(ref TContext context, RefAction<TContext, TArg> action) =>
        new RefClosureRefAction<TContext, TArg>(ref context, action);
}

// Func Closures
public partial struct Closure {
    /// <summary> Creates a <see cref="ClosureFunc{TContext, TResult}"/> with the specified context and function. </summary>
    public static ClosureFunc<TContext, TResult> Func<TContext, TResult>(TContext context, Func<TContext, TResult> func) =>
        Create<TContext, Func<TContext, TResult>, ClosureFunc<TContext, TResult>>(context, func);

    /// <summary> Creates a <see cref="ClosureFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static ClosureFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, Func<TContext, TArg, TResult> func) =>
        Create<TContext, Func<TContext, TArg, TResult>, ClosureFunc<TContext, TArg, TResult>>(context, func);
    
    /// <summary> Creates a <see cref="ClosureRefFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static ClosureRefFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) =>
        Create<TContext, RefFuncWithNormalContext<TContext, TArg, TResult>, ClosureRefFunc<TContext, TArg, TResult>>(context, func);
    
    /// <summary> Creates a <see cref="MutatingClosureFunc{TContext, TResult}"/> with the specified context and function. </summary>
    public static MutatingClosureFunc<TContext, TResult> Func<TContext, TResult>(TContext context, RefFunc<TContext, TResult> func, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) =>
        Create<TContext, RefFunc<TContext, TResult>, MutatingClosureFunc<TContext, TResult>>(context, func, mutatingBehaviour);

    /// <summary> Creates a <see cref="MutatingClosureFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static MutatingClosureFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, FuncWithRefContext<TContext, TArg, TResult> func, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) =>
        Create<TContext, FuncWithRefContext<TContext, TArg, TResult>, MutatingClosureFunc<TContext, TArg, TResult>>(context, func, mutatingBehaviour);

    /// <summary> Creates a <see cref="MutatingClosureRefFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static MutatingClosureRefFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, RefFunc<TContext, TArg, TResult> func, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) =>
        Create<TContext, RefFunc<TContext, TArg, TResult>, MutatingClosureRefFunc<TContext, TArg, TResult>>(context, func, mutatingBehaviour);

    /// <summary> Creates a <see cref="RefClosureFunc{TContext, TResult}"/> with the specified ref context and function. </summary>
    public static RefClosureFunc<TContext, TResult> Func<TContext, TResult>(ref TContext context, RefFunc<TContext, TResult> func) =>
        new RefClosureFunc<TContext, TResult>(ref context, func);

    /// <summary> Creates a <see cref="RefClosureFunc{TContext, TArg, TResult}"/> with the specified ref context and function. </summary>
    public static RefClosureFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(ref TContext context, FuncWithRefContext<TContext, TArg, TResult> func) =>
        new RefClosureFunc<TContext, TArg, TResult>(ref context, func);

    /// <summary> Creates a <see cref="RefClosureRefFunc{TContext, TArg, TResult}"/> with the specified ref context and ref argument function. </summary>
    public static RefClosureRefFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(ref TContext context, RefFunc<TContext, TArg, TResult> func) =>
        new RefClosureRefFunc<TContext, TArg, TResult>(ref context, func);
}
