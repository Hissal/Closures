// ReSharper disable ConvertToPrimaryConstructor

namespace Closures;

public partial struct Closure {
    /// <summary>
    /// Creates a custom closure that encapsulates a delegate and a context.
    /// </summary>
    /// <param name="context">The context to be captured.</param>
    /// <param name="delegate">The delegate to be captured.</param>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <typeparam name="TDelegate">The type of the delegate. Must inherit from <see cref="Delegate"/>.</typeparam>
    /// <returns>A <see cref="CustomClosure{TContext,TDelegate}"/> containing the provided context and delegate.</returns>
    public static CustomClosure<TContext, TDelegate> Custom<TContext, TDelegate>(TContext context,
        TDelegate @delegate)
        where TDelegate : Delegate
        => ClosureFactory.Create<TContext, TDelegate, CustomClosure<TContext, TDelegate>>(context, @delegate);
}

public interface ICustomClosure {
        
}
    
/// <summary>
/// Represents a custom closure that encapsulates a <see cref="Delegate"/> and a context.
/// <br></br> <br></br>
/// <b>Note</b> The <see cref="Delegate"/> must be manually invoked using it directly and passing the context as the first argument.
/// </summary>
public readonly struct CustomClosure<TContext, TDelegate> : IClosure<TContext, TDelegate>, ICustomClosure where TDelegate : Delegate {
    public TDelegate Delegate { get; init; }
    public TContext Context { get; init; }
}