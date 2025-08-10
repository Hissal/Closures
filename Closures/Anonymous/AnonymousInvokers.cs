using System.Collections.Concurrent;
using System.Linq.Expressions;
// ReSharper disable UseCollectionExpression

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
    public delegate void AnonymousActionInvoker(Delegate @delegate, ref AnonymousValue context, MutatingBehaviour mutatingBehaviour);
    public delegate void AnonymousActionInvoker<TArg>(Delegate @delegate, ref AnonymousValue context, MutatingBehaviour mutatingBehaviour, ref TArg arg);
    
    static readonly ConcurrentDictionary<Type, AnonymousActionInvoker> s_actionInvokers = new();
    static readonly ConcurrentDictionary<Type, Delegate> s_argActionInvokers = new();
    
    // Funcs
    public delegate TReturn AnonymousFuncInvoker<out TReturn>(Delegate @delegate, ref AnonymousValue context, MutatingBehaviour mutatingBehaviour);
    public delegate TReturn AnonymousFuncInvoker<TArg, out TReturn>(Delegate @delegate, ref AnonymousValue context, MutatingBehaviour mutatingBehaviour, ref TArg arg);
    
    static readonly ConcurrentDictionary<Type, Delegate> s_funcInvokers = new();
    static readonly ConcurrentDictionary<Type, Delegate> s_argFuncInvokers = new();
    
    static AnonymousInvokers() {
        ClosureManager.OnCacheClear += ClearCache;
    }

    public static void ClearCache() {
        s_actionInvokers.Clear();
        s_argActionInvokers.Clear();
        s_funcInvokers.Clear();
        s_argFuncInvokers.Clear();
    }
    
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
        if (!AnonymousHelper.IsAction(@delegate))
            throw new ArgumentException($"Delegate '{@delegate.GetType()}' is not a valid action.");
        
        return s_actionInvokers.GetOrAdd(@delegate.GetType(), CreateActionInvoker);
    }

    static AnonymousActionInvoker CreateActionInvoker(Type delegateType) {
        // delegate type, context and argument types
        var genericArguments = AnonymousHelper.GetGenericArguments(delegateType);
            
        var contextType = genericArguments[0] ??
                          throw new InvalidOperationException(
                              $"Delegate type '{delegateType.Name}' does not have a context type.");

        // Parameters (AnonymousClosure, ref AnonymousValue)
        var delegateParam = Expression.Parameter(typeof(Delegate), "delegate");
        var contextParam = Expression.Parameter(typeof(AnonymousValue).MakeByRefType(), "context");
        var mutatingBehaviourParam = Expression.Parameter(typeof(MutatingBehaviour), "mutatingBehaviour");

        // Methods
        var asMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.As)) 
                       ?? throw new MissingMethodException($"Method 'As' not found in AnonymousValue.");
            
        var setMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.SetValue)) 
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
                
            return lambda.Compile();
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
                    Expression.Constant(MutatingBehaviour.Mutate)
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
                
            return lambda.Compile();
        }
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
        if (!AnonymousHelper.IsAction(@delegate))
            throw new ArgumentException($"Delegate '{@delegate.GetType()}' is not a valid action.");
        
        return (AnonymousActionInvoker<TArg>)s_argActionInvokers.GetOrAdd(@delegate.GetType(), CreateActionInvoker<TArg>);
    }

    static AnonymousActionInvoker<TArg> CreateActionInvoker<TArg>(Type delegateType) {
        // delegate context and argument types
        var genericArguments = AnonymousHelper.GetGenericArguments(delegateType);
        
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
        var mutatingBehaviourParam = Expression.Parameter(typeof(MutatingBehaviour), "mutatingBehaviour");
        var argParam = Expression.Parameter(typeof(TArg).MakeByRefType(), "arg");
        
        // Methods
        var asMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.As)) 
                       ?? throw new MissingMethodException($"Method 'As' not found in AnonymousValue.");
        
        var setMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.SetValue)) 
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
            
            return lambda.Compile();
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
                    Expression.Constant(MutatingBehaviour.Mutate)
                ),
                contextAsAnonymousValue
                    ? Expression.Assign(contextParam, castContextVar)
                    : Expression.Call(contextParam, setMethod.MakeGenericMethod(contextType), castContextVar)
            );

            var block = Expression.Block(
                new[] { castContextVar },
                assignCastContext,
                invokeExpr,
                ifMutatingSetContext
            );

            var lambda =
                Expression.Lambda<AnonymousActionInvoker<TArg>>(
                    block, delegateParam, contextParam, mutatingBehaviourParam, argParam
                );

            return lambda.Compile();
        }
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
        if (!AnonymousHelper.IsFunc(@delegate))
            throw new ArgumentException($"Delegate '{@delegate.GetType()}' is not a valid function.");
    
        return (AnonymousFuncInvoker<TReturn>)s_funcInvokers.GetOrAdd(@delegate.GetType(), CreateFuncInvoker<TReturn>);
    }

    static AnonymousFuncInvoker<TReturn> CreateFuncInvoker<TReturn>(Type delegateType) {
        // delegate context and return types
        var genericArguments = AnonymousHelper.GetGenericArguments(delegateType);
        
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
        var mutatingBehaviourParam = Expression.Parameter(typeof(MutatingBehaviour), "mutatingBehaviour");

        // Methods
        var asMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.As)) 
                       ?? throw new MissingMethodException($"Method 'As' not found in AnonymousValue.");
        
        var setMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.SetValue))
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
        
            return lambda.Compile();
        }
        else {
            // Variables
            var castContextVar = Expression.Variable(contextType, "castContext");
            var returnVar = Expression.Variable(returnType, "returnValue");

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
            
            var assignReturnInvokeExpr = Expression.Assign(
                returnVar, invokeExpr
            );

            var ifMutatingSetContext = Expression.IfThen(
                Expression.Equal(
                    mutatingBehaviourParam,
                    Expression.Constant(MutatingBehaviour.Mutate)
                ),
                contextAsAnonymousValue
                    ? Expression.Assign(contextParam, castContextVar)
                    : Expression.Call(contextParam, setMethod.MakeGenericMethod(contextType), castContextVar)
            );

            var block = Expression.Block(
                new[] { castContextVar, returnVar },
                assignCastContext,
                assignReturnInvokeExpr,
                ifMutatingSetContext,
                returnVar
            );

            var lambda =
                Expression.Lambda<AnonymousFuncInvoker<TReturn>>(
                    block, delegateParam, contextParam, mutatingBehaviourParam
                );

            return lambda.Compile();
        }
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
        if (!AnonymousHelper.IsFunc(@delegate))
            throw new ArgumentException($"Delegate '{@delegate.GetType()}' is not a valid function.");
        
        return (AnonymousFuncInvoker<TArg, TReturn>)s_argFuncInvokers.GetOrAdd(@delegate.GetType(), CreateFuncInvoker<TArg, TReturn>);
    }

    static AnonymousFuncInvoker<TArg, TReturn> CreateFuncInvoker<TArg, TReturn>(Type delegateType) {
        // delegate context and argument types
        var genericArguments = AnonymousHelper.GetGenericArguments(delegateType);
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
        var mutatingBehaviourParam = Expression.Parameter(typeof(MutatingBehaviour), "mutatingBehaviour");
        var argParam = Expression.Parameter(typeof(TArg).MakeByRefType(), "arg");
        
        // Methods
        var asMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.As)) 
                       ?? throw new MissingMethodException($"Method 'As' not found in AnonymousValue.");
        
        var setMethod = typeof(AnonymousValue).GetMethod(nameof(AnonymousValue.SetValue)) 
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
            
            return lambda.Compile();
        }
        // Compile ref invoker (Mutating)
        else {
            // Variables
            var castContextVar = Expression.Variable(contextType, "castContext");
            var returnVar = Expression.Variable(returnType, "returnValue");
            
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
            
            var assignReturnInvokeExpr = Expression.Assign(
                returnVar, invokeExpr
            );
            
            var ifMutatingSetContext = Expression.IfThen(
                Expression.Equal(
                    mutatingBehaviourParam,
                    Expression.Constant(MutatingBehaviour.Mutate)
                ),
                contextAsAnonymousValue
                    ? Expression.Assign(contextParam, castContextVar)
                    : Expression.Call(contextParam, setMethod.MakeGenericMethod(contextType), castContextVar)
            );
            
            var block = Expression.Block(
                new[] { castContextVar, returnVar},
                assignCastContext,
                assignReturnInvokeExpr,
                ifMutatingSetContext,
                returnVar
            );
            
            var lambda = 
                Expression.Lambda<AnonymousFuncInvoker<TArg, TReturn>>(
                    block, delegateParam, contextParam, mutatingBehaviourParam, argParam
                );
            
            return lambda.Compile();
        }
    }
}