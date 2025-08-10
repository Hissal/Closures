namespace Closures.Anonymous;

public partial record struct AnonymousClosure {
    /// <summary>
    /// Creates a new anonymous closure of the specified type with the given context, delegate, and mutating behaviour.
    /// </summary>
    /// <typeparam name="TClosure">The closure type to create. Must implement <see cref="IAnonymousClosure"/>.</typeparam>
    /// <param name="context">The context to capture.</param>
    /// <param name="delegate">The delegate to encapsulate.</param>
    /// <param name="mutatingBehaviour">Specifies whether the context should be mutated or reset after invocation.</param>
    /// <returns>A new closure of type <typeparamref name="TClosure"/>.</returns>
    public static TClosure Create<TClosure>(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset)
        where TClosure : struct, IAnonymousClosure {
        return new TClosure {
            Context = context,
            Delegate = @delegate,
            MutatingBehaviour = mutatingBehaviour,
        };
    }

    /// <summary>
    /// Creates a new <see cref="AnonymousClosure"/> with the given context, delegate, and mutating behaviour.
    /// </summary>
    /// <param name="context">The context to capture.</param>
    /// <param name="delegate">The delegate to encapsulate.</param>
    /// <param name="mutatingBehaviour">Specifies whether the context should be mutated or reset after invocation.</param>
    /// <returns>A new <see cref="AnonymousClosure"/> instance.</returns>
    public static AnonymousClosure Create(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) => 
        Create<AnonymousClosure>(context, @delegate, mutatingBehaviour);
    
    /// <summary>
    /// Creates a new <see cref="AnonymousClosure"/> with the given context, strongly-typed delegate, and mutating behaviour.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <param name="context">The context to capture.</param>
    /// <param name="delegate">The delegate to encapsulate.</param>
    /// <param name="mutatingBehaviour">Specifies whether the context should be mutated or reset after invocation.</param>
    /// <returns>A new <see cref="AnonymousClosure"/> instance.</returns>
    public static AnonymousClosure Create<TDelegate>(AnonymousValue context, TDelegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset)
        where TDelegate : Delegate => 
        Create<AnonymousClosure>(context, @delegate, mutatingBehaviour);

    /// <summary>
    /// Creates an <see cref="AnonymousClosureAction"/> for a parameterless action delegate.
    /// </summary>
    /// <param name="context">The context to capture.</param>
    /// <param name="delegate">The action delegate (must not have a return value or argument).</param>
    /// <param name="mutatingBehaviour">Specifies whether the context should be mutated or reset after invocation.</param>
    /// <returns>An <see cref="AnonymousClosureAction"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the delegate does not match the required signature.</exception>
    public static AnonymousClosureAction Action(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) {
        if (!AnonymousHelper.IsAction(@delegate))
            throw new ArgumentException("Delegate must not have a return value.", nameof(@delegate));
        
        if (AnonymousHelper.HasArg(@delegate))
            throw new ArgumentException("Delegate must not have an argument.", nameof(@delegate));

        return Create<AnonymousClosureAction>(context, @delegate, mutatingBehaviour);
    }
    /// <summary>
    /// Creates an <see cref="AnonymousClosureAction{TArg}"/> for an action delegate with a single argument.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument.</typeparam>
    /// <param name="context">The context to capture.</param>
    /// <param name="delegate">The action delegate (must not have a return value and must have an argument of type <typeparamref name="TArg"/>).</param>
    /// <param name="mutatingBehaviour">Specifies whether the context should be mutated or reset after invocation.</param>
    /// <returns>An <see cref="AnonymousClosureAction{TArg}"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the delegate does not match the required signature.</exception>
    public static AnonymousClosureAction<TArg> Action<TArg>(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) {
        if (!AnonymousHelper.IsAction(@delegate))
            throw new ArgumentException("Delegate must not have a return value.", nameof(@delegate));
        
        if (!AnonymousHelper.HasArgOfType<TArg>(@delegate))
            throw new ArgumentException($"Delegate must have an argument of type {typeof(TArg).Name}.", nameof(@delegate));
        
        return Create<AnonymousClosureAction<TArg>>(context, @delegate, mutatingBehaviour);
    }
    
