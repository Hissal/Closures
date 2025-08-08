#pragma warning disable CS8601 // Possible null reference assignment.
// ReSharper disable ConvertToPrimaryConstructor
namespace Closures {
    public interface IClosureAction<TContext, TAction> : IClosure<TContext, TAction> where TAction : Delegate {
        void Invoke();
    }

    public interface IClosureAction<TContext, in TArg, TAction> : IClosure<TContext, TAction> where TAction : Delegate {
        void Invoke(TArg arg);
    }

    public interface IClosureRefAction<TContext, TArg, TAction> : IClosureAction<TContext, TArg, TAction> where TAction : Delegate {
        void Invoke(ref TArg arg);
    }

    /// <summary>
    /// Captures a variable context and an action delegate to be invoked with the context.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    public struct ClosureAction<TContext> : IClosureAction<TContext, Action<TContext>> {
        public Action<TContext> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context { get; set; }

        public ClosureAction(TContext context, Action<TContext> action) {
            Context = context;
            Delegate = action;
        }

        public void Add(Action<TContext> @delegate) => Delegate += @delegate;
        public void Remove(Action<TContext> @delegate) => Delegate -= @delegate;
    
        public void Invoke() => Delegate?.Invoke(Context);
    }

    /// <summary>
    /// Captures a variable context and an action delegate to be invoked with the context and an argument.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the action with.</typeparam>
    public struct ClosureAction<TContext, TArg> : IClosureAction<TContext, TArg, Action<TContext, TArg>> {
        public Action<TContext, TArg> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context { get; set; }
    
        public ClosureAction(TContext context, Action<TContext, TArg> action) {
            Context = context;
            Delegate = action;
        }
    
        public void Add(Action<TContext, TArg> @delegate) => Delegate += @delegate;
        public void Remove(Action<TContext, TArg> @delegate) => Delegate -= @delegate;
    
        public void Invoke(TArg arg) => Delegate?.Invoke(Context, arg);
    }

    /// <summary>
    /// Captures a variable context and an action delegate to be invoked with the context and a ref argument.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the action with.</typeparam>
    public struct ClosureRefAction<TContext, TArg> : IClosureRefAction<TContext, TArg, RefActionWithNormalContext<TContext, TArg>> {
        public RefActionWithNormalContext<TContext, TArg> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context { get; set; }
    
        public ClosureRefAction(TContext context, RefActionWithNormalContext<TContext, TArg> action) {
            Context = context;
            Delegate = action;
        }
    
        public void Add(RefActionWithNormalContext<TContext, TArg> @delegate) => Delegate += @delegate;
        public void Remove(RefActionWithNormalContext<TContext, TArg> @delegate) => Delegate -= @delegate;
    
        public void Invoke(ref TArg arg) => Delegate?.Invoke(Context, ref arg);
        public void Invoke(TArg arg) => Invoke(ref arg);
    }

    /// <summary>
    /// Captures a variable context and an action delegate to be invoked with the context by reference,
    /// allowing mutation of the stored context within the action.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <remarks>
    /// Has a <see cref="MutatingClosureBehaviour"/> property to control how the context is handled after invocation.
    /// </remarks>
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
    
        public void Add(RefAction<TContext> @delegate) => Delegate += @delegate;
        public void Remove(RefAction<TContext> @delegate) => Delegate -= @delegate;
    
        public void Invoke() {
            if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
                Delegate?.Invoke(ref context);
                return;
            }
        
            var copiedContext = context;
            Delegate?.Invoke(ref copiedContext);
        }
    }

    /// <summary>
    /// Captures a variable context and an action delegate to be invoked with the context by reference and an argument,
    /// allowing mutation of the stored context within the action.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the action with.</typeparam>
    /// <remarks>
    /// Has a <see cref="MutatingClosureBehaviour"/> property to control how the context is handled after invocation.
    /// </remarks>
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
    
        public void Add(ActionWithRefContext<TContext, TArg> @delegate) => Delegate += @delegate;
        public void Remove(ActionWithRefContext<TContext, TArg> @delegate) => Delegate -= @delegate;
    
        public void Invoke(TArg arg) {
            if (MutatingBehaviour is MutatingClosureBehaviour.Retain) {
                Delegate?.Invoke(ref context, arg);
                return;
            }
        
            var copiedContext = context;
            Delegate?.Invoke(ref copiedContext, arg);
        }
    }

    /// <summary>
    /// Captures a variable context and an action delegate to be invoked with the context and an argument by reference,
    /// allowing mutation of the stored context within the action.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the action with.</typeparam>
    /// <remarks>
    /// Has a <see cref="MutatingClosureBehaviour"/> property to control how the context is handled after invocation.
    /// </remarks>
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
    
        public void Add(RefAction<TContext, TArg> @delegate) => Delegate += @delegate;
        public void Remove(RefAction<TContext, TArg> @delegate) => Delegate -= @delegate;
    
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

    /// <summary>
    /// Captures a reference to a variable context and an action delegate to be invoked with the context by reference,
    /// allowing mutation of the original context within the action.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    public ref struct RefClosureAction<TContext> : IClosureAction<TContext, RefAction<TContext>>, IRefClosure<TContext> {
        public RefAction<TContext> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context {
            get => context; 
            set => context = value;
        }
    
        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;

        public RefClosureAction(ref TContext context, RefAction<TContext> action) {
            this.context = ref context;
            Delegate = action;
        }
    
        public void Add(RefAction<TContext> @delegate) => Delegate += @delegate;
        public void Remove(RefAction<TContext> @delegate) => Delegate -= @delegate;
    
        public void Invoke() => Delegate?.Invoke(ref context);
    }

    /// <summary>
    /// Captures a reference to a variable context and an action delegate to be invoked with the context by reference and an argument,
    /// allowing mutation of the original context within the action.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the action with.</typeparam>
    public ref struct RefClosureAction<TContext, TArg> : IClosureAction<TContext, TArg, ActionWithRefContext<TContext, TArg>>, IRefClosure<TContext> {
        public ActionWithRefContext<TContext, TArg> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context {
            get => context; 
            set => context = value;
        }

        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;
    
        public RefClosureAction(ref TContext context, ActionWithRefContext<TContext, TArg> action) {
            this.context = ref context;
            Delegate = action;
        }
    
        public void Add(ActionWithRefContext<TContext, TArg> @delegate) => Delegate += @delegate;
        public void Remove(ActionWithRefContext<TContext, TArg> @delegate) => Delegate -= @delegate;
    
        public void Invoke(TArg arg) => Delegate?.Invoke(ref context, arg);
    }

    /// <summary>
    /// Captures a reference to a variable context and an action delegate to be invoked with the context and an argument by reference,
    /// allowing mutation of the original context within the action.
    /// </summary>
    /// <typeparam name="TContext">The type of context to capture.</typeparam>
    /// <typeparam name="TArg">The type of argument to invoke the action with.</typeparam>
    public ref struct RefClosureRefAction<TContext, TArg> : IClosureRefAction<TContext, TArg, RefAction<TContext, TArg>>, IRefClosure<TContext> {
        public RefAction<TContext, TArg> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;
    
        public TContext Context {
            get => context; 
            set => context = value;
        }
    
        /// <summary>A reference to the context</summary>
        public ref TContext RefContext => ref context;
        readonly ref TContext context;
    
        public RefClosureRefAction(ref TContext context, RefAction<TContext, TArg> action) {
            this.context = ref context;
            Delegate = action;
        }
    
        public void Add(RefAction<TContext, TArg> @delegate) => Delegate += @delegate;
        public void Remove(RefAction<TContext, TArg> @delegate) => Delegate -= @delegate;
    
        public void Invoke(ref TArg arg) => Delegate?.Invoke(ref context, ref arg);
        public void Invoke(TArg arg) => Delegate?.Invoke(ref context, ref arg);
    }
}
