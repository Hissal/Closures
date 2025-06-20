namespace Closures;

/// <summary>
/// Encapsulates a function delegate with a context.
/// </summary>
public struct FuncClosure<TContext, TResult> {
    event Func<TContext, TResult>? NormalFunc;
    event RefFunc<TContext, TResult>? RefFunc;
    
    /// <summary>
    /// The type of context used in this closure.
    /// </summary>
    public readonly ClosureContextType ContextType;
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; private set; }
    
    FuncClosure(TContext context, NormalOrRefFunc<TContext, TResult> func) {
        Context = context;

        switch (func.ClosureContextType) {
            case ClosureContextType.Normal:
                ContextType = ClosureContextType.Normal;
                NormalFunc = func.AsNormal;
                RefFunc = null;
                break;
            case ClosureContextType.Ref:
                ContextType = ClosureContextType.Ref;
                NormalFunc = null;
                RefFunc = func.AsRef;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(func), "Invalid closure context type.");
        }
    }

    /// <summary>
    /// Adds a function with normal context.
    /// </summary>
    public void AddFunc(Func<TContext, TResult> func) {
        if (ContextType == ClosureContextType.Ref) 
            throw new InvalidOperationException("Cannot add a normal function to a ref context closure.");
        NormalFunc += func;
    }
    
    /// <summary>
    /// Adds a function with ref context.
    /// </summary>
    public void AddFunc(RefFunc<TContext, TResult> func) {
        if (ContextType == ClosureContextType.Normal) 
            throw new InvalidOperationException("Cannot add a ref function to a normal context closure.");
        RefFunc += func;
    }

    /// <summary>
    /// Removes a function with normal context.
    /// </summary>
    public void RemoveFunc(Func<TContext, TResult> func) {
        if (ContextType == ClosureContextType.Ref) 
            throw new InvalidOperationException("Cannot remove a normal function from a ref context closure.");
        NormalFunc -= func;
    }
    
    /// <summary>
    /// Removes a function with ref context.
    /// </summary>
    public void RemoveFunc(RefFunc<TContext, TResult> func) {
        if (ContextType == ClosureContextType.Normal) 
            throw new InvalidOperationException("Cannot remove a ref function from a normal context closure.");
        RefFunc -= func;
    }

    /// <summary>
    /// Invokes the function(s) with the stored context and returns the result.
    /// </summary>
    public TResult Invoke() {
        switch (ContextType) {
            case ClosureContextType.Normal:
                return NormalFunc(Context);
            case ClosureContextType.Ref:
                var ctx = Context;
                var returnValue = RefFunc(ref ctx);
                Context = ctx;
                return returnValue;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Creates a new <see cref="FuncClosure{TContext, TResult}"/> with normal context.
    /// </summary>
    public static FuncClosure<TContext, TResult> Create(TContext context, Func<TContext, TResult> func) => new(context, func);
    /// <summary>
    /// Creates a new <see cref="FuncClosure{TContext, TResult}"/> with ref context.
    /// </summary>
    public static FuncClosure<TContext, TResult> Create(TContext context, RefFunc<TContext, TResult> func) => new(context, func);
}

/// <summary>
/// Encapsulates a function delegate that takes an argument with a context.
/// </summary>
public struct FuncClosure<TContext, TArg, TResult> {
    event Func<TContext, TArg, TResult>? NormalFunc;
    event FuncWithRefContext<TContext, TArg, TResult>? RefFunc;
    
    /// <summary>
    /// The type of context used in this closure.
    /// </summary>
    public readonly ClosureContextType ContextType;
    
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; private set; }
    
    FuncClosure(TContext context, NormalOrRefFunc<TContext, TArg, TResult> func) {
        Context = context;
        
        switch (func.ClosureContextType) {
            case ClosureContextType.Normal:
                ContextType = ClosureContextType.Normal;
                NormalFunc = func.AsNormal;
                RefFunc = null;
                break;
            case ClosureContextType.Ref:
                ContextType = ClosureContextType.Ref;
                NormalFunc = null;
                RefFunc = func.AsRef;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(func), "Invalid closure context type.");
        }
    }

    /// <summary>
    /// Adds a function with normal context.
    /// </summary>
    public void AddFunc(Func<TContext, TArg, TResult> func) {
        if (ContextType == ClosureContextType.Ref) 
            throw new InvalidOperationException("Cannot add a normal function to a ref context closure.");
        NormalFunc += func;
    }
    
    /// <summary>
    /// Adds a function with ref context.
    /// </summary>
    public void AddFunc(FuncWithRefContext<TContext, TArg, TResult> func) {
        if (ContextType == ClosureContextType.Normal) 
            throw new InvalidOperationException("Cannot add a ref function to a normal context closure.");
        RefFunc += func;
    }

    /// <summary>
    /// Removes a function with normal context.
    /// </summary>
    public void RemoveFunc(Func<TContext, TArg, TResult> func) {
        if (ContextType == ClosureContextType.Ref) 
            throw new InvalidOperationException("Cannot remove a normal function from a ref context closure.");
        NormalFunc -= func;
    }
    
    /// <summary>
    /// Removes a function with ref context.
    /// </summary>
    public void RemoveFunc(FuncWithRefContext<TContext, TArg, TResult> func) {
        if (ContextType == ClosureContextType.Normal) 
            throw new InvalidOperationException("Cannot remove a ref function from a normal context closure.");
        RefFunc -= func;
    }

    /// <summary>
    /// Invokes the function(s) with the stored context, given argument, and returns the result.
    /// </summary>
    public TResult Invoke(TArg arg) {
        switch (ContextType) {
            case ClosureContextType.Normal:
                return NormalFunc!(Context, arg);
            case ClosureContextType.Ref:
                var ctx = Context;
                var returnValue = RefFunc!(ref ctx, arg);
                Context = ctx;
                return returnValue;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Creates a new <see cref="FuncClosure{TContext, TArg, TResult}"/> with normal context.
    /// </summary>
    public static FuncClosure<TContext, TArg, TResult> Create(TContext context, Func<TContext, TArg, TResult> func) => new(context, func);
    /// <summary>
    /// Creates a new <see cref="FuncClosure{TContext, TArg, TResult}"/> with ref context.
    /// </summary>
    public static FuncClosure<TContext, TArg, TResult> Create(TContext context, FuncWithRefContext<TContext, TArg, TResult> func) => new(context, func);
}

/// <summary>
/// Encapsulates a function delegate that that takes a ref argument with a context.
/// </summary>
public struct RefFuncClosure<TContext, TArg, TResult> {
    event RefFuncWithNormalContext<TContext, TArg, TResult> NormalFunc;
    event RefFunc<TContext, TArg, TResult> RefFunc;
    
    /// <summary>
    /// The type of context used in this closure.
    /// </summary>
    public readonly ClosureContextType ContextType;
    
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; private set; }
    
    RefFuncClosure(TContext context, NormalOrRefRefFunc<TContext, TArg, TResult> func) {
        Context = context;
        
        switch (func.ClosureContextType) {
            case ClosureContextType.Normal:
                ContextType = ClosureContextType.Normal;
                NormalFunc = func.AsNormal;
                RefFunc = null;
                break;
            case ClosureContextType.Ref:
                ContextType = ClosureContextType.Ref;
                NormalFunc = null;
                RefFunc = func.AsRef;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(func), "Invalid closure context type.");
        }
    }

    /// <summary>
    /// Adds a function with normal context.
    /// </summary>
    public void AddFunc(RefFuncWithNormalContext<TContext, TArg, TResult> func) {
        if (ContextType == ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot add a normal function to a ref context closure.");
        NormalFunc += func;
    }

    /// <summary>
    /// Adds a function with ref context.
    /// </summary>
    public void AddFunc(RefFunc<TContext, TArg, TResult> func) {
        if (ContextType == ClosureContextType.Normal)
            throw new InvalidOperationException("Cannot add a ref function to a normal context closure.");
        RefFunc += func;
    }

    /// <summary>
    /// Removes a function with normal context.
    /// </summary>
    public void RemoveFunc(RefFuncWithNormalContext<TContext, TArg, TResult> func) {
        if (ContextType == ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot remove a normal function from a ref context closure.");
        NormalFunc -= func;
    }

    /// <summary>
    /// Removes a function with ref context.
    /// </summary>
    public void RemoveFunc(RefFunc<TContext, TArg, TResult> func) {
        if (ContextType == ClosureContextType.Normal)
            throw new InvalidOperationException("Cannot remove a ref function from a normal context closure.");
        RefFunc -= func;
    }

    /// <summary>
    /// Invokes the function(s) with the stored context and argument, and returns the result.
    /// </summary>
    public TResult Invoke(ref TArg arg) {
        switch (ContextType) {
            case ClosureContextType.Normal:
                return NormalFunc!(Context, ref arg);
            case ClosureContextType.Ref:
                var ctx = Context;
                var returnValue = RefFunc!(ref ctx, ref arg);
                Context = ctx;
                return returnValue;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// Creates a new <see cref="RefFuncClosure{TContext, TArg, TResult}"/> with ref context.
    /// </summary>
    public static RefFuncClosure<TContext, TArg, TResult> Create(TContext context, RefFunc<TContext, TArg, TResult> func)
        => new(context, func);

    /// <summary>
    /// Creates a new <see cref="RefFuncClosure{TContext, TArg, TResult}"/> with normal context.
    /// </summary>
    public static RefFuncClosure<TContext, TArg, TResult> Create(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func)
        => new(context, func);
}