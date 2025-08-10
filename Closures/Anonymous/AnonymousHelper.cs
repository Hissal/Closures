using System.Collections.Concurrent;

namespace Closures;

/// <summary>
/// Provides utility methods for working with anonymous closures.
/// </summary>
public static class AnonymousHelper {
    static readonly ConcurrentDictionary<Type, Type> s_closureTypeToInterfaceTypeMap = new ();
    static readonly ConcurrentDictionary<Type, Type[]> s_interfaceTypeToGenericArgumentsMap = new ();
    static readonly ConcurrentDictionary<Type, Type[]> s_delegateTypeToGenericArgumentsMap = new ();
    
    /// <summary>
    /// Gets the <see cref="IClosure{TContext,TDelegate}"/> interface implemented by the specified closure type.
    /// </summary>
    /// <typeparam name="TClosureType">The closure type.</typeparam>
    /// <returns>The interface type implemented by the closure.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the type does not implement <see cref="IClosure{TContext,TDelegate}"/>.</exception>
    public static Type GetInterfaceType<TClosureType>() where TClosureType : IClosure {
        return s_closureTypeToInterfaceTypeMap.GetOrAdd(typeof(TClosureType), type => {
            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IClosure<,>));

            if (interfaceType is null)
                throw new InvalidOperationException(
                    $"The type {typeof(TClosureType).Name} does not implement IClosure<> interface.");

            return interfaceType;
        });
    }
    
    /// <summary>
    /// Gets the generic arguments of the specified closure interface type.
    /// </summary>
    /// <param name="interfaceType">The closure interface type.</param>
    /// <returns>An array of generic argument types.</returns>
    public static Type[] GetInterfaceGenericArguments(Type interfaceType) {
        return s_interfaceTypeToGenericArgumentsMap.GetOrAdd(interfaceType, type => type.GetGenericArguments());
    }

    /// <summary>
    /// Determines if an anonymous closure can be converted to a strongly-typed closure.
    /// </summary>
    /// <typeparam name="TFrom">The anonymous closure type.</typeparam>
    /// <typeparam name="TTo">The strongly-typed closure type.</typeparam>
    /// <param name="closure">The anonymous closure instance.</param>
    /// <returns>True if the conversion is possible; otherwise, false.</returns>
    public static bool CanConvert<TFrom, TTo>(TFrom closure) where TTo : IClosure where TFrom : IAnonymousClosure {
        var anonymousContextType = closure.Context.GetUnderlyingType();
        var anonymousDelegateType = closure.Delegate.GetType();
        var conversionInterfaceType = GetInterfaceType<TTo>();
        var genericArguments = GetInterfaceGenericArguments(conversionInterfaceType);
        
        return (genericArguments[0] == anonymousContextType || genericArguments[0] == typeof(AnonymousValue))
               && genericArguments[1] == anonymousDelegateType;
    }
    
    /// <summary>
    /// Determines whether an exception should be thrown based on the specified exception handling policy.
    /// Used in anonymous closure TryInvoke methods to decide whether to propagate exceptions or handle them.
    /// </summary>
    /// <param name="ex">The exception to evaluate.</param>
    /// <param name="policy">The exception handling policy.</param>
    /// <returns>True if the exception should be thrown; otherwise, false.</returns>
    public static bool ShouldThrow(Exception ex, ExceptionHandlingPolicy policy) {
        return policy switch {
            ExceptionHandlingPolicy.HandleExpected => 
                ex is not (InvalidOperationException or InvalidCastException or ArgumentException),
            ExceptionHandlingPolicy.HandleAll => true,
            ExceptionHandlingPolicy.HandleNone => false,
            _ => throw new ArgumentOutOfRangeException(nameof(policy), policy, null)
        };
    }
    
    /// <summary>
    /// Determines whether the specified delegate is an action (returns void).
    /// </summary>
    /// <param name="del">The delegate to check.</param>
    /// <returns>True if the delegate is an action; otherwise, false.</returns>
    public static bool IsAction(Delegate del) => del.Method.ReturnType == typeof(void);
    
    /// <summary>
    /// Determines whether the specified delegate is a function (returns a value).
    /// </summary>
    /// <param name="del">The delegate to check.</param>
    /// <returns>True if the delegate is a function; otherwise, false.</returns>
    public static bool IsFunc(Delegate del) => del.Method.ReturnType != typeof(void);
    
    /// <summary>
    /// Gets the generic argument types of the specified delegate instance.
    /// </summary>
    /// <param name="delegate">The delegate instance.</param>
    /// <returns>An array of generic argument types.</returns>
    public static Type[] GetGenericArguments(Delegate @delegate) {
        return GetGenericArguments(@delegate.GetType());
    }
    /// <summary>
    /// Gets the generic argument types of the specified delegate type.
    /// </summary>
    /// <param name="delegateType">The delegate type.</param>
    /// <returns>An array of generic argument types.</returns>
    public static Type[] GetGenericArguments(Type delegateType) {
        return s_delegateTypeToGenericArgumentsMap.GetOrAdd(delegateType, type => type.GetGenericArguments());
    }
    
    /// <summary>
    /// Determines whether the specified delegate has a TArg.
    /// </summary>
    /// <param name="del">The delegate to check.</param>
    /// <returns>True if the delegate has an argument; otherwise, false.</returns>
    public static bool HasArg(Delegate del) { 
        var genericArguments = GetGenericArguments(del);
        return IsAction(del) ? genericArguments.Length > 1 : genericArguments.Length > 2;
    }
    /// <summary>
    /// Determines whether the specified delegate has an argument of the given type.
    /// </summary>
    /// <typeparam name="TArg">The argument type to check for.</typeparam>
    /// <param name="del">The delegate to check.</param>
    /// <returns>True if the delegate has an argument of the specified type; otherwise, false.</returns>
    public static bool HasArgOfType<TArg>(Delegate del) {
        var genericArguments = GetGenericArguments(del);
        return IsAction(del) 
            ? genericArguments.Length > 1 && genericArguments[1] == typeof(TArg) 
            : genericArguments.Length > 2 && genericArguments[1] == typeof(TArg);
    }
}