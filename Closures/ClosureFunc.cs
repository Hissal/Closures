namespace Lh.Closures;

public partial struct Closure {

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

public interface IClosureFunc<TContext, out TResult, TFunc> : IClosure<TContext, TFunc>, IFunc<TResult> where TFunc : Delegate {

}

public interface IClosureFunc<TContext, in TArg, out TResult, TFunc> : IClosure<TContext, TFunc>, IFunc<TArg, TResult> where TFunc : Delegate {

}

public interface IClosureRefFunc<TContext, TArg, out TResult, TFunc> : IClosureFunc<TContext, TArg, TResult, TFunc>, IRefFunc<TArg, TResult> where TFunc : Delegate {
    
}

public struct ClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, Func<TContext, TResult>> {
    public Func<TContext, TResult> Delegate { get; set; }
    public TContext Context { get; set; }

    public ClosureFunc(TContext context, Func<TContext, TResult> func) {
        Context = context;
        Delegate = func;
    }

    public void Add(Func<TContext, TResult> func) => Delegate += func;
    public void Remove(Func<TContext, TResult> func) => Delegate -= func;

    public TResult Invoke() => Delegate.Invoke(Context);
}

public struct ClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, Func<TContext, TArg, TResult>> {
    public Func<TContext, TArg, TResult> Delegate { get; set; }
    public TContext Context { get; set; }

    public ClosureFunc(TContext context, Func<TContext, TArg, TResult> func) {
        Context = context;
        Delegate = func;
    }

    public void Add(Func<TContext, TArg, TResult> func) => Delegate += func;
    public void Remove(Func<TContext, TArg, TResult> func) => Delegate -= func;

    public TResult Invoke(TArg arg) => Delegate.Invoke(Context, arg);
}

public struct ClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFuncWithNormalContext<TContext, TArg, TResult>> {
    public RefFuncWithNormalContext<TContext, TArg, TResult> Delegate { get; set; }
    public TContext Context { get; set; }

    public ClosureRefFunc(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) {
        Context = context;
        Delegate = func;
    }

    public void Add(RefFuncWithNormalContext<TContext, TArg, TResult> func) => Delegate += func;
    public void Remove(RefFuncWithNormalContext<TContext, TArg, TResult> func) => Delegate -= func;
    
    public TResult Invoke(TArg arg) => Delegate.Invoke(Context, ref arg);
    public TResult Invoke(ref TArg arg) => Delegate.Invoke(Context, ref arg);
}

public struct MutatingClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, RefFunc<TContext, TResult>>, IMutatingClosure {
    public RefFunc<TContext, TResult> Delegate { get; set; }
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    TContext context;

    public MutatingClosureFunc(TContext context, RefFunc<TContext, TResult> func, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        this.context = context;
        Delegate = func;
        MutatingBehaviour = mutatingBehaviour;
    }

    public void Add(RefFunc<TContext, TResult> func) => Delegate += func;
    public void Remove(RefFunc<TContext, TResult> func) => Delegate -= func;

    public TResult Invoke() {
        if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
            return Delegate.Invoke(ref context);
        }
        var copiedContext = context;
        return Delegate.Invoke(ref copiedContext);
    }
}

public struct MutatingClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>>, IMutatingClosure {
    public FuncWithRefContext<TContext, TArg, TResult> Delegate { get; set; }
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    TContext context;

    public MutatingClosureFunc(TContext context, FuncWithRefContext<TContext, TArg, TResult> func, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        this.context = context;
        Delegate = func;
        MutatingBehaviour = mutatingBehaviour;
    }

    public void Add(FuncWithRefContext<TContext, TArg, TResult> func) => Delegate += func;
    public void Remove(FuncWithRefContext<TContext, TArg, TResult> func) => Delegate -= func;

    public TResult Invoke(TArg arg) {
        if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
            return Delegate.Invoke(ref context, arg);
        }
        var copiedContext = context;
        return Delegate.Invoke(ref copiedContext, arg);
    }
}

public struct MutatingClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>>, IMutatingClosure {
    public RefFunc<TContext, TArg, TResult> Delegate { get; set; }
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    TContext context;

    public MutatingClosureRefFunc(TContext context, RefFunc<TContext, TArg, TResult> func, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        this.context = context;
        Delegate = func;
        MutatingBehaviour = mutatingBehaviour;
    }

    public void Add(RefFunc<TContext, TArg, TResult> func) => Delegate += func;
    public void Remove(RefFunc<TContext, TArg, TResult> func) => Delegate -= func;

    public TResult Invoke(ref TArg arg) {
        if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
            return Delegate.Invoke(ref context, ref arg);
        }
        var copiedContext = context;
        return Delegate.Invoke(ref copiedContext, ref arg);
    }
    public TResult Invoke(TArg arg) => Invoke(ref arg);
}

public ref struct RefClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, RefFunc<TContext, TResult>> {
    public RefFunc<TContext, TResult> Delegate { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    readonly ref TContext context;

    public RefClosureFunc(ref TContext context, RefFunc<TContext, TResult> func) {
        this.context = ref context;
        Delegate = func;
    }

    public void Add(RefFunc<TContext, TResult> func) => Delegate += func;
    public void Remove(RefFunc<TContext, TResult> func) => Delegate -= func;

    public TResult Invoke() => Delegate.Invoke(ref context);
}

public ref struct RefClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>> {
    public FuncWithRefContext<TContext, TArg, TResult> Delegate { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    readonly ref TContext context;

    public RefClosureFunc(ref TContext context, FuncWithRefContext<TContext, TArg, TResult> func) {
        this.context = ref context;
        Delegate = func;
    }

    public void Add(FuncWithRefContext<TContext, TArg, TResult> func) => Delegate += func;
    public void Remove(FuncWithRefContext<TContext, TArg, TResult> func) => Delegate -= func;

    public TResult Invoke(TArg arg) => Delegate.Invoke(ref context, arg);
}

public ref struct RefClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>> {
    public RefFunc<TContext, TArg, TResult> Delegate { get; set; }
    public TContext Context {
        get => context;
        set => context = value;
    }
    readonly ref TContext context;

    public RefClosureRefFunc(ref TContext context, RefFunc<TContext, TArg, TResult> func) {
        this.context = ref context;
        Delegate = func;
    }

    public void Add(RefFunc<TContext, TArg, TResult> func) => Delegate += func;
    public void Remove(RefFunc<TContext, TArg, TResult> func) => Delegate -= func;

    public TResult Invoke(ref TArg arg) => Delegate.Invoke(ref context, ref arg);
    public TResult Invoke(TArg arg) => Delegate.Invoke(ref context, ref arg);
}
