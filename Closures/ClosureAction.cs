#pragma warning disable CS8601 // Possible null reference assignment.
// ReSharper disable ConvertToPrimaryConstructor
namespace Lh.Closures;

public interface IAction {
    void Invoke();
}
public interface IAction<in TArg> {
    void Invoke(TArg arg);
}
public interface IRefAction<TArg> {
    void Invoke(ref TArg arg);
}

public interface IClosureAction<TContext, TAction> : IClosure<TContext, TAction>, IAction where TAction : Delegate {

}

public interface IClosureAction<TContext, in TArg, TAction> : IClosure<TContext, TAction>, IAction<TArg> where TAction : Delegate {

}

public interface IClosureRefAction<TContext, TArg, TAction> : IClosureAction<TContext, TArg, TAction>, IRefAction<TArg> where TAction : Delegate {
    
}

public struct ClosureAction<TContext> : IClosureAction<TContext, Action<TContext>> {
    public Action<TContext> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public TContext Context { get; set; }

    public ClosureAction(TContext context, Action<TContext> action) {
        Context = context;
        Delegate = action;
    }

    public void Add(Action<TContext> action) => Delegate += action;
    public void Remove(Action<TContext> action) => Delegate -= action;
    
    public void Invoke() => Delegate?.Invoke(Context);
}

public struct ClosureAction<TContext, TArg> : IClosureAction<TContext, TArg, Action<TContext, TArg>> {
    public Action<TContext, TArg> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public TContext Context { get; set; }
    
    public ClosureAction(TContext context, Action<TContext, TArg> action) {
        Context = context;
        Delegate = action;
    }
    
    public void Add(Action<TContext, TArg> action) => Delegate += action;
    public void Remove(Action<TContext, TArg> action) => Delegate -= action;
    
    public void Invoke(TArg arg) => Delegate?.Invoke(Context, arg);
}

public struct ClosureRefAction<TContext, TArg> : IClosureRefAction<TContext, TArg, RefActionWithNormalContext<TContext, TArg>> {
    public RefActionWithNormalContext<TContext, TArg> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public TContext Context { get; set; }
    
    public ClosureRefAction(TContext context, RefActionWithNormalContext<TContext, TArg> action) {
        Context = context;
        Delegate = action;
    }
    
    public void Add(RefActionWithNormalContext<TContext, TArg> action) => Delegate += action;
    public void Remove(RefActionWithNormalContext<TContext, TArg> action) => Delegate -= action;
    
    public void Invoke(ref TArg arg) => Delegate?.Invoke(Context, ref arg);
    public void Invoke(TArg arg) => Invoke(ref arg);
}

public struct MutatingClosureAction<TContext> : IClosureAction<TContext, RefAction<TContext>>, IMutatingClosure {
    public RefAction<TContext> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    
    public TContext Context {
        get => context; 
        set => context = value;
    }
    
    TContext context;

    public MutatingClosureAction(TContext context, RefAction<TContext> action, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        this.context = context;
        Delegate = action;
        MutatingBehaviour = mutatingBehaviour;
    }
    
    public void Add(RefAction<TContext> action) => Delegate += action;
    public void Remove(RefAction<TContext> action) => Delegate -= action;
    
    public void Invoke() {
        if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
            Delegate?.Invoke(ref context);
            return;
        }
        
        var copiedContext = context;
        Delegate?.Invoke(ref copiedContext);
    }
}

public struct MutatingClosureAction<TContext, TArg> : IClosureAction<TContext, TArg, ActionWithRefContext<TContext, TArg>>, IMutatingClosure {
    public ActionWithRefContext<TContext, TArg> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    
    public TContext Context {
        get => context; 
        set => context = value;
    }
    TContext context;
    
    public MutatingClosureAction(TContext context, ActionWithRefContext<TContext, TArg> action, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        this.context = context;
        Delegate = action;
        MutatingBehaviour = mutatingBehaviour;
    }
    
