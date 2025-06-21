namespace Closures;

/// <summary>
/// Encapsulates an action delegate and context for invocation.
/// </summary>
public struct ActionClosure<TContext> {
    event Action<TContext> Action;
    public readonly TContext Context;

    public ActionClosure(TContext context, Action<TContext> action) {
        Context = context;
        Action = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
    }
    
    public void AddAction(Action<TContext> action) => Action += action;
    public void RemoveAction(Action<TContext> action) => Action -= action;

    public void Invoke() => Action.Invoke(Context);
}

/// <summary>
/// Encapsulates an action delegate with ref context for invocation.
/// </summary>
public struct ActionClosureRef<TContext> {
    event RefAction<TContext> RefAction;
    public TContext Context { get; private set; }
    
    readonly RefContextBehaviour refContextBehaviour;
    
    public ActionClosureRef(TContext context, RefAction<TContext> action, RefContextBehaviour refContextBehaviour) {
        Context = context;
        RefAction = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
        this.refContextBehaviour = refContextBehaviour;
    }
    
    public void AddAction(RefAction<TContext> action) => RefAction += action;
    public void RemoveAction(RefAction<TContext> action) => RefAction -= action;

    public void Invoke() {
        var ctx = Context;
        RefAction.Invoke(ref ctx);
        
        if (refContextBehaviour is RefContextBehaviour.KeepStoredContext)
            return;
        
        Context = ctx;
    }
}

/// <summary>
/// Encapsulates an action delegate with an argument and context for invocation.
/// </summary>
public struct ActionClosure<TContext, TArg> {
    event Action<TContext, TArg> Action;
    public readonly TContext Context;

    public ActionClosure(TContext context, Action<TContext, TArg> action) {
        Context = context;
        Action = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
    }
    
    public void AddAction(Action<TContext, TArg> action) => Action += action;
    public void RemoveAction(Action<TContext, TArg> action) => Action -= action;

    public void Invoke(TArg arg) => Action.Invoke(Context, arg);
}

/// <summary>
/// Encapsulates an action delegate with an argument and ref context for invocation.
/// </summary>
public struct ActionClosureRef<TContext, TArg> {
    event ActionWithRefContext<TContext, TArg> ActionRefCtx;
    public TContext Context { get; private set; }
    
    readonly RefContextBehaviour refContextBehaviour;
    
    public ActionClosureRef(TContext context, ActionWithRefContext<TContext, TArg> action, RefContextBehaviour refContextBehaviour) {
        Context = context;
        ActionRefCtx = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
        this.refContextBehaviour = refContextBehaviour;
    }
    
    public void AddAction(ActionWithRefContext<TContext, TArg> action) => ActionRefCtx += action;
    public void RemoveAction(ActionWithRefContext<TContext, TArg> action) => ActionRefCtx -= action;

    public void Invoke(TArg arg) {
        var ctx = Context;
        ActionRefCtx.Invoke(ref ctx, arg);
        
        if (refContextBehaviour is RefContextBehaviour.KeepStoredContext)
            return;
        
        Context = ctx;
    }
}

/// <summary>
/// Encapsulates an action delegate with context and a ref argument for invocation.
/// </summary>
public struct RefActionClosure<TContext, TArg> {
    event RefActionWithNormalContext<TContext, TArg> RefActionNormalCtx;
    public readonly TContext Context;
    
    public RefActionClosure(TContext context, RefActionWithNormalContext<TContext, TArg> action) {
        Context = context;
        RefActionNormalCtx = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
    }
    
    public void AddAction(RefActionWithNormalContext<TContext, TArg> action) => RefActionNormalCtx += action;
    public void RemoveAction(RefActionWithNormalContext<TContext, TArg> action) => RefActionNormalCtx -= action;
    
    public void Invoke(ref TArg arg) => RefActionNormalCtx.Invoke(Context, ref arg);
}

/// <summary>
/// Encapsulates an action delegate with ref context and a ref argument for invocation.
/// </summary>
public struct RefActionClosureRef<TContext, TArg> {
    event RefAction<TContext, TArg> RefAction;
    public TContext Context { get; private set; }
    
    readonly RefContextBehaviour refContextBehaviour;
    
    public RefActionClosureRef(TContext context, RefAction<TContext, TArg> action, RefContextBehaviour refContextBehaviour) {
        Context = context;
        RefAction = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
        this.refContextBehaviour = refContextBehaviour;
    }
    
    public void AddAction(RefAction<TContext, TArg> action) => RefAction += action;
    public void RemoveAction(RefAction<TContext, TArg> action) => RefAction -= action;

    public void Invoke(ref TArg arg) {
        var ctx = Context;
        RefAction.Invoke(ref ctx, ref arg);
        
        if (refContextBehaviour is RefContextBehaviour.KeepStoredContext)
            return;
        
        Context = ctx;
    }
}

/// <summary>
/// Encapsulates a ref action delegate and a reference to the original context variable for invocation.
/// <para>The original context variable passed by reference is updated when the closure is invoked.</para>
/// </summary>
public ref struct PassedRefActionClosure<TContext> {
    event RefAction<TContext> RefAction;
    readonly ref TContext context;
    
    public TContext Context => context;

    public PassedRefActionClosure(ref TContext context, RefAction<TContext> action) {
        this.context = ref context;
        RefAction = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
    }
    
    public void AddAction(RefAction<TContext> action) => RefAction += action;
    public void RemoveAction(RefAction<TContext> action) => RefAction -= action;
    
    public void Invoke() {
        RefAction.Invoke(ref context);
    }
}

/// <summary>
/// Encapsulates an action delegate with ref context and an argument, holding a reference to the original context variable for invocation.
/// <para>The original context variable passed by reference is updated when the closure is invoked.</para>
/// </summary>
public ref struct PassedRefActionClosure<TContext, TArg> {
    event ActionWithRefContext<TContext, TArg> RefAction;
    readonly ref TContext context;
    
    public TContext Context => context;

    public PassedRefActionClosure(ref TContext context, ActionWithRefContext<TContext, TArg> action) {
        this.context = ref context;
        RefAction = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
    }
    
    public void AddAction(ActionWithRefContext<TContext, TArg> action) => RefAction += action;
    public void RemoveAction(ActionWithRefContext<TContext, TArg> action) => RefAction -= action;
    
    public void Invoke(TArg arg) {
        RefAction.Invoke(ref context, arg);
    }
}

/// <summary>
/// Encapsulates a ref action delegate with ref context and a ref argument, holding a reference to the original context variable for invocation.
/// <para>The original context variable passed by reference is updated when the closure is invoked.</para>
/// </summary>
public ref struct PassedRefRefActionClosure<TContext, TArg> {
    event RefAction<TContext, TArg> RefAction;
    readonly ref TContext context;
    
    public TContext Context => context;

    public PassedRefRefActionClosure(ref TContext context, RefAction<TContext, TArg> action) {
        this.context = ref context;
        RefAction = action ?? throw new ArgumentNullException(nameof(action), "Action cannot be null.");
    }
    
    public void AddAction(RefAction<TContext, TArg> action) => RefAction += action;
    public void RemoveAction(RefAction<TContext, TArg> action) => RefAction -= action;
    
    public void Invoke(ref TArg arg) {
        RefAction.Invoke(ref context, ref arg);
    }
}
