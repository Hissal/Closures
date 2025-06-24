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
        => new AnonymousClosure<TContext, TDelegate> {
            Delegate = @delegate,
            Context = context
        };
    
    /// <summary>
    /// Creates an anonymous closure that encapsulates a delegate and a context.
    /// </summary>
    /// <param name="context">The context to be captured.</param>
    /// <param name="delegate">The delegate to be captured.</param>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <returns>An <see cref="AnonymousClosure{TContext}"/> containing the provided context and delegate.</returns>
    public static AnonymousClosure<TContext> Anonymous<TContext>(TContext context, Delegate @delegate) 
        => new AnonymousClosure<TContext> {
            Delegate = @delegate,
            Context = context
        };
}

public interface IAnonymousClosure : IClosure {
    
}

public interface IAnonymousClosure<TContext, TDelegate> : IClosure<TContext, TDelegate>, IAnonymousClosure where TDelegate : Delegate {

}
public interface IAnonymousClosure<TContext> : IAnonymousClosure<TContext, Delegate> {

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

/// <summary>
/// An anonymous closure must be manually invoked using the Delegate and Context
/// </summary>
public struct AnonymousClosure<TContext> : IAnonymousClosure<TContext> {
    public Delegate Delegate { get; set; }
    public TContext Context { get; set; }
    
    public AnonymousClosure(TContext context, Delegate @delegate) {
        Context = context;
        Delegate = @delegate;
    }
    
    public void Add(Delegate action) => Delegate = Delegate.Combine(Delegate, action);
    public void Remove(Delegate action) => Delegate = Delegate.Remove(Delegate, action);
}

public static class AnonymousClosureExtensions {
    /// <summary>
    /// Creates an anonymous closure from an existing closure.
    /// </summary>
    /// <param name="closure">The closure to make anonymous.</param>
    /// <typeparam name="TContext">The type of context used.</typeparam>
    /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
    /// <returns>An <see cref="AnonymousClosure{TContext,TDelegate}"/></returns>
    /// <remarks>This will box the closure as it has to convert to <see cref="IClosure{TContext, TDelegate}"/> from a value type.</remarks>
    public static AnonymousClosure<TContext, TDelegate> ToAnonymousClosure<TContext, TDelegate>(this IClosure<TContext, TDelegate> closure) 
        where TDelegate : Delegate 
        => Closure.Anonymous(closure.Context, closure.Delegate);

    /// <summary>
    /// Creates an anonymous closure from an existing closure.
    /// </summary>
    /// <param name="closure">The closure to make anonymous.</param>
    /// <typeparam name="TContext">The type of context used.</typeparam>
    /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
    /// <typeparam name="TClosure">The type of closure to convert.</typeparam>
    /// <returns>An <see cref="AnonymousClosure{TContext,TDelegate}"/></returns>
    public static AnonymousClosure<TContext, TDelegate> AsAnonymousClosure<TContext, TDelegate, TClosure>(this TClosure closure) 
        where TDelegate : Delegate 
        where TClosure :  IClosure<TContext, TDelegate>
        => Closure.Anonymous(closure.Context, closure.Delegate);

    /// <summary>
    /// Creates a closure of a specific type from an anonymous closure.
    /// </summary>
    /// <param name="closure">The anonymous closure to make typed.</param>
    /// <typeparam name="TContext">The type of context used.</typeparam>
    /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
    /// <typeparam name="TClosure">The type of closure to convert to.</typeparam>
    /// <returns>A closure of the specified type</returns>
    public static TClosure AsTypedClosure<TContext, TDelegate, TClosure>(this AnonymousClosure<TContext, TDelegate> closure) 
        where TClosure : struct, IClosure<TContext, TDelegate>
        where TDelegate : Delegate 
        => Closure.Create<TContext, TDelegate, TClosure>(closure.Context, closure.Delegate);
    
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