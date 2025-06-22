namespace Lh.Closures;

/// <summary>
/// Encapsulates a function delegate and context for invocation.
/// </summary>
public struct FuncClosure<TContext, TResult> {
    event Func<TContext, TResult> Func;
    public readonly TContext Context;

    public FuncClosure(TContext context, Func<TContext, TResult> func) {
        Context = context;
        Func = func;
    }

    public void AddFunc(Func<TContext, TResult> func) => Func += func;
    public void RemoveFunc(Func<TContext, TResult> func) => Func -= func;

    public TResult Invoke() => Func(Context);

    /// <summary>
    /// Invokes the function(s) with the stored context and returns the results separately.
    /// </summary>
    /// <returns> Array of the results </returns>
    public TResult[] InvokeSeparate() {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (Func<TContext, TResult>)invocationList[i];
            results[i] = func(Context);
        }
        
        return results;
    }
}

/// <summary>
/// Encapsulates a function delegate with ref context for invocation.
/// </summary>
public struct FuncClosureRef<TContext, TResult> {
    event RefFunc<TContext, TResult> Func;
    TContext context;
    public TContext Context => context;

    public FuncClosureRef(TContext context, RefFunc<TContext, TResult> func) {
        this.context = context;
        Func = func;
    }

    public void AddFunc(RefFunc<TContext, TResult> func) => Func += func;
    public void RemoveFunc(RefFunc<TContext, TResult> func) => Func -= func;

    public TResult Invoke() => Func(ref context);

    /// <summary>
    /// Invokes the function(s) with the stored context and returns the results separately.
    /// </summary>
    /// <returns> Array of the results </returns>
    public TResult[] InvokeSeparate() {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (RefFunc<TContext, TResult>)invocationList[i];
            results[i] = func(ref context);
        }
        
        return results;
    }
}

/// <summary>
/// Encapsulates a function delegate with an argument and context for invocation.
/// </summary>
public struct FuncClosure<TContext, TArg, TResult> {
    event Func<TContext, TArg, TResult> Func;
    public readonly TContext Context;

    public FuncClosure(TContext context, Func<TContext, TArg, TResult> func) {
        Context = context;
        Func = func;
    }

    public void AddFunc(Func<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(Func<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(TArg arg) => Func(Context, arg);

    /// <summary>
    /// Invokes the function(s) with the stored context and argument and returns the results separately.
    /// </summary>
    /// <returns> Array of the results </returns>
    public TResult[] InvokeSeparate(TArg arg) {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (Func<TContext, TArg, TResult>)invocationList[i];
            results[i] = func(Context, arg);
        }
        
        return results;
    }
}

/// <summary>
/// Encapsulates a function delegate with an argument and ref context for invocation.
/// </summary>
public struct FuncClosureRef<TContext, TArg, TResult> {
    event FuncWithRefContext<TContext, TArg, TResult> Func;
    TContext context;
    public TContext Context => context;

    public FuncClosureRef(TContext context, FuncWithRefContext<TContext, TArg, TResult> func) {
        this.context = context;
        Func = func;
    }

    public void AddFunc(FuncWithRefContext<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(FuncWithRefContext<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(TArg arg) => Func(ref context, arg);

    /// <summary>
    /// Invokes the function(s) with the stored context and argument and returns the results separately.
    /// </summary>
    /// <returns> Arraay of the results </returns>
    public TResult[] InvokeSeparate(TArg arg) {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (FuncWithRefContext<TContext, TArg, TResult>)invocationList[i];
            results[i] = func(ref context, arg);
        }
        
        return results;
    }
}

/// <summary>
/// Encapsulates a ref function delegate with normal context and a ref argument for invocation.
/// </summary>
public struct RefFuncClosure<TContext, TArg, TResult> {
    event RefFuncWithNormalContext<TContext, TArg, TResult> Func;
    public readonly TContext Context;

    public RefFuncClosure(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) {
        Context = context;
        Func = func;
    }