    /// <summary>
    /// Creates an <see cref="AnonymousClosureFunc{TReturn}"/> for a parameterless function delegate.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the function.</typeparam>
    /// <param name="context">The context to capture.</param>
    /// <param name="delegate">The function delegate (must have a return value and no argument).</param>
    /// <param name="mutatingBehaviour">Specifies whether the context should be mutated or reset after invocation.</param>
    /// <returns>An <see cref="AnonymousClosureFunc{TReturn}"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the delegate does not match the required signature.</exception>
    public static AnonymousClosureFunc<TReturn> Func<TReturn>(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) {
        if (!AnonymousHelper.IsFunc(@delegate))
            throw new ArgumentException("Delegate must have a return value.", nameof(@delegate));
        
        if (AnonymousHelper.HasArg(@delegate))
            throw new ArgumentException("Delegate must not have an argument.", nameof(@delegate));
        
        return Create<AnonymousClosureFunc<TReturn>>(context, @delegate, mutatingBehaviour);
    }
    /// <summary>
    /// Creates an <see cref="AnonymousClosureFunc{TArg, TReturn}"/> for a function delegate with a single argument.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument.</typeparam>
    /// <typeparam name="TReturn">The return type of the function.</typeparam>
    /// <param name="context">The context to capture.</param>
    /// <param name="delegate">The function delegate (must have a return value and an argument of type <typeparamref name="TArg"/>).</param>
    /// <param name="mutatingBehaviour">Specifies whether the context should be mutated or reset after invocation.</param>
    /// <returns>An <see cref="AnonymousClosureFunc{TArg, TReturn}"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the delegate does not match the required signature.</exception>
    public static AnonymousClosureFunc<TArg, TReturn> Func<TArg, TReturn>(AnonymousValue context, Delegate @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Reset) {
        if (!AnonymousHelper.IsFunc(@delegate))
            throw new ArgumentException("Delegate must have a return value.", nameof(@delegate));
        
        if (!AnonymousHelper.HasArgOfType<TArg>(@delegate))
            throw new ArgumentException($"Delegate must have an argument of type {typeof(TArg).Name}.", nameof(@delegate));
        
        return Create<AnonymousClosureFunc<TArg, TReturn>>(context, @delegate, mutatingBehaviour);
    }

    // Targeted creation methods for specific delegate types
    
