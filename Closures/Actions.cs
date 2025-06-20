namespace Closures;

/// <summary>
/// Encapsulates an action delegate with a context.
/// </summary>
public struct ActionClosure<TContext> {
    event Action<TContext>? NormalAction;
    event RefAction<TContext>? RefAction;
    
    /// <summary>
    /// The type of context used in this closure.
    /// </summary>
    public readonly ClosureContextType ClosureContextType;
    
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; private set; }

    ActionClosure(TContext context, NormalOrRefAction<TContext> action) {
        Context = context;
        
        switch (action.ClosureContextType) {
            case ClosureContextType.Normal:
                ClosureContextType = ClosureContextType.Normal;
                NormalAction = action.AsNormal;
                RefAction = null;
                break;
            case ClosureContextType.Ref:
                ClosureContextType = ClosureContextType.Ref;
                NormalAction = null;
                RefAction = action.AsRef;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), "Unknown closure type");
        }
    }

    /// <summary>
    /// Adds an action with a normal context.
    /// </summary>
    public void AddAction(Action<TContext> action) {
        if (ClosureContextType is ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot add a normal context action to a closure with ref context.");
        NormalAction += action;
    }
    
    /// <summary>
    /// Adds an action with a ref context.
    /// </summary>
    public void AddAction(RefAction<TContext> action) {
        if (ClosureContextType is ClosureContextType.Normal)
            throw new InvalidOperationException("Cannot add a ref context action to a closure with normal context.");
        RefAction += action;
    }

    /// <summary>
    /// Removes an action with normal context.
    /// </summary>
    public void RemoveAction(Action<TContext> action) {
        if (ClosureContextType is ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot remove a normal context action from a closure with ref context.");
        NormalAction -= action;
    }

    /// <summary>
    /// Removes an action with a ref context.
    /// </summary>
    public void RemoveAction(RefAction<TContext> action) {
        if (ClosureContextType is ClosureContextType.Normal)
            throw new InvalidOperationException("Cannot remove a ref context action from a closure with normal context.");
        RefAction -= action;
    }

    /// <summary>
    /// Invokes the action(s) with the stored context.
    /// </summary>
    public void Invoke() {
        switch (ClosureContextType) {
            case ClosureContextType.Normal:
                NormalAction?.Invoke(Context);
                break;
            case ClosureContextType.Ref: {
                var ctx = Context; // Capture context to avoid closure issues
                RefAction?.Invoke(ref ctx);
                Context = ctx; // Update context after invocation
                break;
            }
            default:
                throw new InvalidOperationException("Unknown closure type");
        }
    }

    /// <summary>
    /// Creates a new <see cref="ActionClosure{TContext}"/> with normal context.
    /// </summary>
    public static ActionClosure<TContext> Create(TContext context, Action<TContext> action) => new(context, action);
    /// <summary>
    /// Creates a new <see cref="ActionClosure{TContext}"/> with ref context.
    /// </summary>
    public static ActionClosure<TContext> Create(TContext context, RefAction<TContext> action) => new(context, action);
}

/// <summary>
/// Encapsulates an action delegate that takes an argument with a context.
/// </summary>
public struct ActionClosure<TContext, TArg> {
    event Action<TContext, TArg>? NormalContextAction; 
    event ActionWithRefContext<TContext, TArg>? RefContextAction;
    
    /// <summary>
    /// The type of context used in this closure.
    /// </summary>
    public readonly ClosureContextType ClosureContextType;
    
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; private set; }
    
    ActionClosure(TContext context, NormalOrRefAction<TContext, TArg> normalOrRefAction) {
        Context = context;

        switch (normalOrRefAction.ClosureContextType) {
            case ClosureContextType.Normal:
                ClosureContextType = ClosureContextType.Normal;
                NormalContextAction = normalOrRefAction.AsNormal;
                RefContextAction = null;
                break;
            case ClosureContextType.Ref:
                ClosureContextType = ClosureContextType.Ref;
                NormalContextAction = null;
                RefContextAction = normalOrRefAction.AsRef;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(normalOrRefAction), "Unknown closure type");
        }
    }

    /// <summary>
    /// Adds an action with a normal context.
    /// </summary>
    public void AddAction(Action<TContext, TArg> action) {
        if (ClosureContextType is ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot add a normal context action to a closure with ref context.");
        NormalContextAction += action;
    }
    
    /// <summary>
    /// Adds an action with a ref context.
    /// </summary>
    public void AddAction(ActionWithRefContext<TContext, TArg> action) {
        if (ClosureContextType is ClosureContextType.Normal)
            throw new InvalidOperationException("Cannot add a ref context action to a closure with normal context.");
        RefContextAction += action;
    }

    /// <summary>
    /// Removes an action with a normal context.
    /// </summary>
    public void RemoveAction(Action<TContext, TArg> action) {
        if (ClosureContextType is ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot remove a normal context action from a closure with ref context.");
        NormalContextAction -= action;
    }
    
    /// <summary>
    /// Removes an action with a ref context.
    /// </summary>
    public void RemoveAction(ActionWithRefContext<TContext, TArg> action) {
        if (ClosureContextType is ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot remove a normal context action from a closure with ref context.");
        RefContextAction -= action;
    }

    /// <summary>
    /// Invokes the action(s) with the stored context and given argument.
    /// </summary>
    public void Invoke(TArg arg) {
        switch (ClosureContextType) {
            case ClosureContextType.Normal:
                NormalContextAction?.Invoke(Context, arg);
                break;
            case ClosureContextType.Ref: {
                var ctx = Context; // Capture context to avoid closure issues
                RefContextAction?.Invoke(ref ctx, arg);
                Context = ctx; // Update context after invocation
                break;
            }
            default:
                throw new InvalidOperationException("Unknown closure type");
        }
    }

    /// <summary>
    /// Creates a new <see cref="ActionClosure{TContext, TArg}"/> with normal context.
    /// </summary>
    public static ActionClosure<TContext, TArg> Create(TContext context, Action<TContext, TArg> action) => new(context, action);
    /// <summary>
    /// Creates a new <see cref="ActionClosure{TContext, TArg}"/> with ref context.
    /// </summary>
    public static ActionClosure<TContext, TArg> Create(TContext context, ActionWithRefContext<TContext, TArg> action) => new(context, action);
}

/// <summary>
/// Encapsulates an action delegate that takes a ref argument with a context.
/// </summary>
public struct RefActionClosure<TContext, TArg> {
    event RefActionWithNormalContext<TContext, TArg>? NormalContextAction;
    event RefAction<TContext, TArg>? RefContextAction;

    /// <summary>
    /// The type of context used in this closure.
    /// </summary>
    public readonly ClosureContextType ClosureContextType;
    
    /// <summary>
    /// The context associated with this closure.
    /// </summary>
    public TContext Context { get; private set; }
    
    RefActionClosure(TContext context, NormalOrRefRefAction<TContext, TArg> normalOrRefRefAction) {
        Context = context;
        
        if (normalOrRefRefAction.IsNormal) {
            ClosureContextType = ClosureContextType.Normal;
            NormalContextAction = normalOrRefRefAction.AsNormal;
            RefContextAction = null;
        } else {
            ClosureContextType = ClosureContextType.Ref;
            NormalContextAction = null;
            RefContextAction = normalOrRefRefAction.AsRef;
        }
    }

    /// <summary>
    /// Adds an action with a normal context.
    /// </summary>
    public void AddAction(RefActionWithNormalContext<TContext, TArg> action) {
        if (ClosureContextType is ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot add a normal context action to a closure with ref context.");
        NormalContextAction += action;
    }

    /// <summary>
    /// Adds an action with a ref context.
    /// </summary>
    public void AddAction(RefAction<TContext, TArg> action) {
        if (ClosureContextType is ClosureContextType.Normal)
            throw new InvalidOperationException("Cannot add a ref context action to a closure with normal context.");
        RefContextAction += action;
    }

    /// <summary>
    /// Removes an action with a normal context.
    /// </summary>
    public void RemoveAction(RefActionWithNormalContext<TContext, TArg> action) {
        if (ClosureContextType is ClosureContextType.Ref)
            throw new InvalidOperationException("Cannot remove a normal context action from a closure with ref context.");
        NormalContextAction -= action;
    }
    /// <summary>
    /// Removes an action with a ref context.
    /// </summary>
    public void RemoveAction(RefAction<TContext, TArg> action) {
        if (ClosureContextType is ClosureContextType.Normal)
            throw new InvalidOperationException("Cannot remove a ref context action from a closure with normal context.");
        RefContextAction -= action;
    }

    /// <summary>
    /// Invokes the ref action(s) with the stored context and given ref argument.
    /// </summary>
    public void Invoke(ref TArg arg) {
        switch (ClosureContextType) {
            case ClosureContextType.Normal:
                NormalContextAction?.Invoke(Context, ref arg);
                break;
            case ClosureContextType.Ref: {
                var ctx = Context; // Capture context to avoid closure issues
                RefContextAction?.Invoke(ref ctx, ref arg);
                Context = ctx; // Update context after invocation
                break;
            }
            default:
                throw new InvalidOperationException("Unknown closure type");
        }
    }

    /// <summary>
    /// Creates a new <see cref="RefActionClosure{TContext, TArg}"/> with normal context.
    /// </summary>
    public static RefActionClosure<TContext, TArg> Create(TContext context, RefActionWithNormalContext<TContext, TArg> action) => new(context, action);
    /// <summary>
    /// Creates a new <see cref="RefActionClosure{TContext, TArg}"/> with ref context .
    /// </summary>
    public static RefActionClosure<TContext, TArg> Create(TContext context, RefAction<TContext, TArg> action) => new(context, action);
}