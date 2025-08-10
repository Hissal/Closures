// ReSharper disable ConvertToPrimaryConstructor

namespace Closures {
    public interface IClosureFunc<TContext, out TResult, TFunc> : IClosure<TContext, TFunc> where TFunc : Delegate {
        TResult Invoke();
    }

    public interface IClosureFunc<TContext, in TArg, out TResult, TFunc> : IClosure<TContext, TFunc> where TFunc : Delegate {
        TResult Invoke(TArg arg);
    }

    public interface IClosureRefFunc<TContext, TArg, out TResult, TFunc> : IClosureFunc<TContext, TArg, TResult, TFunc> where TFunc : Delegate {
        TResult Invoke(ref TArg arg);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public readonly record struct ClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, Func<TContext, TResult>> {
        public Func<TContext, TResult> Delegate { get; init; }
        public TContext Context { get; init; }

        public TResult Invoke() => Delegate.Invoke(Context);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context and an argument.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public readonly record struct ClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, Func<TContext, TArg, TResult>> {
        public Func<TContext, TArg, TResult> Delegate { get; init; }
        public TContext Context { get; init; }
        
        public TResult Invoke(TArg arg) => Delegate.Invoke(Context, arg);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context and a ref argument.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public readonly record struct ClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFuncWithNormalContext<TContext, TArg, TResult>> {
        public RefFuncWithNormalContext<TContext, TArg, TResult> Delegate { get; init; }
        public TContext Context { get; init; }
        
        public TResult Invoke(TArg arg) => Delegate.Invoke(Context, ref arg);
        public TResult Invoke(ref TArg arg) => Delegate.Invoke(Context, ref arg);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context by reference,
    /// allowing mutation of the stored context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public record struct MutatingClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, RefFunc<TContext, TResult>>, IMutatingClosure {
        public RefFunc<TContext, TResult> Delegate { get; init; }
        public TContext Context {
            get => context;
            init => context = value;
        }
        TContext context;
        

        public TResult Invoke() => Delegate.Invoke(ref context);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context by reference and an argument,
    /// allowing mutation of the stored context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public record struct MutatingClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>>, IMutatingClosure {
        public FuncWithRefContext<TContext, TArg, TResult> Delegate { get; init; }
        public TContext Context {
            get => context;
            init => context = value;
        }
        TContext context;
        
        public TResult Invoke(TArg arg) => Delegate.Invoke(ref context, arg);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context and an argument by reference,
    /// allowing mutation of the stored context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public record struct MutatingClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>>, IMutatingClosure {
        public RefFunc<TContext, TArg, TResult> Delegate { get; init; }
        public TContext Context {
            get => context;
            init => context = value;
        }
        TContext context;
        
        public TResult Invoke(TArg arg) => Delegate.Invoke(ref context, ref arg);
        public TResult Invoke(ref TArg arg) => Delegate.Invoke(ref context, ref arg);
    }

    /// <summary>
    /// Captures a reference to a variable context and a function delegate to be invoked with the context by reference,
    /// allowing mutation of the original context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public readonly ref struct RefClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, RefFunc<TContext, TResult>>, IRefClosure<TContext> {
        public RefFunc<TContext, TResult> Delegate { get; init; }
        public TContext Context {
            get => context;
            init => context = value;
        }
    
        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;

        public RefClosureFunc(ref TContext context, RefFunc<TContext, TResult> func) {
            this.context = ref context;
            Delegate = func;
        }

        public TResult Invoke() => Delegate.Invoke(ref context);

    }

    /// <summary>
    /// Captures a reference to a variable context and a function delegate to be invoked with the context by reference and an argument,
    /// allowing mutation of the original context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public readonly ref struct RefClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>>, IRefClosure<TContext> {
        public FuncWithRefContext<TContext, TArg, TResult> Delegate { get; init; }
        public TContext Context {
            get => context;
            init => context = value;
        }
    
        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;

        public RefClosureFunc(ref TContext context, FuncWithRefContext<TContext, TArg, TResult> func) {
            this.context = ref context;
            Delegate = func;
        }

        public TResult Invoke(TArg arg) => Delegate.Invoke(ref context, arg);
    }

    /// <summary>
    /// Captures a reference to a variable context and a function delegate to be invoked with the context and argument by reference,
    /// allowing mutation of the original context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public readonly ref struct RefClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>>, IRefClosure<TContext> {
        public RefFunc<TContext, TArg, TResult> Delegate { get; init; }
        public TContext Context {
            get => context;
            init => context = value;
        }
    
        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;

        public RefClosureRefFunc(ref TContext context, RefFunc<TContext, TArg, TResult> func) {
            this.context = ref context;
            Delegate = func;
        }

        public TResult Invoke(TArg arg) => Delegate.Invoke(ref context, ref arg);
        public TResult Invoke(ref TArg arg) => Delegate.Invoke(ref context, ref arg);
    }
}
