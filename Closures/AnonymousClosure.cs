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
}

/// <summary>
/// An anonymous closure must be manually invoked using the Delegate and Context
/// </summary>
public struct AnonymousClosure<TContext, TDelegate> : IClosure<TContext, TDelegate> 
    where TDelegate : Delegate {
    public TDelegate Delegate { get; set; }
    public TContext Context { get; set; }
    
    public AnonymousClosure(TContext context, TDelegate @delegate) {
        Context = context;
        Delegate = @delegate;
        
        var closure = Closure.Action(10, (int ctx) => { /* Do something with ctx */ });
        var anonymous = closure.ToAnonymousClosure();
        anonymous.DynamicInvokeWithContext();
    }

    public void Add(TDelegate action) => Delegate = (TDelegate)System.Delegate.Combine(Delegate, action);
    public void Remove(TDelegate action) => Delegate = (TDelegate)System.Delegate.Remove(Delegate, action);
}

public static class AnonymousClosureExtensions {
    /// <summary>
    /// Creates an anonymous closure from an existing closure.
    /// </summary>
    /// <param name="closure"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TDelegate"></typeparam>
    /// <returns></returns>
    public static AnonymousClosure<TContext, TDelegate> ToAnonymousClosure<TContext, TDelegate>(this IClosure<TContext, TDelegate> closure) 
        where TDelegate : Delegate 
        => Closure.Anonymous(closure.Context, closure.Delegate);

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