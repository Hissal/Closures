namespace Lh.Closures;

public static partial class Closure {
    public static TClosure CreateAction<TContext, TAction, TClosure>(TContext context, TAction action)
        where TClosure : struct, IClosureAction<TContext, TAction>
        where TAction : Delegate 
        => new TClosure() {
            Action = action,
            Context = context
        };

    public static TClosure CreateAction<TContext, TArg, TAction, TClosure>(TContext context, TAction action)
        where TClosure : struct, IClosureAction<TContext, TArg, TAction>
        where TAction : Delegate
        => new TClosure() {
            Action = action,
            Context = context
        };
}

public interface IAction {
    void Invoke();
}
public interface IAction<in TArg> {
    void Invoke(TArg arg);
}
public interface IRefAction<TArg> {
    void Invoke(ref TArg arg);
}

public interface IClosureAction<TContext, TAction> : IAction where TAction : Delegate {
    public TAction Action { get; set; }
    TContext Context { get; set; }
    
    void AddAction(TAction action);
    void RemoveAction(TAction action);
}

public interface IClosureAction<TContext, in TArg, TAction> : IAction<TArg> where TAction : Delegate {
    public TAction Action { get; set; }
    TContext Context { get; set; }
    
    void AddAction(TAction action);
    void RemoveAction(TAction action);
}

public interface IClosureRefAction<TContext, TArg, TAction> : IClosureAction<TContext, TArg, TAction>, IRefAction<TArg> where TAction : Delegate {
    
}

public struct ClosureAction<TContext> : IClosureAction<TContext, Action<TContext>> {
    public Action<TContext> Action { get; set; }
    public TContext Context { get; set; }

    public ClosureAction(TContext context, Action<TContext> action) {
        Context = context;
        Action = action;
    }
    
    public void AddAction(Action<TContext> action) => Action += action;
    public void RemoveAction(Action<TContext> action) => Action -= action;
    
    public void Invoke() => Action.Invoke(Context);
}

public struct ClosureAction<TContext, TArg> : IClosureAction<TContext, TArg, Action<TContext, TArg>> {
    public Action<TContext, TArg> Action { get; set; }
    public TContext Context { get; set; }
    
    public ClosureAction(TContext context, Action<TContext, TArg> action) {
        Context = context;
        Action = action;
    }
    
    public void AddAction(Action<TContext, TArg> action) => Action += action;
    public void RemoveAction(Action<TContext, TArg> action) => Action -= action;
    
    public void Invoke(TArg arg) => Action.Invoke(Context, arg);
}

public struct ClosureRefAction<TContext, TArg> : IClosureRefAction<TContext, TArg, RefActionWithNormalContext<TContext, TArg>> {
    public RefActionWithNormalContext<TContext, TArg> Action { get; set; }
    public TContext Context { get; set; }
    
    public ClosureRefAction(TContext context, RefActionWithNormalContext<TContext, TArg> action) {
        Context = context;
        Action = action;
    }
    
    public void AddAction(RefActionWithNormalContext<TContext, TArg> action) => Action += action;
    public void RemoveAction(RefActionWithNormalContext<TContext, TArg> action) => Action -= action;
    
    public void Invoke(ref TArg arg) => Action.Invoke(Context, ref arg);
    public void Invoke(TArg arg) => Action.Invoke(Context, ref arg);
}

public struct MutatingClosureAction<TContext> : IClosureAction<TContext, RefAction<TContext>> {
    public RefAction<TContext> Action { get; set; }
    public TContext Context {
        get => context; 
        set => context = value;
    }
    
    TContext context;

    public MutatingClosureAction(TContext context, RefAction<TContext> action) {
        Context = context;
        Action = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
    }
    
    public void AddAction(RefAction<TContext> action) => Action += action;
    public void RemoveAction(RefAction<TContext> action) => Action -= action;
    
    public void Invoke() => Action.Invoke(ref context);
}

public struct MutatingClosureAction<TContext, TArg> : IClosureAction<TContext, TArg, ActionWithRefContext<TContext, TArg>> {
    public ActionWithRefContext<TContext, TArg> Action { get; set; }
    public TContext Context {
        get => context; 
        set => context = value;
    }
    TContext context;
    
    public MutatingClosureAction(TContext context, ActionWithRefContext<TContext, TArg> action) {
        Context = context;
        Action = action;
    }
    
    public void AddAction(ActionWithRefContext<TContext, TArg> action) => Action += action;
    public void RemoveAction(ActionWithRefContext<TContext, TArg> action) => Action -= action;
    
    public void Invoke(TArg arg) => Action.Invoke(ref context, arg);
}

public struct MutatingClosureRefAction<TContext, TArg> : IClosureRefAction<TContext, TArg, RefAction<TContext, TArg>> {
    public RefAction<TContext, TArg> Action { get; set; }
    public TContext Context {
        get => context; 
        set => context = value;
    }
    
    TContext context;

    public MutatingClosureRefAction(TContext context, RefAction<TContext, TArg> action) {
        Context = context;
        Action = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
    }
    
    public void AddAction(RefAction<TContext, TArg> action) => Action += action;
    public void RemoveAction(RefAction<TContext, TArg> action) => Action -= action;
    
    public void Invoke(ref TArg arg) => Action.Invoke(ref context, ref arg);
    public void Invoke(TArg arg) => Action.Invoke(ref context, ref arg);
}

public ref struct RefClosureAction<TContext> : IClosureAction<TContext, RefAction<TContext>> {
    public RefAction<TContext> Action { get; set; }
    public TContext Context {
        get => context; 
        set => context = value;
    }
    
    ref TContext context;

    public RefClosureAction(ref TContext context, RefAction<TContext> action) {
        this.context = ref context;
        Action = action;
    }
    
    public void AddAction(RefAction<TContext> action) => Action += action;
    public void RemoveAction(RefAction<TContext> action) => Action -= action;
    
    public void Invoke() => Action.Invoke(ref context);
}

public ref struct RefClosureAction<TContext, TArg> : IClosureAction<TContext, TArg, ActionWithRefContext<TContext, TArg>> {
    public ActionWithRefContext<TContext, TArg> Action { get; set; }
    public TContext Context {
        get => context; 
        set => context = value;
    }

    readonly ref TContext context;
    
    public RefClosureAction(ref TContext context, ActionWithRefContext<TContext, TArg> action) {
        this.context = ref context;
        Action = action;
    }
    
    public void AddAction(ActionWithRefContext<TContext, TArg> action) => Action += action;
    public void RemoveAction(ActionWithRefContext<TContext, TArg> action) => Action -= action;
    
    public void Invoke(TArg arg) => Action.Invoke(ref context, arg);
}

public ref struct RefClosureRefAction<TContext, TArg> : IClosureRefAction<TContext, TArg, RefAction<TContext, TArg>> {
    public RefAction<TContext, TArg> Action { get; set; }
    public TContext Context {
        get => context; 
        set => context = value;
    }

    readonly ref TContext context;
    
    public RefClosureRefAction(ref TContext context, RefAction<TContext, TArg> action) {
        this.context = ref context;
        Action = action;
    }
    
    public void AddAction(RefAction<TContext, TArg> action) => Action += action;
    public void RemoveAction(RefAction<TContext, TArg> action) => Action -= action;
    
    public void Invoke(ref TArg arg) => Action.Invoke(ref context, ref arg);
    public void Invoke(TArg arg) => Action.Invoke(ref context, ref arg);
}
