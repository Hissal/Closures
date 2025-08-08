using System.Linq.Expressions;

namespace Closures;

/// <summary>
/// Provides dynamic invoker delegates for anonymous closure actions and functions.
/// <br></br>
/// <para>
/// This class generates and caches delegates that can invoke arbitrary closure delegates
/// (with or without ref context/arguments) using <see cref="AnonymousValue"/> as the context container.
/// It supports both action and function delegates, with or without arguments, and handles
/// mutating behaviour for ref context closures.
/// </para>
/// <para>
/// The invokers are generated using expression trees and cached for each delegate type,
/// enabling efficient and type-safe invocation of anonymous closures at runtime.
/// </para>
/// </summary>
public static class AnonymousInvokers {
    // Actions
    public delegate void AnonymousActionInvoker(Delegate @delegate, ref AnonymousValue context, MutatingClosureBehaviour mutatingBehaviour);
    public delegate void AnonymousActionInvoker<TArg>(Delegate @delegate, ref AnonymousValue context, MutatingClosureBehaviour mutatingBehaviour, ref TArg arg);
    
    static readonly Dictionary<Type, AnonymousActionInvoker> s_actionInvokers = new();
    static readonly Dictionary<Type, Delegate> s_argActionInvokers = new();
    
    // Funcs
    public delegate TReturn AnonymousFuncInvoker<out TReturn>(Delegate @delegate, ref AnonymousValue context, MutatingClosureBehaviour mutatingBehaviour);
    public delegate TReturn AnonymousFuncInvoker<TArg, out TReturn>(Delegate @delegate, ref AnonymousValue context, MutatingClosureBehaviour mutatingBehaviour, ref TArg arg);
    
    static readonly Dictionary<Type, Delegate> s_funcInvokers = new();
    static readonly Dictionary<Type, Delegate> s_argFuncInvokers = new();
    
    /// <summary>
    /// Gets an invoker delegate for an anonymous action closure with the specified delegate type.
    /// The invoker will handle both normal and mutating (ref context) actions.
    /// </summary>
    /// <param name="delegate">The closure delegate to invoke.</param>
    /// <returns>An <see cref="AnonymousActionInvoker"/> that can invoke the closure with a context and mutating behaviour.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the delegate type does not have a context type.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the delegate type does not match the expected signature for an anonymous action invoker.
    /// </exception>
    public static AnonymousActionInvoker GetActionInvoker(Delegate @delegate) {
        var delegateType = @delegate.GetType();
        
        if (!s_actionInvokers.TryGetValue(delegateType, out var invoker)) {
            // delegate context and argument types
            var genericArguments = AnonymousHelper.GetGenericArguments(@delegate);
            
            var contextType = genericArguments[0] ??
                              throw new InvalidOperationException(
                                  $"Delegate type '{delegateType.Name}' does not have a context type.");

            // Parameters (AnonymousClosure, ref AnonymousValue)
            var delegateParam = Expression.Parameter(typeof(Delegate), "delegate");
            var contextParam = Expression.Parameter(typeof(AnonymousValue).MakeByRefType(), "context");
            var mutatingBehaviourParam = Expression.Parameter(typeof(MutatingClosureBehaviour), "mutatingBehaviour");

            // Methods
            var asMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.As)) 
                           ?? throw new MissingMethodException($"Method 'As' not found in AnonymousValue.");
            
