namespace Closures;

internal readonly struct NormalOrRefFunc<TContext, TResult> {
    readonly Func<TContext, TResult>? normalContextAction;
    readonly RefFunc<TContext, TResult>? refContextAction;

    public readonly ClosureContextType ClosureContextType;
    
    internal bool IsNormal => ClosureContextType == ClosureContextType.Normal;
    internal bool IsRef => ClosureContextType == ClosureContextType.Ref;
    
    internal Func<TContext, TResult> AsNormal => normalContextAction!;
    internal RefFunc<TContext, TResult> AsRef => refContextAction!;
    
    public static implicit operator NormalOrRefFunc<TContext, TResult>(Func<TContext, TResult> normalNormalContextAction) => 
        CreateNormal(normalNormalContextAction);
    public static implicit operator NormalOrRefFunc<TContext, TResult>(RefFunc<TContext, TResult> refContextAction) =>
        CreateRef(refContextAction);

    NormalOrRefFunc(ClosureContextType closureContextType, Func<TContext, TResult>? normalContextAction, RefFunc<TContext, TResult>? refContextAction) {
        this.normalContextAction = normalContextAction;
        this.refContextAction = refContextAction;
        ClosureContextType = closureContextType;
    }

    internal static NormalOrRefFunc<TContext, TResult> CreateNormal(Func<TContext, TResult> normalNormalContextAction) =>
        new(ClosureContextType.Normal, normalNormalContextAction, null);
    internal static NormalOrRefFunc<TContext, TResult> CreateRef(RefFunc<TContext, TResult> refContextAction) =>
        new(ClosureContextType.Ref, null, refContextAction);
}

internal readonly struct NormalOrRefFunc<TContext, TArg, TResult> {
    readonly Func<TContext, TArg, TResult>? normalContextAction;
    readonly FuncWithRefContext<TContext, TArg, TResult>? refContextAction;

    public readonly ClosureContextType ClosureContextType;
    
    internal bool IsNormal => ClosureContextType == ClosureContextType.Normal;
    internal bool IsRef => ClosureContextType == ClosureContextType.Ref;
    
    internal Func<TContext, TArg, TResult> AsNormal => normalContextAction!;
    internal FuncWithRefContext<TContext, TArg, TResult> AsRef => refContextAction!;
    
    public static implicit operator NormalOrRefFunc<TContext, TArg, TResult>(Func<TContext, TArg, TResult> normalNormalContextAction) => 
        CreateNormal(normalNormalContextAction);
    public static implicit operator NormalOrRefFunc<TContext, TArg, TResult>(FuncWithRefContext<TContext, TArg, TResult> refContextAction) =>
        CreateRef(refContextAction);

    NormalOrRefFunc(ClosureContextType closureContextType, Func<TContext, TArg, TResult>? normalContextAction, FuncWithRefContext<TContext, TArg, TResult>? refContextAction) {
        this.normalContextAction = normalContextAction;
        this.refContextAction = refContextAction;
        ClosureContextType = closureContextType;
    }

    internal static NormalOrRefFunc<TContext, TArg, TResult> CreateNormal(Func<TContext, TArg, TResult> normalNormalContextAction) =>
        new(ClosureContextType.Normal, normalNormalContextAction, null);
    internal static NormalOrRefFunc<TContext, TArg, TResult> CreateRef(FuncWithRefContext<TContext, TArg, TResult> refContextAction) =>
        new(ClosureContextType.Ref, null, refContextAction);
}

internal readonly struct NormalOrRefRefFunc<TContext, TArg, TResult> {
    readonly RefFuncWithNormalContext<TContext, TArg, TResult>? normalContextAction;
    readonly RefFunc<TContext, TArg, TResult>? refContextAction;

    public readonly ClosureContextType ClosureContextType;
    
    internal bool IsNormal => ClosureContextType == ClosureContextType.Normal;
    internal bool IsRef => ClosureContextType == ClosureContextType.Ref;
    
    internal RefFuncWithNormalContext<TContext, TArg, TResult> AsNormal => normalContextAction!;
    internal RefFunc<TContext, TArg, TResult> AsRef => refContextAction!;
    
    public static implicit operator NormalOrRefRefFunc<TContext, TArg, TResult>(RefFuncWithNormalContext<TContext, TArg, TResult> normalNormalContextAction) => 
        CreateNormal(normalNormalContextAction);
    public static implicit operator NormalOrRefRefFunc<TContext, TArg, TResult>(RefFunc<TContext, TArg, TResult> refContextAction) =>
        CreateRef(refContextAction);

    NormalOrRefRefFunc(ClosureContextType closureContextType, RefFuncWithNormalContext<TContext, TArg, TResult>? normalContextAction, RefFunc<TContext, TArg, TResult>? refContextAction) {
        this.normalContextAction = normalContextAction;
        this.refContextAction = refContextAction;
        ClosureContextType = closureContextType;
    }

    internal static NormalOrRefRefFunc<TContext, TArg, TResult> CreateNormal(RefFuncWithNormalContext<TContext, TArg, TResult> normalNormalContextAction) =>
        new(ClosureContextType.Normal, normalNormalContextAction, null);
    internal static NormalOrRefRefFunc<TContext, TArg, TResult> CreateRef(RefFunc<TContext, TArg, TResult> refContextAction) =>
        new(ClosureContextType.Ref, null, refContextAction);
}