    /// <summary> Creates an <see cref="AnonymousClosureAction"/> using an <see cref="System.Action{TContext}"/>. <br></br>
    /// Represents a <see cref="ClosureAction{TContext}"/></summary>
    public static AnonymousClosureAction Action<TContext>(TContext context, Action<TContext> @delegate) where TContext : notnull =>
        Create<AnonymousClosureAction>(AnonymousValue.From(context), @delegate);
    /// <summary> Creates an <see cref="AnonymousClosureAction"/> using a <see cref="RefAction{TContext}"/>.<br></br>
    /// Represents a <see cref="MutatingClosureAction{TContext}"/> </summary>
    public static AnonymousClosureAction Action<TContext>(TContext context, RefAction<TContext> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull =>
        Create<AnonymousClosureAction>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    
    /// <summary> Creates an <see cref="AnonymousClosureAction"/> using a <see cref="System.Action{TContext, TArg}"/>.<br></br>
    /// Represents a <see cref="ClosureAction{TContext, TArg}"/></summary>
    public static AnonymousClosureAction<TArg> Action<TContext, TArg>(TContext context, Action<TContext, TArg> @delegate) where TContext : notnull => 
        Create<AnonymousClosureAction<TArg>>(AnonymousValue.From(context), @delegate);
    /// <summary> Creates an <see cref="AnonymousClosureAction"/> using a <see cref="RefActionWithNormalContext{TContext, TArg}"/>.<br></br>
    /// Represents a <see cref="ClosureRefAction{TContext,TArg}"/></summary>
    public static AnonymousClosureAction<TArg> Action<TContext, TArg>(TContext context, RefActionWithNormalContext<TContext, TArg> @delegate) where TContext : notnull => 
        Create<AnonymousClosureAction<TArg>>(AnonymousValue.From(context), @delegate);
    /// <summary> Creates an <see cref="AnonymousClosureAction"/> using a <see cref="ActionWithRefContext{TContext, TArg}"/>.<br></br>
    /// Represents a <see cref="MutatingClosureAction{TContext, TArg}"/></summary>
    public static AnonymousClosureAction<TArg> Action<TContext, TArg>(TContext context, ActionWithRefContext<TContext, TArg> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureAction<TArg>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    /// <summary> Creates an <see cref="AnonymousClosureAction"/> using a <see cref="RefAction{TContext, TArg}"/>.<br></br>
    /// Represents a <see cref="MutatingClosureRefAction{TContext,TArg}"/></summary>
    public static AnonymousClosureAction<TArg> Action<TContext, TArg>(TContext context, RefAction<TContext, TArg> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureAction<TArg>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    
    /// <summary> Creates an <see cref="AnonymousClosureFunc{TReturn}"/> using a <see cref="System.Func{TContext, TReturn}"/>.<br></br>
    /// Represents a <see cref="ClosureFunc{TContext,TResult}"/></summary>
    public static AnonymousClosureFunc<TReturn> Func<TContext, TReturn>(TContext context, Func<TContext, TReturn> @delegate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TReturn>>(AnonymousValue.From(context), @delegate);
    /// <summary> Creates an <see cref="AnonymousClosureFunc{TReturn}"/> using a <see cref="RefFunc{TContext, TReturn}"/>.<br></br>
    /// Represents a <see cref="MutatingClosureFunc{TContext,TResult}"/></summary>
    public static AnonymousClosureFunc<TReturn> Func<TContext, TReturn>(TContext context, RefFunc<TContext, TReturn> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TReturn>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    
    /// <summary> Creates an <see cref="AnonymousClosureFunc{TArg, TReturn}"/> using a <see cref="System.Func{TContext, TArg, TReturn}"/>. <br></br>
    /// Represents a <see cref="ClosureFunc{TContext,TArg,TResult}"/></summary>
    public static AnonymousClosureFunc<TArg, TReturn> Func<TContext, TArg, TReturn>(TContext context, Func<TContext, TArg, TReturn> @delegate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TArg, TReturn>>(AnonymousValue.From(context), @delegate);
    /// <summary> Creates an <see cref="AnonymousClosureFunc{TArg, TReturn}"/> using a <see cref="RefFuncWithNormalContext{TContext, TArg, TReturn}"/>.<br></br>
    /// Represents a <see cref="ClosureRefFunc{TContext,TArg,TResult}"/></summary>
    public static AnonymousClosureFunc<TArg, TReturn> Func<TContext, TArg, TReturn>(TContext context, RefFuncWithNormalContext<TContext, TArg, TReturn> @delegate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TArg, TReturn>>(AnonymousValue.From(context), @delegate);
    /// <summary> Creates an <see cref="AnonymousClosureFunc{TArg, TReturn}"/> using a <see cref="ActionWithRefContext{TContext, TArg, TReturn}"/>. <br></br>
    /// Represents a <see cref="MutatingClosureFunc{TContext,TArg,TResult}"/></summary>
    public static AnonymousClosureFunc<TArg, TReturn> Func<TContext, TArg, TReturn>(TContext context, FuncWithRefContext<TContext, TArg, TReturn> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TArg, TReturn>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
    /// <summary> Creates an <see cref="AnonymousClosureFunc{TArg, TReturn}"/> using a <see cref="RefFunc{TContext, TArg, TReturn}"/>.<br></br>
    /// Represents a <see cref="MutatingClosureRefFunc{TContext,TArg,TResult}"/></summary>
    public static AnonymousClosureFunc<TArg, TReturn> Func<TContext, TArg, TReturn>(TContext context, RefFunc<TContext, TArg, TReturn> @delegate, MutatingBehaviour mutatingBehaviour = MutatingBehaviour.Mutate) where TContext : notnull => 
        Create<AnonymousClosureFunc<TArg, TReturn>>(AnonymousValue.From(context), @delegate, mutatingBehaviour);
}