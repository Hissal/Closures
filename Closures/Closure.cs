namespace Closures;

public enum ClosureContextType {
    Normal,
    Ref
}

/// <summary>
/// Provides factory methods for creating closure structs that encapsulate context and delegate logic.
/// </summary>
public static class Closure {
    /// <summary>
    /// Creates an <see cref="ActionClosure{TContext}"/> with the specified context and action.
    /// </summary>
    public static ActionClosure<TContext> CreateAction<TContext>(TContext context, Action<TContext> action) =>
        ActionClosure<TContext>.Create(context, action);

    /// <summary>
    /// Creates an <see cref="ActionClosure{TContext}"/> with the specified ref context and action.
    /// </summary>
    public static ActionClosure<TContext> CreateAction<TContext>(TContext context, RefAction<TContext> action) =>
        ActionClosure<TContext>.Create(context, action);
    
    /// <summary>
    /// Creates an <see cref="ActionClosure{TContext, TArg}"/> with the specified context and action.
    /// </summary>
    public static ActionClosure<TContext, TArg> CreateAction<TContext, TArg>(TContext context, Action<TContext, TArg> action) =>
        ActionClosure<TContext, TArg>.Create(context, action);
    
    /// <summary>
    /// Creates an <see cref="ActionClosure{TContext, TArg}"/> with the specified ref context and action.
    /// </summary>
    public static ActionClosure<TContext, TArg> CreateAction<TContext, TArg>(TContext context, ActionWithRefContext<TContext, TArg> action) =>
        ActionClosure<TContext, TArg>.Create(context, action);
    
    /// <summary>
    /// Creates a <see cref="RefActionClosure{TContext, TArg}"/> with the specified context and action.
    /// </summary>
    public static RefActionClosure<TContext, TArg> CreateRefAction<TContext, TArg>(TContext context, RefActionWithNormalContext<TContext, TArg> action) =>
        RefActionClosure<TContext, TArg>.Create(context, action);
    /// <summary>
    /// Creates a <see cref="RefActionClosure{TContext, TArg}"/> with the specified ref context and action.
    /// </summary>
    public static RefActionClosure<TContext, TArg> CreateRefAction<TContext, TArg>(TContext context, RefAction<TContext, TArg> action) =>
        RefActionClosure<TContext, TArg>.Create(context, action);

    
    /// <summary>
    /// Creates a <see cref="FuncClosure{TContext, TResult}"/> with the specified context and function.
    /// </summary>
    public static FuncClosure<TContext, TResult> CreateFunc<TContext, TResult>(TContext context, Func<TContext, TResult> func) =>
        FuncClosure<TContext, TResult>.Create(context, func);

    public static FuncClosure<TContext, TResult> CreateFunc<TContext, TResult>(TContext context, RefFunc<TContext, TResult> func) =>
        FuncClosure<TContext, TResult>.Create(context, func);
    
    /// <summary>
    /// Creates a <see cref="FuncClosure{TContext, TArg, TResult}"/> with the specified context and function.
    /// </summary>
    public static FuncClosure<TContext, TArg, TResult> CreateFunc<TContext, TArg, TResult>(TContext context, Func<TContext, TArg, TResult> func) =>
        FuncClosure<TContext, TArg, TResult>.Create(context, func);
    
    /// <summary>
    /// Creates a <see cref="FuncClosure{TContext, TArg, TResult}"/> with the specified context and function.
    /// </summary>
    public static FuncClosure<TContext, TArg, TResult> CreateFunc<TContext, TArg, TResult>(TContext context, FuncWithRefContext<TContext, TArg, TResult> func) =>
        FuncClosure<TContext, TArg, TResult>.Create(context, func);

    /// <summary>
    /// Creates a <see cref="RefFuncClosure{TContext, TArg, TResult}"/> with the specified context and function.
    /// </summary>
    public static RefFuncClosure<TContext, TArg, TResult> CreateRefFunc<TContext, TArg, TResult>(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) =>
        RefFuncClosure<TContext, TArg, TResult>.Create(context, func);
    
    public static RefFuncClosure<TContext, TArg, TResult> CreateRefFunc<TContext, TArg, TResult>(TContext context, RefFunc<TContext, TArg, TResult> func) =>
        RefFuncClosure<TContext, TArg, TResult>.Create(context, func);
}