            var setMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.Set)) 
                            ?? throw new MissingMethodException($"Method 'Set' not found in AnonymousValue.");
            
            var invokeMethod = delegateType.GetMethod("Invoke") 
                               ?? throw new MissingMethodException($"Method 'Invoke' not found in {delegateType}.");

            // Determine if the context should be passed as AnonymousValue or as a specific type
            var contextAsAnonymousValue = contextType == typeof(AnonymousValue);
            Expression contextExpr = contextAsAnonymousValue
                ? contextParam
                : Expression.Call(contextParam, asMethod.MakeGenericMethod(contextType));
            
            // ref parameters
            var isRefContext = invokeMethod.GetParameters()[0].ParameterType.IsByRef;
            
            // Compile non-ref invoker (Normal)
            if (!isRefContext) {
                var invokeExpr = Expression.Call(
                    Expression.TypeAs(delegateParam, delegateType),
                    invokeMethod, 
                    contextExpr
                );
                
                var lambda = 
                    Expression.Lambda<AnonymousActionInvoker>(
                        invokeExpr, delegateParam, contextParam, mutatingBehaviourParam
                    );
                
                invoker = lambda.Compile();
            }
            // Compile ref invoker (Mutating)
            else {
                // Variables
                var castContextVar = Expression.Variable(contextType, "castContext");
                
                // Expressions
                var assignCastContext = Expression.Assign(
                    castContextVar, contextExpr
                );
                
                var castDelegateExpr = Expression.TypeAs(
                    delegateParam, delegateType
                );

                var invokeExpr = Expression.Call(
                    castDelegateExpr, invokeMethod, castContextVar
                );
                
                var ifMutatingSetContext = Expression.IfThen(
                    Expression.Equal(
                        mutatingBehaviourParam,
                        Expression.Constant(MutatingClosureBehaviour.Retain)
                    ),
                    contextAsAnonymousValue
                        ? Expression.Assign(contextParam, castContextVar)
                        : Expression.Call(contextParam, setMethod.MakeGenericMethod(contextType), castContextVar)
                );
                
                var block = Expression.Block(
                    new[] { castContextVar},
                    assignCastContext,
                    invokeExpr,
                    ifMutatingSetContext
                );

                var lambda = 
                    Expression.Lambda<AnonymousActionInvoker>(
                        block, delegateParam, contextParam, mutatingBehaviourParam
                    );
                
                invoker = lambda.Compile();
            }
            
            s_actionInvokers[delegateType] = invoker;
        }

        return invoker;
    }
    
    /// <summary>
    /// Gets an invoker delegate for an anonymous action closure with an argument of type <typeparamref name="TArg"/>.
    /// The invoker will handle both normal and mutating (ref context) actions.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the closure.</typeparam>
    /// <param name="delegate">The closure delegate to invoke.</param>
    /// <returns>An <see cref="AnonymousActionInvoker{TArg}"/> that can invoke the closure with a context, mutating behaviour, and argument.</returns>
    /// <exception cref="InvalidCastException">
    /// Thrown if the delegate argument type does not match <typeparamref name="TArg"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the delegate type does not have a context or argument type.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the delegate type does not match the expected signature for an anonymous action invoker with an argument.
    /// </exception>
    public static AnonymousActionInvoker<TArg> GetActionInvoker<TArg>(Delegate @delegate) {
        var delegateType = @delegate.GetType();
        
        if (!s_argActionInvokers.TryGetValue(delegateType, out var invoker)) {
            // delegate context and argument types
            var genericArguments = delegateType.GetGenericArguments();
            var contextType = genericArguments[0] ??
                              throw new InvalidOperationException(
                                  $"Delegate type '{delegateType.Name}' does not have a context type.");
            var argType = genericArguments[1] ??
                          throw new InvalidOperationException(
                              $"Delegate type '{delegateType.Name}' does not have an argument type.");
            
            // Validate types
            if (argType != typeof(TArg)) {
                throw new InvalidCastException(
                    $"Delegate type '{delegateType.Name}' argument type '{argType.Name}' does not match expected type '{typeof(TArg).Name}'.");
            }

            // Parameters (AnonymousClosure, ref AnonymousValue)
            var delegateParam = Expression.Parameter(typeof(Delegate), "delegate");
            var contextParam = Expression.Parameter(typeof(AnonymousValue).MakeByRefType(), "context");
            var mutatingBehaviourParam = Expression.Parameter(typeof(MutatingClosureBehaviour), "mutatingBehaviour");
            var argParam = Expression.Parameter(typeof(TArg).MakeByRefType(), "arg");
            
            // Methods
            var asMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.As)) 
                           ?? throw new MissingMethodException($"Method 'As' not found in AnonymousValue.");
            
            var setMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.Set)) 
                            ?? throw new MissingMethodException($"Method 'Set' not found in AnonymousValue.");
            
            var invokeMethod = delegateType.GetMethod("Invoke") 
                               ?? throw new MissingMethodException($"Method 'Invoke' not found in {delegateType}.");

            // Determine if the context should be passed as AnonymousValue or as a specific type
            var contextAsAnonymousValue = contextType == typeof(AnonymousValue);
            Expression contextExpr = contextAsAnonymousValue
                ? contextParam
                : Expression.Call(contextParam, asMethod.MakeGenericMethod(contextType));
            
            // ref parameters
            var invokeMethodParameters = invokeMethod.GetParameters();
            var isRefContext = invokeMethodParameters[0].ParameterType.IsByRef;

            // Compile non-ref invoker (Normal)
            if (!isRefContext) {
                var invokeExpr = Expression.Call(
                    Expression.TypeAs(delegateParam, delegateType),
                    invokeMethod, 
                    contextExpr,
                    argParam
                );
                
                var lambda = 
                    Expression.Lambda<AnonymousActionInvoker<TArg>>(
                        invokeExpr, delegateParam, contextParam, mutatingBehaviourParam, argParam
                    );
                
                invoker = lambda.Compile();
            }
            // Compile ref invoker (Mutating)
            else {
                // Variables
                var castContextVar = Expression.Variable(contextType, "castContext");
                
                // Expressions
                var assignCastContext = Expression.Assign(
                    castContextVar, contextExpr
                );
                
                var castDelegateExpr = Expression.TypeAs(
                    delegateParam, delegateType
                );

                var invokeExpr = Expression.Call(
                    castDelegateExpr, invokeMethod, castContextVar, argParam
                );
                
                var ifMutatingSetContext = Expression.IfThen(
                    Expression.Equal(
                        mutatingBehaviourParam,
                        Expression.Constant(MutatingClosureBehaviour.Retain)
                    ),
                    contextAsAnonymousValue
                        ? Expression.Assign(contextParam, castContextVar)
                        : Expression.Call(contextParam, setMethod.MakeGenericMethod(contextType), castContextVar)
                );
                
                var block = Expression.Block(
                    new[] { castContextVar},
                    assignCastContext,
                    invokeExpr,
                    ifMutatingSetContext
                );

                var lambda = 
                    Expression.Lambda<AnonymousActionInvoker<TArg>>(
                        block, delegateParam, contextParam, mutatingBehaviourParam, argParam
                    );
                
                invoker = lambda.Compile();
            }
            
            s_argActionInvokers[delegateType] = invoker;
        }

        return (AnonymousActionInvoker<TArg>)invoker;
    }
    
    /// <summary>
    /// Gets an invoker delegate for an anonymous function closure with the specified return type.
    /// The invoker will handle both normal and mutating (ref context) functions.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the closure function.</typeparam>
    /// <param name="delegate">The closure delegate to invoke.</param>
    /// <returns>An <see cref="AnonymousFuncInvoker{TReturn}"/> that can invoke the closure with a context and mutating behaviour, returning a value.</returns>
    /// <exception cref="InvalidCastException">
    /// Thrown if the delegate return type does not match <typeparamref name="TReturn"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the delegate type does not have a context or return type.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the delegate type does not match the expected signature for an anonymous function invoker.
    /// </exception>>
    public static AnonymousFuncInvoker<TReturn> GetFuncInvoker<TReturn>(Delegate @delegate) {
        var delegateType = @delegate.GetType();
        
        if (!s_funcInvokers.TryGetValue(delegateType, out var invoker)) {
            // delegate context and return types
            var genericArguments = delegateType.GetGenericArguments();
            var contextType = genericArguments[0] ??
                              throw new InvalidOperationException(
                                  $"Delegate type '{delegateType.Name}' does not have a context type.");
            var returnType = genericArguments[1] ??
                             throw new InvalidOperationException(
                                 $"Delegate type '{delegateType.Name}' does not have a return type.");
            
            // Validate types
            if (returnType != typeof(TReturn)) {
                throw new InvalidCastException(
                    $"Delegate type '{delegateType.Name}' return type '{returnType.Name}' does not match expected type '{typeof(TReturn).Name}'.");
            }

            // Parameters (AnonymousClosure, ref AnonymousValue)
            var delegateParam = Expression.Parameter(typeof(Delegate), "delegate");
            var contextParam = Expression.Parameter(typeof(AnonymousValue).MakeByRefType(), "context");
            var mutatingBehaviourParam = Expression.Parameter(typeof(MutatingClosureBehaviour), "mutatingBehaviour");

            // Methods
            var asMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.As)) 
                           ?? throw new MissingMethodException($"Method 'As' not found in AnonymousValue.");
            
            var setMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.Set))
                            ?? throw new MissingMethodException($"Method 'Set' not found in AnonymousValue.");
            
            var invokeMethod = delegateType.GetMethod("Invoke") 
                               ?? throw new MissingMethodException($"Method 'Invoke' not found in {delegateType}.");

            // Determine if the context should be passed as AnonymousValue or as a specific type
            var contextAsAnonymousValue = contextType == typeof(AnonymousValue);
            Expression contextExpr = contextAsAnonymousValue
                ? contextParam
                : Expression.Call(contextParam, asMethod.MakeGenericMethod(contextType));
            
            // ref parameters
            var invokeMethodParameters = invokeMethod.GetParameters();
            var isRefContext = invokeMethodParameters[0].ParameterType.IsByRef;
            
            // Compile non ref invoker
            if (!isRefContext) {
                var invokeExpr = Expression.Call(
                    Expression.TypeAs(delegateParam, delegateType),
                    invokeMethod, 
                    contextExpr
                );
            
                var lambda = 
                    Expression.Lambda<AnonymousFuncInvoker<TReturn>>(
                        invokeExpr, delegateParam, contextParam, mutatingBehaviourParam
                    );
            
                invoker = lambda.Compile();
            }
            else {
                // Variables
                var castContextVar = Expression.Variable(contextType, "castContext");
                
                // Expressions
                var assignCastContext = Expression.Assign(
                    castContextVar, contextExpr
                );
                
                var castDelegateExpr = Expression.TypeAs(
                    delegateParam, delegateType
                );

                var invokeExpr = Expression.Call(
                    castDelegateExpr, invokeMethod, castContextVar
                );
                
                var ifMutatingSetContext = Expression.IfThen(
                    Expression.Equal(
                        mutatingBehaviourParam,
                        Expression.Constant(MutatingClosureBehaviour.Retain)
                    ),
                    contextAsAnonymousValue
                        ? Expression.Assign(contextParam, castContextVar)
                        : Expression.Call(contextParam, setMethod.MakeGenericMethod(contextType), castContextVar)
                );
                
                var block = Expression.Block(
                    new[] { castContextVar},
                    assignCastContext,
                    invokeExpr,
                    ifMutatingSetContext
                );

                var lambda = 
                    Expression.Lambda<AnonymousActionInvoker>(
                        block, delegateParam, contextParam, mutatingBehaviourParam
                    );
                
                invoker = lambda.Compile();
            }
            
            s_funcInvokers[delegateType] = invoker;
        }

        return (AnonymousFuncInvoker<TReturn>)invoker;
    }
    
    /// <summary>
    /// Gets an invoker delegate for an anonymous function closure with an argument and return type.
    /// The invoker will handle both normal and mutating (ref context) functions.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the closure.</typeparam>
    /// <typeparam name="TReturn">The return type of the closure function.</typeparam>
    /// <param name="delegate">The closure delegate to invoke.</param>
    /// <returns>An <see cref="AnonymousFuncInvoker{TArg, TReturn}"/> that can invoke the closure with a context, mutating behaviour, and argument, returning a value.</returns>
    /// <exception cref="InvalidCastException">
    /// Thrown if the delegate argument or return type does not match <typeparamref name="TArg"/> or <typeparamref name="TReturn"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the delegate type does not have a context, argument, or return type.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the delegate type does not match the expected signature for an anonymous function invoker with an argument.
    /// </exception>
    public static AnonymousFuncInvoker<TArg, TReturn> GetFuncInvoker<TArg, TReturn>(Delegate @delegate) {
        var delegateType = @delegate.GetType();
        
        if (!s_argFuncInvokers.TryGetValue(delegateType, out var invoker)) {
            // delegate context and argument types
            var genericArguments = delegateType.GetGenericArguments();
            var contextType = genericArguments[0] ??
                              throw new InvalidOperationException(
                                  $"Delegate type '{delegateType.Name}' does not have a context type.");
            var argType = genericArguments[1] ??
                          throw new InvalidOperationException(
                              $"Delegate type '{delegateType.Name}' does not have an argument type.");
            
            var returnType = genericArguments[2] ??
                             throw new InvalidOperationException(
                                 $"Delegate type '{delegateType.Name}' does not have a return type.");
            
            // Validate types
            if (argType != typeof(TArg)) {
                throw new InvalidCastException(
                    $"Delegate type '{delegateType.Name}' argument type '{argType.Name}' does not match expected type '{typeof(TArg).Name}'.");
            }
            
            if (returnType != typeof(TReturn)) {
                throw new InvalidCastException(
                    $"Delegate type '{delegateType.Name}' return type '{returnType.Name}' does not match expected type '{typeof(TReturn).Name}'.");
            }

            // Parameters (AnonymousClosure, ref AnonymousValue)
            var delegateParam = Expression.Parameter(typeof(Delegate), "delegate");
            var contextParam = Expression.Parameter(typeof(AnonymousValue).MakeByRefType(), "context");
            var mutatingBehaviourParam = Expression.Parameter(typeof(MutatingClosureBehaviour), "mutatingBehaviour");
            var argParam = Expression.Parameter(typeof(TArg).MakeByRefType(), "arg");
            
            // Methods
            var asMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.As)) 
                           ?? throw new MissingMethodException($"Method 'As' not found in AnonymousValue.");
            
            var setMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.Set)) 
                            ?? throw new MissingMethodException($"Method 'Set' not found in AnonymousValue.");
            
            var invokeMethod = delegateType.GetMethod("Invoke") 
                               ?? throw new MissingMethodException($"Method 'Invoke' not found in {delegateType}.");

            // Determine if the context should be passed as AnonymousValue or as a specific type
            var contextAsAnonymousValue = contextType == typeof(AnonymousValue);
            Expression contextExpr = contextAsAnonymousValue
                ? contextParam
                : Expression.Call(contextParam, asMethod.MakeGenericMethod(contextType));
            
            // ref parameters
            var invokeMethodParameters = invokeMethod.GetParameters();
            var isRefContext = invokeMethodParameters[0].ParameterType.IsByRef;

            // Compile non-ref invoker (Normal)
            if (!isRefContext) {
                var invokeExpr = Expression.Call(
                    Expression.TypeAs(delegateParam, delegateType),
                    invokeMethod, 
                    contextExpr,
                    argParam
                );
                
                var lambda = 
                    Expression.Lambda<AnonymousFuncInvoker<TArg, TReturn>>(
                        invokeExpr, delegateParam, contextParam, mutatingBehaviourParam, argParam
                    );
                
                invoker = lambda.Compile();
            }
            // Compile ref invoker (Mutating)
            else {
                // Variables
                var castContextVar = Expression.Variable(contextType, "castContext");
                
                // Expressions
                var assignCastContext = Expression.Assign(
                    castContextVar, contextExpr
                );
                
                var castDelegateExpr = Expression.TypeAs(
                    delegateParam, delegateType
                );

                var invokeExpr = Expression.Call(
                    castDelegateExpr, invokeMethod, castContextVar, argParam
                );
                
                var ifMutatingSetContext = Expression.IfThen(
                    Expression.Equal(
                        mutatingBehaviourParam,
                        Expression.Constant(MutatingClosureBehaviour.Retain)
                    ),
                    contextAsAnonymousValue
                        ? Expression.Assign(contextParam, castContextVar)
                        : Expression.Call(contextParam, setMethod.MakeGenericMethod(contextType), castContextVar)
                );
                
                var block = Expression.Block(
                    new[] { castContextVar},
                    assignCastContext,
                    invokeExpr,
                    ifMutatingSetContext
                );

                var lambda = 
                    Expression.Lambda<AnonymousFuncInvoker<TArg, TReturn>>(
                        block, delegateParam, contextParam, mutatingBehaviourParam, argParam
                    );
                
                invoker = lambda.Compile();
            }
            
            s_argFuncInvokers[delegateType] = invoker;
        }

        return (AnonymousFuncInvoker<TArg, TReturn>)invoker;
    }
}