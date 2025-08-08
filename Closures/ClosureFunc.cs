#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8601 // Possible null reference assignment.
// ReSharper disable ConvertToPrimaryConstructor

namespace Closures {
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

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public struct ClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, Func<TContext, TResult>> {
        public Func<TContext, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context { get; set; }

        public ClosureFunc(TContext context, Func<TContext, TResult> func) {
            Context = context;
            Delegate = func;
        }

        public void Add(Func<TContext, TResult> @delegate) => Delegate += @delegate;
        public void Remove(Func<TContext, TResult> @delegate) => Delegate -= @delegate;

        public TResult Invoke() => DelegateIsNull ? default : Delegate.Invoke(Context);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context and an argument.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public struct ClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, Func<TContext, TArg, TResult>> {
        public Func<TContext, TArg, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context { get; set; }

        public ClosureFunc(TContext context, Func<TContext, TArg, TResult> func) {
            Context = context;
            Delegate = func;
        }

        public void Add(Func<TContext, TArg, TResult> @delegate) => Delegate += @delegate;
        public void Remove(Func<TContext, TArg, TResult> @delegate) => Delegate -= @delegate;

        public TResult Invoke(TArg arg) => DelegateIsNull ? default : Delegate.Invoke(Context, arg);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context and a ref argument.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public struct ClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFuncWithNormalContext<TContext, TArg, TResult>> {
        public RefFuncWithNormalContext<TContext, TArg, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context { get; set; }

        public ClosureRefFunc(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) {
            Context = context;
            Delegate = func;
        }

        public void Add(RefFuncWithNormalContext<TContext, TArg, TResult> @delegate) => Delegate += @delegate;
        public void Remove(RefFuncWithNormalContext<TContext, TArg, TResult> @delegate) => Delegate -= @delegate;
    
        public TResult Invoke(TArg arg) => Invoke(ref arg);
        public TResult Invoke(ref TArg arg) => DelegateIsNull ? default : Delegate.Invoke(Context, ref arg);
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context by reference,
    /// allowing mutation of the stored context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <remarks>
    /// Has a <see cref="MutatingClosureBehaviour"/> property to control how the context is handled after invocation.
    /// </remarks>
    public struct MutatingClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, RefFunc<TContext, TResult>>, IMutatingClosure {
        public RefFunc<TContext, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
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

        public void Add(RefFunc<TContext, TResult> @delegate) => Delegate += @delegate;
        public void Remove(RefFunc<TContext, TResult> @delegate) => Delegate -= @delegate;

        public TResult Invoke() {
            if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
                return DelegateIsNull ? default : Delegate.Invoke(ref context);
            }
            var copiedContext = context;
            return DelegateIsNull ? default : Delegate.Invoke(ref copiedContext);
        }
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context by reference and an argument,
    /// allowing mutation of the stored context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <remarks>
    /// Has a <see cref="MutatingClosureBehaviour"/> property to control how the context is handled after invocation.
    /// </remarks>
    public struct MutatingClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>>, IMutatingClosure {
        public FuncWithRefContext<TContext, TArg, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
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

        public void Add(FuncWithRefContext<TContext, TArg, TResult> @delegate) => Delegate += @delegate;
        public void Remove(FuncWithRefContext<TContext, TArg, TResult> @delegate) => Delegate -= @delegate;

        public TResult Invoke(TArg arg) {
            if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
                return DelegateIsNull ? default : Delegate.Invoke(ref context, arg);
            }
            var copiedContext = context;
            return DelegateIsNull ? default : Delegate.Invoke(ref copiedContext, arg);
        }
    }

    /// <summary>
    /// Captures a variable context and a function delegate to be invoked with the context and an argument by reference,
    /// allowing mutation of the stored context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    /// <remarks>
    /// Has a <see cref="MutatingClosureBehaviour"/> property to control how the context is handled after invocation.
    /// </remarks>
    public struct MutatingClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>>, IMutatingClosure {
        public RefFunc<TContext, TArg, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
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

        public void Add(RefFunc<TContext, TArg, TResult> @delegate) => Delegate += @delegate;
        public void Remove(RefFunc<TContext, TArg, TResult> @delegate) => Delegate -= @delegate;

        public TResult Invoke(ref TArg arg) {
            if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
                return DelegateIsNull ? default : Delegate.Invoke(ref context, ref arg);
            }
            var copiedContext = context;
            return DelegateIsNull ? default : Delegate.Invoke(ref copiedContext, ref arg);
        }
        public TResult Invoke(TArg arg) => Invoke(ref arg);
    }

    /// <summary>
    /// Captures a reference to a variable context and a function delegate to be invoked with the context by reference,
    /// allowing mutation of the original context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public ref struct RefClosureFunc<TContext, TResult> : IClosureFunc<TContext, TResult, RefFunc<TContext, TResult>>, IRefClosure<TContext> {
        public RefFunc<TContext, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context {
            get => context;
            set => context = value;
        }
    
        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;

        public RefClosureFunc(ref TContext context, RefFunc<TContext, TResult> func) {
            this.context = ref context;
            Delegate = func;
        }

        public void Add(RefFunc<TContext, TResult> @delegate) => Delegate += @delegate;
        public void Remove(RefFunc<TContext, TResult> @delegate) => Delegate -= @delegate;

        public TResult Invoke() => DelegateIsNull ? default : Delegate.Invoke(ref context);

    }

    /// <summary>
    /// Captures a reference to a variable context and a function delegate to be invoked with the context by reference and an argument,
    /// allowing mutation of the original context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public ref struct RefClosureFunc<TContext, TArg, TResult> : IClosureFunc<TContext, TArg, TResult, FuncWithRefContext<TContext, TArg, TResult>>, IRefClosure<TContext> {
        public FuncWithRefContext<TContext, TArg, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context {
            get => context;
            set => context = value;
        }
    
        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;

        public RefClosureFunc(ref TContext context, FuncWithRefContext<TContext, TArg, TResult> func) {
            this.context = ref context;
            Delegate = func;
        }

        public void Add(FuncWithRefContext<TContext, TArg, TResult> @delegate) => Delegate += @delegate;
        public void Remove(FuncWithRefContext<TContext, TArg, TResult> @delegate) => Delegate -= @delegate;

        public TResult Invoke(TArg arg) => DelegateIsNull ? default : Delegate.Invoke(ref context, arg);
    }

    /// <summary>
    /// Captures a reference to a variable context and a function delegate to be invoked with the context and argument by reference,
    /// allowing mutation of the original context within the function.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the function with.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the function.</typeparam>
    public ref struct RefClosureRefFunc<TContext, TArg, TResult> : IClosureRefFunc<TContext, TArg, TResult, RefFunc<TContext, TArg, TResult>>, IRefClosure<TContext> {
        public RefFunc<TContext, TArg, TResult> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context {
            get => context;
            set => context = value;
        }
    
        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;

        public RefClosureRefFunc(ref TContext context, RefFunc<TContext, TArg, TResult> func) {
            this.context = ref context;
            Delegate = func;
        }

        public void Add(RefFunc<TContext, TArg, TResult> @delegate) => Delegate += @delegate;
        public void Remove(RefFunc<TContext, TArg, TResult> @delegate) => Delegate -= @delegate;

        public TResult Invoke(TArg arg) => Invoke(ref arg);
        public TResult Invoke(ref TArg arg) => DelegateIsNull ? default : Delegate.Invoke(ref context, ref arg);
    }
}
