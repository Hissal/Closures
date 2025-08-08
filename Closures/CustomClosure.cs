using System.Net;
using System.Runtime.CompilerServices;
using Closures.Converting;

#pragma warning disable CS8601 // Possible null reference assignment.
// ReSharper disable ConvertToPrimaryConstructor

namespace Closures {
    public partial struct Closure {
        /// <summary>
        /// Creates an anonymous closure that encapsulates a delegate and a context.
        /// </summary>
        /// <param name="context">The context to be captured.</param>
        /// <param name="delegate">The delegate to be captured.</param>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate. Must inherit from <see cref="Delegate"/>.</typeparam>
        /// <returns>An <see cref="CustomClosure{TContext,TDelegate}"/> containing the provided context and delegate.</returns>
        public static CustomClosure<TContext, TDelegate> Custom<TContext, TDelegate>(TContext context,
            TDelegate @delegate)
            where TDelegate : Delegate
            => Create<TContext, TDelegate, CustomClosure<TContext, TDelegate>>(context, @delegate);
    }

    public interface ICustomClosure : IClosure { }

    public interface ICustomClosure<TContext> : IClosure<TContext>, ICustomClosure { }

    public interface ICustomClosure<TContext, TDelegate> : IClosure<TContext, TDelegate>, ICustomClosure<TContext>
        where TDelegate : Delegate { }

    /// <summary>
    /// An anonymous closure is a closure for delegates that do not have a known type. Meaning they can be used with any delegate type.
    /// <br></br> <br></br>
    /// <b>Note</b> The <see cref="Delegate"/> must be manually invoked using it directly and passing the context as the first argument.
    /// </summary>
    public struct CustomClosure<TContext, TDelegate> : ICustomClosure<TContext, TDelegate>
        where TDelegate : Delegate {
        public TDelegate Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;

        public TContext Context { get; set; }

        public CustomClosure(TContext context, TDelegate @delegate) {
            Context = context;
            Delegate = @delegate;
        }

        public void Add(TDelegate @delegate) => Delegate = (TDelegate)System.Delegate.Combine(Delegate, @delegate);
        public void Remove(TDelegate @delegate) => Delegate = (TDelegate)System.Delegate.Remove(Delegate, @delegate)!;
    }

    public static class CustomClosureExtensions {
        /// <summary>
        /// Converts an existing closure to an anonymous closure.
        /// </summary>
        /// <param name="closure">The closure to make anonymous.</param>
        /// <typeparam name="TContext">The type of context used.</typeparam>
        /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
        /// <returns>An <see cref="CustomClosure{TContext,TDelegate}"/></returns>
        /// <remarks>
        /// This will box the closure as it has to convert to <see cref="IClosure{TContext, TDelegate}"/> from a value type.<br></br>
        /// Specify the type of closure you are converting to avoid this <see cref="AsCustom{TContext,TDelegate,TClosure}"/>
        /// </remarks>
        public static CustomClosure<TContext, TDelegate> ToCustom<TContext, TDelegate>(
            this IClosure<TContext, TDelegate> closure)
            where TDelegate : Delegate
            => ClosureConverter.ConvertToAnonymous(closure);

        /// <summary>
        /// Converts an existing closure to an anonymous closure.
        /// </summary>
        /// <param name="closure">The closure to make anonymous.</param>
        /// <typeparam name="TContext">The type of context used.</typeparam>
        /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
        /// <typeparam name="TClosure">The type of closure to convert.</typeparam>
        /// <returns>An <see cref="CustomClosure{TContext,TDelegate}"/></returns>
        public static CustomClosure<TContext, TDelegate> AsCustom<TContext, TDelegate, TClosure>(
            this TClosure closure)
            where TDelegate : Delegate
            where TClosure : IClosure<TContext, TDelegate>
            => ClosureConverter.ConvertToAnonymous<TContext, TDelegate, TClosure>(closure);

        /// <summary>
        /// Creates a closure of a specified type from an anonymous closure.
        /// </summary>
        /// <param name="closure">The anonymous closure to convert.</param>
        /// <typeparam name="TContext">The type of context used.</typeparam>
        /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
        /// <typeparam name="TClosure">The type of closure to convert to.</typeparam>
        /// <returns>A closure of the specified type</returns>
        public static TClosure AsKnown<TContext, TDelegate, TClosure>(this CustomClosure<TContext, TDelegate> closure)
            where TClosure : struct, IClosure<TContext, TDelegate>
            where TDelegate : Delegate
            => ClosureConverter.Convert<TContext, TDelegate, TClosure>(closure);

        /// <summary>
        /// Invokes the delegate with the context.
        /// </summary>
        /// <param name="closure">The anonymous closure to invoke.</param>
        /// <remarks>
        /// <b>Warning:</b> Calls <see cref="System.Delegate.DynamicInvoke"/> to perform the operation
        /// creating an array of objects and possibly boxing the context.
        /// </remarks>
        public static void DynamicInvokeWithContext<TContext, TDelegate>(
            this CustomClosure<TContext, TDelegate> closure)
            where TDelegate : Delegate {
            closure.Delegate.DynamicInvoke(closure.Context);
        }

        /// <summary>
        /// Invokes the delegate with the context. The context is passed as the first argument followed by the provided argument.
        /// </summary>
        /// <param name="closure">The anonymous closure to invoke.</param>
        /// <param name="arg">Argument to invoke with</param>
        /// <remarks>
        /// <b>Warning:</b> Calls <see cref="System.Delegate.DynamicInvoke"/> to perform the operation
        /// creating an array of objects and possibly boxing the context and argument.
        /// </remarks>
        public static void DynamicInvokeWithContext<TContext, TDelegate, TArg>(
            this CustomClosure<TContext, TDelegate> closure, TArg arg)
            where TDelegate : Delegate {
            closure.Delegate.DynamicInvoke(closure.Context, arg);
        }

        /// <summary>
        /// Invokes the delegate with the context. The context is passed as the first argument followed by the provided arguments.
        /// </summary>
        /// <param name="closure">The anonymous closure to invoke.</param>
        /// <param name="arg1">First argument</param>
        /// <param name="arg2">Second argument</param>
        /// <remarks>
        /// <b>Warning:</b> Calls <see cref="System.Delegate.DynamicInvoke"/> to perform the operation
        /// creating an array of objects and possibly boxing the context and arguments.
        /// </remarks>
        public static void DynamicInvokeWithContext<TContext, TDelegate, TArg1, TArg2>(
            this CustomClosure<TContext, TDelegate> closure, TArg1 arg1, TArg2 arg2)
            where TDelegate : Delegate {
            closure.Delegate.DynamicInvoke(closure.Context, arg1, arg2);
        }

        /// <summary>
        /// Invokes the delegate with the context. The context is passed as the first argument followed by the provided arguments.
        /// </summary>
        /// <param name="closure">The anonymous closure to invoke.</param>
        /// <param name="arg1">First argument</param>
        /// <param name="arg2">Second argument</param>
        /// <param name="arg3">Third argument</param>
        /// <remarks>
        /// <b>Warning:</b> Calls <see cref="System.Delegate.DynamicInvoke"/> to perform the operation
        /// creating an array of objects and possibly boxing the context and arguments.
        /// </remarks>
        public static void DynamicInvokeWithContext<TContext, TDelegate, TArg1, TArg2, TArg3>(
            this CustomClosure<TContext, TDelegate> closure, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            where TDelegate : Delegate {
            closure.Delegate.DynamicInvoke(closure.Context, arg1, arg2, arg3);
        }
    }
}