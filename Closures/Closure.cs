using System.Collections.Concurrent;

namespace Closures {

    public interface IClosure {

    }
    public interface IClosure<TContext, TDelegate> : IClosure where TDelegate : Delegate {
        TContext Context { get; init; }
        TDelegate Delegate { get; init; }
    }
    
    public interface IMutatingClosure {
        
    }

    public interface IRefClosure<TContext> {
        ref TContext RefContext { get; }
    }
    
    public static class ClosureFactory {
        public static TClosure Create<TContext, TDelegate, TClosure>(TContext context, TDelegate @delegate)
            where TClosure : struct, IClosure<TContext, TDelegate> where TDelegate : Delegate 
            => new TClosure() {
                Delegate = @delegate,
                Context = context
            };
    }
    
    /// <summary>
    /// Provides factory methods for creating closure structs that encapsulate context and delegate logic.
    /// </summary>
    public partial struct Closure {
        /// <summary> Creates an <see cref="ClosureAction{TContext}"/> with the specified context and action. </summary>
        public static ClosureAction<TContext> Action<TContext>(TContext context, Action<TContext> action) =>
            ClosureFactory.Create<TContext, Action<TContext>, ClosureAction<TContext>>(context, action);
    
        /// <summary> Creates an <see cref="ClosureAction{TContext, TArg}"/> with the specified context and action. </summary>
        public static ClosureAction<TContext, TArg> Action<TContext, TArg>(TContext context, Action<TContext, TArg> action) =>
            ClosureFactory.Create<TContext, Action<TContext, TArg>, ClosureAction<TContext, TArg>>(context, action);

        /// <summary> Creates a <see cref="ClosureRefAction{TContext, TArg}"/> with the specified context and action. </summary>
        public static ClosureRefAction<TContext, TArg> RefAction<TContext, TArg>(TContext context, RefActionWithNormalContext<TContext, TArg> action) =>
            ClosureFactory.Create<TContext, RefActionWithNormalContext<TContext, TArg>, ClosureRefAction<TContext, TArg>>(context, action);
        
        
        /// <summary> Creates a <see cref="ClosureFunc{TContext, TResult}"/> with the specified context and function. </summary>
        public static ClosureFunc<TContext, TResult> Func<TContext, TResult>(TContext context, Func<TContext, TResult> func) =>
            ClosureFactory.Create<TContext, Func<TContext, TResult>, ClosureFunc<TContext, TResult>>(context, func);

        /// <summary> Creates a <see cref="ClosureFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
        public static ClosureFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, Func<TContext, TArg, TResult> func) =>
            ClosureFactory.Create<TContext, Func<TContext, TArg, TResult>, ClosureFunc<TContext, TArg, TResult>>(context, func);
    