    public void AddFunc(RefFuncWithNormalContext<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(RefFuncWithNormalContext<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(ref TArg arg) => Func(Context, ref arg);

    /// <summary>
    /// Invokes the function(s) with the stored context and argument and returns the results separately.
    /// </summary>
    /// <returns> Array of the results </returns>
    public TResult[] InvokeSeparate(ref TArg arg) {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (RefFuncWithNormalContext<TContext, TArg, TResult>)invocationList[i];
            results[i] = func(Context, ref arg);
        }
        
        return results;
    }
}

/// <summary>
/// Encapsulates a ref function delegate with ref context and a ref argument for invocation.
/// </summary>
public struct RefFuncClosureRef<TContext, TArg, TResult> {
    event RefFunc<TContext, TArg, TResult> Func;
    TContext context;
    public TContext Context => context;

    public RefFuncClosureRef(TContext context, RefFunc<TContext, TArg, TResult> func) {
        this.context = context;
        Func = func;
    }

    public void AddFunc(RefFunc<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(RefFunc<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(ref TArg arg) => Func(ref context, ref arg);

    /// <summary>
    /// Invokes the function(s) with the stored context and argument and returns the results separately.
    /// </summary>
    /// <returns> Array of the results </returns>
    public TResult[] InvokeSeparate(ref TArg arg) {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (RefFunc<TContext, TArg, TResult>)invocationList[i];
            results[i] = func(ref context, ref arg);
        }
        
        return results;
    }
}

/// <summary>
/// Encapsulates a ref function delegate and a reference to the original context variable for invocation.
/// <para>The original context variable passed by reference is updated when the closure is invoked.</para>
/// </summary>
public ref struct PassedRefFuncClosure<TContext, TResult> {
    event RefFunc<TContext, TResult> Func;
    readonly ref TContext context;
    
    public TContext Context => context;

    public PassedRefFuncClosure(ref TContext context, RefFunc<TContext, TResult> func) {
        this.context = ref context;
        Func = func;
    }

    public void AddFunc(RefFunc<TContext, TResult> func) => Func += func;
    public void RemoveFunc(RefFunc<TContext, TResult> func) => Func -= func;

    public TResult Invoke() => Func(ref context);
    
    public TResult[] InvokeSeparate() {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (RefFunc<TContext, TResult>)invocationList[i];
            results[i] = func(ref context);
        }
        
        return results;
    }
}

/// <summary>
/// Encapsulates a function delegate with ref context and an argument, holding a reference to the original context variable for invocation.
/// <para>The original context variable passed by reference is updated when the closure is invoked.</para>
/// </summary>
public ref struct PassedRefFuncClosure<TContext, TArg, TResult> {
    event FuncWithRefContext<TContext, TArg, TResult> Func;
    readonly ref TContext context;
    
    public TContext Context => context;

    public PassedRefFuncClosure(ref TContext context, FuncWithRefContext<TContext, TArg, TResult> func) {
        this.context = ref context;
        Func = func;
    }

    public void AddFunc(FuncWithRefContext<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(FuncWithRefContext<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(TArg arg) => Func(ref context, arg);
    
    public TResult[] InvokeSeparate(TArg arg) {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (FuncWithRefContext<TContext, TArg, TResult>)invocationList[i];
            results[i] = func(ref context, arg);
        }
        
        return results;
    }
}

/// <summary>
/// Encapsulates a ref function delegate with ref context and a ref argument, holding a reference to the original context variable for invocation.
/// <para>The original context variable passed by reference is updated when the closure is invoked.</para>
/// </summary>
public ref struct PassedRefRefFuncClosure<TContext, TArg, TResult> {
    event RefFunc<TContext, TArg, TResult> Func;
    readonly ref TContext context;
    
    public TContext Context => context;

    public PassedRefRefFuncClosure(ref TContext context, RefFunc<TContext, TArg, TResult> func) {
        this.context = ref context;
        Func = func;
    }

    public void AddFunc(RefFunc<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(RefFunc<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(ref TArg arg) => Func(ref context, ref arg);
    
    public TResult[] InvokeSeparate(ref TArg arg) {
        var invocationList = Func.GetInvocationList();
        var results = new TResult[invocationList.Length];
        
        for (int i = 0; i < invocationList.Length; i++) {
            var func = (RefFunc<TContext, TArg, TResult>)invocationList[i];
            results[i] = func(ref context, ref arg);
        }
        
        return results;
    }
}
