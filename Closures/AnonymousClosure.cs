using Lh.Closures.Reflection.Experimental;

namespace Lh.Closures;

public partial struct Closure {
    /// <summary>
    /// Creates an anonymous closure that encapsulates a delegate and a context.
    /// </summary>
    /// <param name="context">The context to be captured.</param>
    /// <param name="delegate">The delegate to be captured.</param>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <typeparam name="TDelegate">The type of the delegate. Must inherit from <see cref="Delegate"/>.</typeparam>
    /// <returns>An <see cref="AnonymousClosure{TContext, TDelegate}"/> containing the provided context and delegate.</returns>
    public static AnonymousClosure<TContext, TDelegate> Anonymous<TContext, TDelegate>(TContext context, TDelegate @delegate) 
        where TDelegate : Delegate 
        => Create<TContext, TDelegate, AnonymousClosure<TContext, TDelegate>>(context, @delegate);
}

public interface IAnonymousClosure : IClosure {
    
}
public interface IAnonymousClosure<TContext> : IClosure<TContext>, IAnonymousClosure {

}
public interface IAnonymousClosure<TContext, TDelegate> : IClosure<TContext, TDelegate>, IAnonymousClosure<TContext> where TDelegate : Delegate {

}

/// <summary>
/// An anonymous closure must be manually invoked using the Delegate and Context
/// </summary>
public struct AnonymousClosure<TContext, TDelegate> : IAnonymousClosure<TContext, TDelegate>
    where TDelegate : Delegate {
    public TDelegate Delegate { get; set; }
    public TContext Context { get; set; }
    
    public AnonymousClosure(TContext context, TDelegate @delegate) {
        Context = context;
        Delegate = @delegate;
    }

    public void Add(TDelegate action) => Delegate = (TDelegate)System.Delegate.Combine(Delegate, action);
    public void Remove(TDelegate action) => Delegate = (TDelegate)System.Delegate.Remove(Delegate, action);
}

public static class AnonymousClosureExtensions {
    /// <summary>
    /// Converts an existing closure to an anonymous closure.
    /// </summary>
    /// <param name="closure">The closure to make anonymous.</param>
    /// <typeparam name="TContext">The type of context used.</typeparam>
    /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
    /// <returns>An <see cref="AnonymousClosure{TContext,TDelegate}"/></returns>
    /// <remarks>
    /// This will box the closure as it has to convert to <see cref="IClosure{TContext, TDelegate}"/> from a value type.<br></br>
    /// Specify the type of closure you are converting to avoid this <see cref="AsAnonymous{TContext,TDelegate,TClosure}"/>
    /// </remarks>
    public static AnonymousClosure<TContext, TDelegate> ToAnonymous<TContext, TDelegate>(this IClosure<TContext, TDelegate> closure) 
        where TDelegate : Delegate 
        => ClosureConverter.ConvertToAnonymous(closure);

    /// <summary>
    /// Converts an existing closure to an anonymous closure.
    /// </summary>
    /// <param name="closure">The closure to make anonymous.</param>
    /// <typeparam name="TContext">The type of context used.</typeparam>
    /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
    /// <typeparam name="TClosure">The type of closure to convert.</typeparam>
    /// <returns>An <see cref="AnonymousClosure{TContext,TDelegate}"/></returns>
    public static AnonymousClosure<TContext, TDelegate> AsAnonymous<TContext, TDelegate, TClosure>(this TClosure closure) 
        where TDelegate : Delegate 
        where TClosure :  IClosure<TContext, TDelegate>
        => ClosureConverter.ConvertToAnonymous<TContext, TDelegate, TClosure>(closure);

    /// <summary>
    /// Creates a closure of a specified type from an anonymous closure.
    /// </summary>
    /// <param name="closure">The anonymous closure to convert.</param>
    /// <typeparam name="TContext">The type of context used.</typeparam>
    /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
    /// <typeparam name="TClosure">The type of closure to convert to.</typeparam>
    /// <returns>A closure of the specified type</returns>
    public static TClosure AsKnown<TContext, TDelegate, TClosure>(this AnonymousClosure<TContext, TDelegate> closure) 
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
    public static void DynamicInvokeWithContext<TContext, TDelegate>(this AnonymousClosure<TContext, TDelegate> closure) 
        where TDelegate : Delegate {
        closure.Delegate.DynamicInvoke(closure.Context);
    }
    
    /// <summary>
    /// Invokes the delegate with the context. The context is passed as the first argument followed by the provided argument.
    /// </summary>
    /// <param name="closure">The anonymous closure to invoke.</param>
    /// <remarks>
    /// <b>Warning:</b> Calls <see cref="System.Delegate.DynamicInvoke"/> to perform the operation
    /// creating an array of objects and possibly boxing the context and argument.
    /// </remarks>
    public static void DynamicInvokeWithContext<TContext, TDelegate, TArg>(this AnonymousClosure<TContext, TDelegate> closure, TArg arg) 
        where TDelegate : Delegate {
        closure.Delegate.DynamicInvoke(closure.Context, arg);
    }
    
    /// <summary>
    /// Invokes the delegate with the context. The context is passed as the first argument followed by the provided arguments.
    /// </summary>
    /// <param name="closure">The anonymous closure to invoke.</param>
    /// <remarks>
    /// <b>Warning:</b> Calls <see cref="System.Delegate.DynamicInvoke"/> to perform the operation
    /// creating an array of objects and possibly boxing the context and arguments.
    /// </remarks>
    public static void DynamicInvokeWithContext<TContext, TDelegate, TArg1, TArg2>(this AnonymousClosure<TContext, TDelegate> closure, TArg1 arg1, TArg2 arg2) 
        where TDelegate : Delegate {
        closure.Delegate.DynamicInvoke(closure.Context, arg1, arg2);
    }
}