        /// <summary> Creates a <see cref="ClosureRefFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
        public static ClosureRefFunc<TContext, TArg, TResult> RefFunc<TContext, TArg, TResult>(TContext context, RefFuncWithNormalContext<TContext, TArg, TResult> func) =>
            ClosureFactory.Create<TContext, RefFuncWithNormalContext<TContext, TArg, TResult>, ClosureRefFunc<TContext, TArg, TResult>>(context, func);
    }

    public struct MutatingClosure {
        /// <summary> Creates an <see cref="MutatingClosureAction{TContext}"/> with the specified context and action. </summary>
        public static MutatingClosureAction<TContext> Action<TContext>(TContext context, RefAction<TContext> action) =>
            ClosureFactory.Create<TContext, RefAction<TContext>, MutatingClosureAction<TContext>>(context, action);
    
        /// <summary> Creates an <see cref="MutatingClosureAction{TContext, TArg}"/> with the specified context and action. </summary>
        public static MutatingClosureAction<TContext, TArg> Action<TContext, TArg>(TContext context, ActionWithRefContext<TContext, TArg> action) =>
            ClosureFactory.Create<TContext, ActionWithRefContext<TContext, TArg>, MutatingClosureAction<TContext, TArg>>(context, action);
    
        /// <summary> Creates a <see cref="MutatingClosureRefAction{TContext, TArg}"/> with the specified context and action. </summary>
        public static MutatingClosureRefAction<TContext, TArg> RefAction<TContext, TArg>(TContext context, RefAction<TContext, TArg> action) =>
            ClosureFactory.Create<TContext, RefAction<TContext, TArg>, MutatingClosureRefAction<TContext, TArg>>(context, action);
        
        
        /// <summary> Creates a <see cref="MutatingClosureFunc{TContext, TResult}"/> with the specified context and function. </summary>
        public static MutatingClosureFunc<TContext, TResult> Func<TContext, TResult>(TContext context, RefFunc<TContext, TResult> func) =>
            ClosureFactory.Create<TContext, RefFunc<TContext, TResult>, MutatingClosureFunc<TContext, TResult>>(context, func);

        /// <summary> Creates a <see cref="MutatingClosureFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
        public static MutatingClosureFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(TContext context, FuncWithRefContext<TContext, TArg, TResult> func) =>
            ClosureFactory.Create<TContext, FuncWithRefContext<TContext, TArg, TResult>, MutatingClosureFunc<TContext, TArg, TResult>>(context, func);

        /// <summary> Creates a <see cref="MutatingClosureRefFunc{TContext, TArg, TResult}"/> with the specified context and function. </summary>
        public static MutatingClosureRefFunc<TContext, TArg, TResult> RefFunc<TContext, TArg, TResult>(TContext context, RefFunc<TContext, TArg, TResult> func) =>
            ClosureFactory.Create<TContext, RefFunc<TContext, TArg, TResult>, MutatingClosureRefFunc<TContext, TArg, TResult>>(context, func);
    }
    
    public struct RefClosure {
        /// <summary> Creates a <see cref="RefClosureAction{TContext}"/> with the specified ref context and action. </summary>
        public static RefClosureAction<TContext> Action<TContext>(ref TContext context, RefAction<TContext> action) =>
            new RefClosureAction<TContext>(ref context, action);

        /// <summary> Creates a <see cref="RefClosureAction{TContext, TArg}"/> with the specified ref context and action. </summary>
        public static RefClosureAction<TContext, TArg> Action<TContext, TArg>(ref TContext context, ActionWithRefContext<TContext, TArg> action) =>
            new RefClosureAction<TContext, TArg>(ref context, action);

        /// <summary> Creates a <see cref="RefClosureRefAction{TContext, TArg}"/> with the specified ref context and action. </summary>
        public static RefClosureRefAction<TContext, TArg> RefAction<TContext, TArg>(ref TContext context, RefAction<TContext, TArg> action) =>
            new RefClosureRefAction<TContext, TArg>(ref context, action);
        
        
        /// <summary> Creates a <see cref="RefClosureFunc{TContext, TResult}"/> with the specified ref context and function. </summary>
        public static RefClosureFunc<TContext, TResult> Func<TContext, TResult>(ref TContext context, RefFunc<TContext, TResult> func) =>
            new RefClosureFunc<TContext, TResult>(ref context, func);

        /// <summary> Creates a <see cref="RefClosureFunc{TContext, TArg, TResult}"/> with the specified ref context and function. </summary>
        public static RefClosureFunc<TContext, TArg, TResult> Func<TContext, TArg, TResult>(ref TContext context, FuncWithRefContext<TContext, TArg, TResult> func) =>
            new RefClosureFunc<TContext, TArg, TResult>(ref context, func);

        /// <summary> Creates a <see cref="RefClosureRefFunc{TContext, TArg, TResult}"/> with the specified ref context and ref argument function. </summary>
        public static RefClosureRefFunc<TContext, TArg, TResult> RefFunc<TContext, TArg, TResult>(ref TContext context, RefFunc<TContext, TArg, TResult> func) =>
            new RefClosureRefFunc<TContext, TArg, TResult>(ref context, func);
    }

    public static class ClosureManager {
        public static event Action? OnCacheClear;
        
        public static void ClearCache() {
            OnCacheClear?.Invoke();
        }
    }
    
    public static class ClosureExtensions {
        public static Action AsAction<TContext>(this ClosureAction<TContext> closure) => ClosureToAction<TContext>.AsAction(closure);
        public static Action<TArg> AsAction<TContext, TArg>(this ClosureAction<TContext, TArg> closure) => ClosureToAction<TContext, TArg>.AsAction(closure);
        public static Func<TResult> AsFunc<TContext, TResult>(this ClosureFunc<TContext, TResult> closure) => ClosureToFunc<TContext, TResult>.AsFunc(closure);
        public static Func<TArg, TResult> AsFunc<TContext, TArg, TResult>(this ClosureFunc<TContext, TArg, TResult> closure) => ClosureToFunc<TContext, TArg, TResult>.AsFunc(closure);
    }
    
    // ReSharper disable ConvertClosureToMethodGroup
    internal static class ClosureToAction<TContext> {
        static readonly ConcurrentDictionary<ClosureAction<TContext>, Action> s_closureToActionMap = new ();
        static ClosureToAction() => ClosureManager.OnCacheClear += () => s_closureToActionMap.Clear();
        public static Action AsAction(ClosureAction<TContext> closure) => s_closureToActionMap.GetOrAdd(closure, c => () => c.Invoke());
    }
    
    internal static class ClosureToAction<TContext, TArg> {
        static readonly ConcurrentDictionary<ClosureAction<TContext, TArg>, Action<TArg>> s_closureToActionMap = new ();
        static ClosureToAction() => ClosureManager.OnCacheClear += () => s_closureToActionMap.Clear();
        public static Action<TArg> AsAction(ClosureAction<TContext, TArg> closure) => s_closureToActionMap.GetOrAdd(closure, c => arg => c.Invoke(arg));
    }
    
    internal static class ClosureToFunc<TContext, TResult> {
        static readonly ConcurrentDictionary<ClosureFunc<TContext, TResult>, Func<TResult>> s_closureToFuncMap = new ();
        static ClosureToFunc() => ClosureManager.OnCacheClear += () => s_closureToFuncMap.Clear();
        public static Func<TResult> AsFunc(ClosureFunc<TContext, TResult> closure) => s_closureToFuncMap.GetOrAdd(closure, c => () => c.Invoke());
    }
    
    internal static class ClosureToFunc<TContext, TArg, TResult> {
        static readonly ConcurrentDictionary<ClosureFunc<TContext, TArg, TResult>, Func<TArg, TResult>> s_closureToFuncMap = new ();
        static ClosureToFunc() => ClosureManager.OnCacheClear += () => s_closureToFuncMap.Clear();
        public static Func<TArg, TResult> AsFunc(ClosureFunc<TContext, TArg, TResult> closure) => s_closureToFuncMap.GetOrAdd(closure, c => arg => c.Invoke(arg));
    }
    // ReSharper restore ConvertClosureToMethodGroup
}
