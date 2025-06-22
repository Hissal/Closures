namespace Lh.Closures;

public static partial class Closure {
    public static TClosure CreateFunc<TContext, TResult, TFunc, TClosure>(TContext context, TFunc func)
        where TClosure : struct, IClosureFunc<TContext, TResult, TFunc>
        where TFunc : Delegate 
        => new TClosure() {
            Func = func,
            Context = context
        };
    
    public static TClosure CreateFunc<TContext, TArg, TResult, TFunc, TClosure>(TContext context, TFunc func)
        where TClosure : struct, IClosureFunc<TContext, TArg, TResult, TFunc>
        where TFunc : Delegate
        => new TClosure() {
            Func = func,
            Context = context
        };
}

public interface IFunc<out TResult> {
    TResult Invoke();
}
public interface IFunc<in TArg, out TResult> {
    TResult Invoke(TArg arg);
}
public interface IRefFunc<TArg, out TResult> {
    TResult Invoke(ref TArg arg);
}

public interface IClosureFunc<TContext, out TResult, TFunc> : IFunc<TResult> where TFunc : Delegate {
    TFunc Func { get; set; }
    TContext Context { get; set; }

    void AddFunc(TFunc func);
    void RemoveFunc(TFunc func);
}

public interface IClosureFunc<TContext, in TArg, out TResult, TFunc> : IFunc<TArg, TResult> where TFunc : Delegate {
    TFunc Func { get; set; }
    TContext Context { get; set; }

    void AddFunc(TFunc func);
    void RemoveFunc(TFunc func);
}

public interface IClosureRefFunc<TContext, TArg, out TResult, TFunc> : IClosureFunc<TContext, TArg, TResult, TFunc>, IRefFunc<TArg, TResult> where TFunc : Delegate {
    
}

public struct ClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, Func<TContext, TResult>> {
    public Func<TContext, TResult> Func { get; set; }
    public TContext Context { get; set; }

    public ClosureFunc(TContext context, Func<TContext, TResult> func) {
        Context = context;
        Func = func;
    }

    public void AddFunc(Func<TContext, TResult> func) => Func += func;
    public void RemoveFunc(Func<TContext, TResult> func) => Func -= func;

    public TResult Invoke() => Func.Invoke(Context);
}

public struct ClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, Func<TContext, TArg, TResult>> {
    public Func<TContext, TArg, TResult> Func { get; set; }
    public TContext Context { get; set; }

    public ClosureFunc(TContext context, Func<TContext, TArg, TResult> func) {
        Context = context;
        Func = func;
    }

    public void AddFunc(Func<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(Func<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(TArg arg) => Func.Invoke(Context, arg);
}

public struct ClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFuncWithNormalContext<TContext, TArg, TResult>> {
    public RefFuncWithNormalContext<TContext, TArg, TResult> Func { get; set; }
    public TContext Context { get; set; }

    public ClosureRefFunc(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) {
        Context = context;
        Func = func;
    }

    public void AddFunc(RefFuncWithNormalContext<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(RefFuncWithNormalContext<TContext, TArg, TResult> func) => Func -= func;
    
    public TResult Invoke(TArg arg) => Func.Invoke(Context, ref arg);
    public TResult Invoke(ref TArg arg) => Func.Invoke(Context, ref arg);
}

public struct MutatingClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, RefFunc<TContext, TResult>> {
    public RefFunc<TContext, TResult> Func { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    
    TContext context;

    public MutatingClosureFunc(TContext context, RefFunc<TContext, TResult> func) {
        this.context = context;
        Func = func;
    }

    public void AddFunc(RefFunc<TContext, TResult> func) => Func += func;
    public void RemoveFunc(RefFunc<TContext, TResult> func) => Func -= func;

    public TResult Invoke() => Func.Invoke(ref context);
}

public struct MutatingClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>> {
    public FuncWithRefContext<TContext, TArg, TResult> Func { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    TContext context;

    public MutatingClosureFunc(TContext context, FuncWithRefContext<TContext, TArg, TResult> func) {
        this.context = context;
        Func = func;
    }

    public void AddFunc(FuncWithRefContext<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(FuncWithRefContext<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(TArg arg) => Func.Invoke(ref context, arg);
}

public struct MutatingClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>> {
    public RefFunc<TContext, TArg, TResult> Func { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    TContext context;

    public MutatingClosureRefFunc(TContext context, RefFunc<TContext, TArg, TResult> func) {
        this.context = context;
        Func = func;
    }

    public void AddFunc(RefFunc<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(RefFunc<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(ref TArg arg) => Func.Invoke(ref context, ref arg);
    public TResult Invoke(TArg arg) => Func.Invoke(ref context, ref arg);
}

public ref struct RefClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, RefFunc<TContext, TResult>> {
    public RefFunc<TContext, TResult> Func { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    readonly ref TContext context;

    public RefClosureFunc(ref TContext context, RefFunc<TContext, TResult> func) {
        this.context = ref context;
        Func = func;
    }

    public void AddFunc(RefFunc<TContext, TResult> func) => Func += func;
    public void RemoveFunc(RefFunc<TContext, TResult> func) => Func -= func;

    public TResult Invoke() => Func.Invoke(ref context);
}

public ref struct RefClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>> {
    public FuncWithRefContext<TContext, TArg, TResult> Func { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    readonly ref TContext context;

    public RefClosureFunc(ref TContext context, FuncWithRefContext<TContext, TArg, TResult> func) {
        this.context = ref context;
        Func = func;
    }

    public void AddFunc(FuncWithRefContext<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(FuncWithRefContext<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(TArg arg) => Func.Invoke(ref context, arg);
}

public ref struct RefClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>> {
    public RefFunc<TContext, TArg, TResult> Func { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    readonly ref TContext context;

    public RefClosureRefFunc(ref TContext context, RefFunc<TContext, TArg, TResult> func) {
        this.context = ref context;
        Func = func;
    }

    public void AddFunc(RefFunc<TContext, TArg, TResult> func) => Func += func;
    public void RemoveFunc(RefFunc<TContext, TArg, TResult> func) => Func -= func;

    public TResult Invoke(ref TArg arg) => Func.Invoke(ref context, ref arg);
    public TResult Invoke(TArg arg) => Func.Invoke(ref context, ref arg);
}
