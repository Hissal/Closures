namespace Closures;

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
    /// <returns> IEnumerable of the results </returns>
    public IEnumerable<TResult> InvokeSeparate() {
        foreach (var func in Func.GetInvocationList())
            yield return ((Func<TContext, TResult>)func)(Context);
    }
}
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
    /// <returns> IEnumerable of the results </returns>
    public IEnumerable<TResult> InvokeSeparate() {
        foreach (var func in Func.GetInvocationList())
            yield return ((RefFunc<TContext, TResult>)func)(ref context);
    }
}

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
    /// <returns> IEnumerable of the results </returns>
    public IEnumerable<TResult> InvokeSeparate(TArg arg) {
        foreach (var func in Func.GetInvocationList())
            yield return ((Func<TContext, TArg, TResult>)func)(Context, arg);
    }
}
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
    /// <returns> IEnumerable of the results </returns>
    public IEnumerable<TResult> InvokeSeparate(TArg arg) {
        foreach (var func in Func.GetInvocationList())
            yield return ((FuncWithRefContext<TContext, TArg, TResult>)func)(ref context, arg);
    }
}

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
    /// <returns> IEnumerable of the results </returns>
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
    /// <returns> IEnumerable of the results </returns>
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