namespace Closures;

internal readonly struct NormalOrRefAction<TContext> {
    readonly Action<TContext>? normalContextAction;
    readonly RefAction<TContext>? refContextAction;

    public readonly ClosureContextType ClosureContextType;
    
    internal bool IsNormal => ClosureContextType == ClosureContextType.Normal;
    internal bool IsRef => ClosureContextType == ClosureContextType.Ref;
    
    internal Action<TContext> AsNormal => normalContextAction!;
    internal RefAction<TContext> AsRef => refContextAction!;
    
    public static implicit operator NormalOrRefAction<TContext>(Action<TContext> normalNormalContextAction) => 
        CreateNormal(normalNormalContextAction);
    public static implicit operator NormalOrRefAction<TContext>(RefAction<TContext> refContextAction) =>
        CreateRef(refContextAction);

    NormalOrRefAction(ClosureContextType closureContextType, Action<TContext>? normalContextAction, RefAction<TContext>? refContextAction) {
        this.normalContextAction = normalContextAction;
        this.refContextAction = refContextAction;
        ClosureContextType = closureContextType;
    }

    internal static NormalOrRefAction<TContext> CreateNormal(Action<TContext> normalNormalContextAction) =>
        new(ClosureContextType.Normal, normalNormalContextAction, null);
    internal static NormalOrRefAction<TContext> CreateRef(RefAction<TContext> refContextAction) =>
        new(ClosureContextType.Ref, null, refContextAction);
}

internal readonly struct NormalOrRefAction<TContext, TArg> {
    readonly Action<TContext, TArg>? normalContextAction;
    readonly ActionWithRefContext<TContext, TArg>? refContextAction;

    public readonly ClosureContextType ClosureContextType;
    
    internal bool IsNormal => ClosureContextType == ClosureContextType.Normal;
    internal bool IsRef => ClosureContextType == ClosureContextType.Ref;
    
    internal Action<TContext, TArg> AsNormal => normalContextAction!;
    internal ActionWithRefContext<TContext, TArg> AsRef => refContextAction!;
    
    public static implicit operator NormalOrRefAction<TContext, TArg>(Action<TContext, TArg> normalNormalContextAction) => 
        CreateNormal(normalNormalContextAction);
    public static implicit operator NormalOrRefAction<TContext, TArg>(ActionWithRefContext<TContext, TArg> refContextAction) =>
        CreateRef(refContextAction);

    NormalOrRefAction(ClosureContextType closureContextType, Action<TContext, TArg>? normalContextAction, ActionWithRefContext<TContext, TArg>? refContextAction) {
        this.normalContextAction = normalContextAction;
        this.refContextAction = refContextAction;
        ClosureContextType = closureContextType;
    }

    internal static NormalOrRefAction<TContext, TArg> CreateNormal(Action<TContext, TArg> normalNormalContextAction) =>
        new(ClosureContextType.Normal, normalNormalContextAction, null);
    internal static NormalOrRefAction<TContext, TArg> CreateRef(ActionWithRefContext<TContext, TArg> refContextAction) =>
        new(ClosureContextType.Ref, null, refContextAction);
}

internal readonly struct NormalOrRefRefAction<TContext, TArg> {
    readonly RefActionWithNormalContext<TContext, TArg>? normalContextAction;
    readonly RefAction<TContext, TArg>? refContextAction;

    public readonly ClosureContextType ClosureContextType;
    
    internal bool IsNormal => ClosureContextType == ClosureContextType.Normal;
    internal bool IsRef => ClosureContextType == ClosureContextType.Ref;
    
    internal RefActionWithNormalContext<TContext, TArg> AsNormal => normalContextAction!;
    internal RefAction<TContext, TArg> AsRef => refContextAction!;
    
    public static implicit operator NormalOrRefRefAction<TContext, TArg>(RefActionWithNormalContext<TContext, TArg> normalNormalContextAction) => 
        CreateNormal(normalNormalContextAction);
    public static implicit operator NormalOrRefRefAction<TContext, TArg>(RefAction<TContext, TArg> refContextAction) =>
        CreateRef(refContextAction);

    NormalOrRefRefAction(ClosureContextType closureContextType, RefActionWithNormalContext<TContext, TArg>? normalContextAction, RefAction<TContext, TArg>? refContextAction) {
        this.normalContextAction = normalContextAction;
        this.refContextAction = refContextAction;
        ClosureContextType = closureContextType;
    }

    internal static NormalOrRefRefAction<TContext, TArg> CreateNormal(RefActionWithNormalContext<TContext, TArg> normalNormalContextAction) =>
        new(ClosureContextType.Normal, normalNormalContextAction, null);
    internal static NormalOrRefRefAction<TContext, TArg> CreateRef(RefAction<TContext, TArg> refContextAction) =>
        new(ClosureContextType.Ref, null, refContextAction);
}