namespace Lh.Closures;

/// <summary>
/// Provides factory methods for creating closure structs that encapsulate context and delegate logic.
/// </summary>
public static partial class Closure {
    /// <summary> Creates an <see cref="ActionClosure{TContext}"/> with the specified context and action. </summary>
    public static ClosureAction<TContext> Action<TContext>(TContext context, Action<TContext> action) =>
        Create<TContext, Action<TContext>, ClosureAction<TContext>>(context, action);

    /// <summary> Creates an <see cref="ActionClosureRef{TContext}"/> with the specified context and action. </summary>
    public static MutatingClosureAction<TContext> Action<TContext>(TContext context, RefAction<TContext> action) =>
        Create<TContext, RefAction<TContext>, MutatingClosureAction<TContext>>(context, action);
    
    /// <summary> Creates an <see cref="ActionClosure{TContext, TArg}"/> with the specified context and action. </summary>
    public static ClosureAction<TContext, TArg> Action<TContext, TArg>(TContext context, Action<TContext, TArg> action) =>
        Create<TContext, TArg, Action<TContext, TArg>, ClosureAction<TContext, TArg>>(context, action);
    
    /// <summary> Creates an <see cref="ActionClosureRef{TContext, TArg}"/> with the specified context and action. </summary>
    public static MutatingClosureAction<TContext, TArg> Action<TContext, TArg>(TContext context, ActionWithRefContext<TContext, TArg> action) =>
        Create<TContext, TArg, ActionWithRefContext<TContext, TArg>, MutatingClosureAction<TContext, TArg>>(context, action);
    
    /// <summary> Creates a <see cref="RefActionClosure{TContext, TArg}"/> with the specified context and action. </summary>
    public static ClosureRefAction<TContext, TArg> Action<TContext, TArg>(TContext context, RefActionWithNormalContext<TContext, TArg> action) =>
        Create<TContext, TArg, RefActionWithNormalContext<TContext, TArg>, ClosureRefAction<TContext, TArg>>(context, action);
    
    /// <summary> Creates a <see cref="RefActionClosureRef{TContext, TArg}"/> with the specified context and action. </summary>
    public static MutatingClosureRefAction<TContext, TArg> Action<TContext, TArg>(TContext context, RefAction<TContext, TArg> action) =>
        Create<TContext, TArg, RefAction<TContext, TArg>, MutatingClosureRefAction<TContext, TArg>>(context, action);
    
    /// <summary> Creates a <see cref="PassedRefActionClosure{TContext}"/> with the specified ref context and action. </summary>
    public static RefClosureAction<TContext> Action<TContext>(ref TContext context, RefAction<TContext> action) =>
        new RefClosureAction<TContext>(ref context, action);

    /// <summary> Creates a <see cref="PassedRefActionClosure{TContext, TArg}"/> with the specified ref context and action. </summary>
    public static RefClosureAction<TContext, TArg> Action<TContext, TArg>(ref TContext context, ActionWithRefContext<TContext, TArg> action) =>
        new RefClosureAction<TContext, TArg>(ref context, action);

    /// <summary> Creates a <see cref="PassedRefRefActionClosure{TContext, TArg}"/> with the specified ref context and action. </summary>
    public static RefClosureRefAction<TContext, TArg> Action<TContext, TArg>(ref TContext context, RefAction<TContext, TArg> action) =>
        new RefClosureRefAction<TContext, TArg>(ref context, action);
}

public static partial class Closure {
    /// <summary> Creates a <see cref="FuncClosure{TContext, TResult}"/> with the specified context and function. </summary>
    public static FuncClosure<TContext, TResult> CreateFunc<TContext, TResult>(TContext context, Func<TContext, TResult> func) =>
        new FuncClosure<TContext, TResult>(context, func);

    /// <summary> Creates a <see cref="FuncClosureRef{TContext, TResult}"/> with the specified context and function. </summary>
    public static FuncClosureRef<TContext, TResult> CreateFunc<TContext, TResult>(TContext context, RefFunc<TContext, TResult> func) =>
        new FuncClosureRef<TContext, TResult>(context, func);
    
    /// <summary> Creates a <see cref="FuncClosure{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static FuncClosure<TContext, TArg, TResult> CreateFunc<TContext, TArg, TResult>(TContext context, Func<TContext, TArg, TResult> func) =>
        new FuncClosure<TContext, TArg, TResult>(context, func);
    
    /// <summary> Creates a <see cref="FuncClosureRef{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static FuncClosureRef<TContext, TArg, TResult> CreateFunc<TContext, TArg, TResult>(TContext context, FuncWithRefContext<TContext, TArg, TResult> func) =>
        new FuncClosureRef<TContext, TArg, TResult>(context, func);

    /// <summary> Creates a <see cref="RefFuncClosure{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static RefFuncClosure<TContext, TArg, TResult> CreateFunc<TContext, TArg, TResult>(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) =>
        new RefFuncClosure<TContext, TArg, TResult>(context, func);
    
    /// <summary> Creates a <see cref="RefFuncClosureRef{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static RefFuncClosureRef<TContext, TArg, TResult> CreateFunc<TContext, TArg, TResult>(TContext context, RefFunc<TContext, TArg, TResult> func) =>
        new RefFuncClosureRef<TContext, TArg, TResult>(context, func);
    
    /// <summary> Creates a <see cref="PassedRefFuncClosure{TContext, TResult}"/> with the specified ref context and function. </summary>
    public static PassedRefFuncClosure<TContext, TResult> CreateFunc<TContext, TResult>(ref TContext context, RefFunc<TContext, TResult> func) =>
        new PassedRefFuncClosure<TContext, TResult>(ref context, func);

    /// <summary> Creates a <see cref="PassedRefFuncClosure{TContext, TArg, TResult}"/> with the specified ref context and function. </summary>
    public static PassedRefFuncClosure<TContext, TArg, TResult> CreateFunc<TContext, TArg, TResult>(ref TContext context, FuncWithRefContext<TContext, TArg, TResult> func) =>
        new PassedRefFuncClosure<TContext, TArg, TResult>(ref context, func);

    /// <summary> Creates a <see cref="PassedRefRefFuncClosure{TContext, TArg, TResult}"/> with the specified ref context and ref argument function. </summary>
    public static PassedRefRefFuncClosure<TContext, TArg, TResult> CreateFunc<TContext, TArg, TResult>(ref TContext context, RefFunc<TContext, TArg, TResult> func) =>
        new PassedRefRefFuncClosure<TContext, TArg, TResult>(ref context, func);
}
