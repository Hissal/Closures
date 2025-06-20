namespace Closures;

/// <summary>
/// Encapsulates a function delegate with a context.
/// </summary>
public struct FuncClosure<TContext, TResult> {
    event Func<TContext, TResult> Func;
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; }
    
    FuncClosure(TContext context, Func<TContext, TResult> func) {
        Func = func;
        Context = context;
    }

    /// <summary>
    /// Adds a function to the invocation list.
    /// </summary>
    public void AddFunc(Func<TContext, TResult> func) => Func += func;

    /// <summary>
    /// Removes a function from the invocation list.
    /// </summary>
    public void RemoveFunc(Func<TContext, TResult> func) => Func -= func;

    /// <summary>
    /// Invokes the function(s) with the stored context and returns the result.
    /// </summary>
    public TResult Invoke() => Func(Context);

    /// <summary>
    /// Creates a new <see cref="FuncClosure{TContext, TResult}"/>.
    /// </summary>
    public static FuncClosure<TContext, TResult> Create(TContext context, Func<TContext, TResult> func) => new(context, func);
}

/// <summary>
/// Encapsulates a function delegate that takes an argument with a context.
/// </summary>
public struct FuncClosure<TContext, TArg, TResult> {
    event Func<TContext, TArg, TResult> Func;
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; }
    
    FuncClosure(TContext context, Func<TContext, TArg, TResult> func) {
        Func = func;
        Context = context;
    }

    /// <summary>
    /// Adds a function to the invocation list.
    /// </summary>
    public void AddFunc(Func<TContext, TArg, TResult> func) => Func += func;

    /// <summary>
    /// Removes a function from the invocation list.
    /// </summary>
    public void RemoveFunc(Func<TContext, TArg, TResult> func) => Func -= func;

    /// <summary>
    /// Invokes the function(s) with the stored context, given argument, and returns the result.
    /// </summary>
    public TResult Invoke(TArg arg) => Func(Context, arg);

    /// <summary>
    /// Creates a new <see cref="FuncClosure{TContext, TArg, TResult}"/>.
    /// </summary>
    public static FuncClosure<TContext, TArg, TResult> Create(TContext context, Func<TContext, TArg, TResult> func) => new(context, func);
}

/// <summary>
/// Encapsulates a function delegate that that takes a ref argument with a context.
/// </summary>
public struct RefFuncClosure<TContext, TArg, TResult> {
    event RefFuncWithContext<TContext, TArg, TResult> Func;
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; }
    
    RefFuncClosure(TContext context, RefFuncWithContext<TContext, TArg, TResult> func) {
        Func = func;
        Context = context;
    }

    /// <summary>
    /// Adds a ref function to the invocation list.
    /// </summary>
    public void AddFunc(RefFuncWithContext<TContext, TArg, TResult> func) => Func += func;

    /// <summary>
    /// Removes a ref function from the invocation list.
    /// </summary>
    public void RemoveFunc(RefFuncWithContext<TContext, TArg, TResult> func) => Func -= func;

    /// <summary>
    /// Invokes the ref function(s) with the stored context and argument, and returns the result.
    /// </summary>
    public TResult Invoke(ref TArg arg) => Func(Context, ref arg);

    /// <summary>
    /// Creates a new <see cref="RefFuncClosure{TContext, TArg, TResult}"/>.
    /// </summary>
    public static RefFuncClosure<TContext, TArg, TResult> Create(TContext context, RefFuncWithContext<TContext, TArg, TResult> func) => new(context, func);
}