namespace Lh.Closures;

/// <summary>
/// Provides factory methods for creating closure structs that encapsulate context and delegate logic.
/// </summary>
public static partial class Closure {
    /// <summary> Creates an <see cref="ActionClosure{TContext}"/> with the specified context and action. </summary>
    public static ActionClosure<TContext> CreateAction<TContext>(TContext context, Action<TContext> action) =>
        new ActionClosure<TContext>(context, action);

    /// <summary> Creates an <see cref="ActionClosureRef{TContext}"/> with the specified context and action. </summary>
    public static ActionClosureRef<TContext> CreateAction<TContext>(TContext context, RefAction<TContext> action) =>
        new ActionClosureRef<TContext>(context, action);
    
    /// <summary> Creates an <see cref="ActionClosure{TContext, TArg}"/> with the specified context and action. </summary>
    public static ActionClosure<TContext, TArg> CreateAction<TContext, TArg>(TContext context, Action<TContext, TArg> action) =>
        new ActionClosure<TContext, TArg>(context, action);
    
    /// <summary> Creates an <see cref="ActionClosureRef{TContext, TArg}"/> with the specified context and action. </summary>
    public static ActionClosureRef<TContext, TArg> CreateAction<TContext, TArg>(TContext context, ActionWithRefContext<TContext, TArg> action) =>
        new ActionClosureRef<TContext, TArg>(context, action);
    
    /// <summary> Creates a <see cref="RefActionClosure{TContext, TArg}"/> with the specified context and action. </summary>
    public static RefActionClosure<TContext, TArg> CreateAction<TContext, TArg>(TContext context, RefActionWithNormalContext<TContext, TArg> action) =>
        new RefActionClosure<TContext, TArg>(context, action);
    
    /// <summary> Creates a <see cref="RefActionClosureRef{TContext, TArg}"/> with the specified context and action. </summary>
    public static RefActionClosureRef<TContext, TArg> CreateAction<TContext, TArg>(TContext context, RefAction<TContext, TArg> action) =>
        new RefActionClosureRef<TContext, TArg>(context, action);
    
    /// <summary> Creates a <see cref="PassedRefActionClosure{TContext}"/> with the specified ref context and action. </summary>
    public static PassedRefActionClosure<TContext> CreateAction<TContext>(ref TContext context, RefAction<TContext> action) =>
        new PassedRefActionClosure<TContext>(ref context, action);

    /// <summary> Creates a <see cref="PassedRefActionClosure{TContext, TArg}"/> with the specified ref context and action. </summary>
    public static PassedRefActionClosure<TContext, TArg> CreateAction<TContext, TArg>(ref TContext context, ActionWithRefContext<TContext, TArg> action) =>
        new PassedRefActionClosure<TContext, TArg>(ref context, action);

    /// <summary> Creates a <see cref="PassedRefRefActionClosure{TContext, TArg}"/> with the specified ref context and action. </summary>
    public static PassedRefRefActionClosure<TContext, TArg> CreateAction<TContext, TArg>(ref TContext context, RefAction<TContext, TArg> action) =>
        new PassedRefRefActionClosure<TContext, TArg>(ref context, action);
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
