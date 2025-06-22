namespace Lh.Closures;

/// <summary>
/// Provides factory methods for creating closure structs that encapsulate context and delegate logic.
/// </summary>
public static partial class Closure {
    /// <summary> Creates an <see cref="ClosureAction{TContext}"/> with the specified context and action. </summary>
    public static ClosureAction<TContext> Action<TContext>(TContext context, Action<TContext> action) =>
        CreateAction<TContext, Action<TContext>, ClosureAction<TContext>>(context, action);

    /// <summary> Creates an <see cref="MutatingClosureAction{TContext}"/> with the specified context and action. </summary>
    public static MutatingClosureAction<TContext> Action<TContext>(TContext context, RefAction<TContext> action) =>
        CreateAction<TContext, RefAction<TContext>, MutatingClosureAction<TContext>>(context, action);
    
    /// <summary> Creates an <see cref="ClosureAction{TContext, TArg}"/> with the specified context and action. </summary>
    public static ClosureAction<TContext, TArg> Action<TContext, TArg>(TContext context, Action<TContext, TArg> action) =>
        CreateAction<TContext, TArg, Action<TContext, TArg>, ClosureAction<TContext, TArg>>(context, action);
    
    /// <summary> Creates an <see cref="MutatingClosureAction{TContext, TArg}"/> with the specified context and action. </summary>
    public static MutatingClosureAction<TContext, TArg> Action<TContext, TArg>(TContext context, ActionWithRefContext<TContext, TArg> action) =>
        CreateAction<TContext, TArg, ActionWithRefContext<TContext, TArg>, MutatingClosureAction<TContext, TArg>>(context, action);
    
    /// <summary> Creates a <see cref="ClosureRefAction{TContext, TArg}"/> with the specified context and action. </summary>
    public static ClosureRefAction<TContext, TArg> Action<TContext, TArg>(TContext context, RefActionWithNormalContext<TContext, TArg> action) =>
        CreateAction<TContext, TArg, RefActionWithNormalContext<TContext, TArg>, ClosureRefAction<TContext, TArg>>(context, action);
    
    /// <summary> Creates a <see cref="MutatingClosureRefAction{TContext, TArg}"/> with the specified context and action. </summary>
    public static MutatingClosureRefAction<TContext, TArg> Action<TContext, TArg>(TContext context, RefAction<TContext, TArg> action) =>
        CreateAction<TContext, TArg, RefAction<TContext, TArg>, MutatingClosureRefAction<TContext, TArg>>(context, action);
    
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

public static partial class Closure {
    /// <summary> Creates a <see cref="ClosureFunc{TContext, TResult}"/> with the specified context and function. </summary>
    public static ClosureFunc<TContext, TResult> Func<TContext, TResult>(TContext context, Func<TContext, TResult> func) =>
        CreateFunc<TContext, TResult, Func<TContext, TResult>, ClosureFunc<TContext, TResult>>(context, func);

    /// <summary> Creates a <see cref="MutatingClosureFunc{TContext, TResult}"/> with the specified context and function. </summary>
    public static MutatingClosureFunc<TContext, TResult> Func<TContext, TResult>(TContext context, RefFunc<TContext, TResult> func) =>
        CreateFunc<TContext, TResult, RefFunc<TContext, TResult>, MutatingClosureFunc<TContext, TResult>>(context, func);

    /// <summary> Creates a <see cref="ClosureFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static ClosureFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, Func<TContext, TArg, TResult> func) =>
        CreateFunc<TContext, TArg, TResult, Func<TContext, TArg, TResult>, ClosureFunc<TContext, TArg, TResult>>(context, func);

    /// <summary> Creates a <see cref="MutatingClosureFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static MutatingClosureFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, FuncWithRefContext<TContext, TArg, TResult> func) =>
        CreateFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>, MutatingClosureFunc<TContext, TArg, TResult>>(context, func);

    /// <summary> Creates a <see cref="ClosureRefFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static ClosureRefFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) =>
        CreateFunc<TContext, TArg, TResult, RefFuncWithNormalContext<TContext, TArg, TResult>, ClosureRefFunc<TContext, TArg, TResult>>(context, func);

    /// <summary> Creates a <see cref="MutatingClosureRefFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
    public static MutatingClosureRefFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, RefFunc<TContext, TArg, TResult> func) =>
        CreateFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>, MutatingClosureRefFunc<TContext, TArg, TResult>>(context, func);

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