    public void Add(ActionWithRefContext<TContext, TArg> action) => Delegate += action;
    public void Remove(ActionWithRefContext<TContext, TArg> action) => Delegate -= action;
    
    public void Invoke(TArg arg) {
        if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
            Delegate?.Invoke(ref context, arg);
            return;
        }
        
        var copiedContext = context;
        Delegate?.Invoke(ref copiedContext, arg);
    }
}

public struct MutatingClosureRefAction<TContext, TArg> : IClosureRefAction<TContext, TArg, RefAction<TContext, TArg>>, IMutatingClosure {
    public RefAction<TContext, TArg> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public MutatingClosureBehaviour MutatingBehaviour { get; set; }
    
    public TContext Context {
        get => context; 
        set => context = value;
    }
    
    TContext context;

    public MutatingClosureRefAction(TContext context, RefAction<TContext, TArg> action, MutatingClosureBehaviour mutatingBehaviour = MutatingClosureBehaviour.Retain) {
        this.context = context;
        Delegate = action;
        MutatingBehaviour = mutatingBehaviour;
    }
    
    public void Add(RefAction<TContext, TArg> action) => Delegate += action;
    public void Remove(RefAction<TContext, TArg> action) => Delegate -= action;
    
    public void Invoke(ref TArg arg) {
        if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
            Delegate?.Invoke(ref context, ref arg);
            return;
        }
        
        var copiedContext = context;
        Delegate?.Invoke(ref copiedContext, ref arg);
    }

    public void Invoke(TArg arg) => Invoke(ref arg);
}

public ref struct RefClosureAction<TContext> : IClosureAction<TContext, RefAction<TContext>>, IRefClosure<TContext> {
    public RefAction<TContext> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public TContext Context {
        get => context; 
        set => context = value;
    }
    
    public ref TContext RefContext => ref context;
    readonly ref TContext context;

    public RefClosureAction(ref TContext context, RefAction<TContext> action) {
        this.context = ref context;
        Delegate = action;
    }
    
    public void Add(RefAction<TContext> action) => Delegate += action;
    public void Remove(RefAction<TContext> action) => Delegate -= action;
    
    public void Invoke() => Delegate?.Invoke(ref context);
}

public ref struct RefClosureAction<TContext, TArg> : IClosureAction<TContext, TArg, ActionWithRefContext<TContext, TArg>>, IRefClosure<TContext> {
    public ActionWithRefContext<TContext, TArg> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public TContext Context {
        get => context; 
        set => context = value;
    }

    public ref TContext RefContext => ref context;
    readonly ref TContext context;
    
    public RefClosureAction(ref TContext context, ActionWithRefContext<TContext, TArg> action) {
        this.context = ref context;
        Delegate = action;
    }
    
    public void Add(ActionWithRefContext<TContext, TArg> action) => Delegate += action;
    public void Remove(ActionWithRefContext<TContext, TArg> action) => Delegate -= action;
    
    public void Invoke(TArg arg) => Delegate?.Invoke(ref context, arg);
}

public ref struct RefClosureRefAction<TContext, TArg> : IClosureRefAction<TContext, TArg, RefAction<TContext, TArg>>, IRefClosure<TContext> {
    public RefAction<TContext, TArg> Delegate { get; set; }
    public bool DelegateIsNull => Delegate is null;
    
    public TContext Context {
        get => context; 
        set => context = value;
    }
    public ref TContext RefContext => ref context;
    
    readonly ref TContext context;
    
    public RefClosureRefAction(ref TContext context, RefAction<TContext, TArg> action) {
        this.context = ref context;
        Delegate = action;
    }
    
    public void Add(RefAction<TContext, TArg> action) => Delegate += action;
    public void Remove(RefAction<TContext, TArg> action) => Delegate -= action;
    
    public void Invoke(ref TArg arg) => Delegate?.Invoke(ref context, ref arg);
    public void Invoke(TArg arg) => Delegate?.Invoke(ref context, ref arg);
}
