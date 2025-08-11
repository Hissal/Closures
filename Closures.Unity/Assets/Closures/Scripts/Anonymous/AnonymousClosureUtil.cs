using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Closures.Anonymous;

/// <summary>
/// Provides utility methods for working with anonymous closures.
/// </summary>
/// <remarks>
/// Do not use with normal delegates, methods such as <see cref="HasArg(System.Delegate)"/> skips the first argument (context).
/// </remarks>
public static class AnonymousClosureUtil {
    static readonly ConcurrentDictionary<Type, Type> s_closureTypeToInterfaceTypeMap = new ();
    static readonly ConcurrentDictionary<Type, Type[]> s_interfaceTypeToGenericArgumentsMap = new ();
    static readonly ConcurrentDictionary<Type, Type[]> s_delegateTypeToGenericArgumentsMap = new ();
    static readonly ConcurrentDictionary<Type, MethodInfo> s_delegateTypeToMethodInfoMap = new ();
    static readonly ConcurrentDictionary<Type, ParameterInfo[]> s_delegateTypeToParametersMap = new ();

    static AnonymousClosureUtil() {
        ClosureManager.OnCacheClear += ClearCache;
    }

    public static void ClearCache() {
        s_closureTypeToInterfaceTypeMap.Clear();
        s_interfaceTypeToGenericArgumentsMap.Clear();
        s_delegateTypeToGenericArgumentsMap.Clear();
        s_delegateTypeToMethodInfoMap.Clear();
        s_delegateTypeToParametersMap.Clear();
    }
    
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
    /// Determines whether the specified delegate can be invoked as a delegate of type <typeparamref name="TDelegate"/>.
    /// </summary>
    /// <param name="delegate">The delegate to invoke including context</param>
    /// <typeparam name="TDelegate">The delegate type excluding context</typeparam>
    /// <returns>True if the type arguments excluding the first one of the given delegate (TContext) match; Otherwise false</returns>
    public static bool InvokableAs<TDelegate>(Delegate @delegate) where TDelegate : Delegate {
        if (IsAction(@delegate) != IsAction<TDelegate>())
            return false;
        
        var parameters = GetParameters(@delegate);
        var expectedParameters = GetParameters<TDelegate>();
        
        if (parameters.Length - 1 != expectedParameters.Length)
            return false;
        
        for (var i = 0; i < parameters.Length - 1; i++) {
            if (parameters[i + 1].ParameterType != expectedParameters[i].ParameterType) {
                return false;
            }
        }
        
        return true;
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
    /// Determines whether the specified delegate type is an action (returns void).
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type to check.</typeparam>
    /// <returns>True if the delegate type is an action; otherwise, false.</returns>
    public static bool IsAction<TDelegate>() where TDelegate : Delegate {
        return GetInvokeMethod<TDelegate>().ReturnType == typeof(void);
    }
    /// <summary>
    /// Determines whether the specified delegate type is an action (returns void).
    /// </summary>
    /// <param name="delegateType">The delegate type to check.</param>
    /// <returns>True if the delegate type is an action; otherwise, false.</returns>
    public static bool IsAction(Type delegateType) {
        return GetInvokeMethod(delegateType).ReturnType == typeof(void);
    }
    
    /// <summary>
    /// Determines whether the specified delegate is a function (returns a value).
    /// </summary>
    /// <param name="del">The delegate to check.</param>
    /// <returns>True if the delegate is a function; otherwise, false.</returns>
    public static bool IsFunc(Delegate del) => del.Method.ReturnType != typeof(void);
    /// <summary>
    /// Determines whether the specified delegate type is a function (returns a value).
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type to check.</typeparam>
    /// <returns>True if the delegate type is a function; otherwise, false.</returns>
    public static bool IsFunc<TDelegate>() where TDelegate : Delegate {
        return GetInvokeMethod<TDelegate>().ReturnType != typeof(void);
    }
    /// <summary>
    /// Determines whether the specified delegate type is a function (returns a value).
    /// </summary>
    /// <param name="delegateType">The delegate type to check.</param>
    /// <returns>True if the delegate type is a function; otherwise, false.</returns>
    public static bool IsFunc(Type delegateType) {
        return GetInvokeMethod(delegateType).ReturnType != typeof(void);
    }
    
    /// <summary>
    /// Gets the generic argument types of the specified delegate instance.
    /// </summary>
    /// <param name="delegate">The delegate instance.</param>
    /// <returns>An array of generic argument types.</returns>
    public static Type[] GetGenericArguments(Delegate @delegate) {
        return s_delegateTypeToGenericArgumentsMap.GetOrAdd(@delegate.GetType(), type => type.GetGenericArguments());
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
    /// Gets the generic argument types of the specified delegate type.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type.</typeparam>
    /// <returns>An array of generic argument types.</returns>
    public static Type[] GetGenericArguments<TDelegate>() {
        return s_delegateTypeToGenericArgumentsMap.GetOrAdd(typeof(TDelegate), type => type.GetGenericArguments());
    }
    
    /// <summary>
    /// Gets the parameter information for the specified delegate instance.
    /// </summary>
    /// <param name="delegate">The delegate instance.</param>
    /// <returns>An array of <see cref="ParameterInfo"/> objects describing the parameters of the delegate's Invoke method.</returns>
    public static ParameterInfo[] GetParameters(Delegate @delegate) {
        return s_delegateTypeToParametersMap.GetOrAdd(@delegate.GetType(), type => GetInvokeMethod(type).GetParameters());
    }

    /// <summary>
    /// Gets the parameter information for the specified delegate type.
    /// </summary>
    /// <param name="delegateType">The delegate type.</param>
    /// <returns>An array of <see cref="ParameterInfo"/> objects describing the parameters of the delegate's Invoke method.</returns>
    public static ParameterInfo[] GetParameters(Type delegateType) {
        return s_delegateTypeToParametersMap.GetOrAdd(delegateType, type => GetInvokeMethod(type).GetParameters());
    }

    /// <summary>
    /// Gets the parameter information for the specified delegate type.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type.</typeparam>
    /// <returns>An array of <see cref="ParameterInfo"/> objects describing the parameters of the delegate's Invoke method.</returns>
    public static ParameterInfo[] GetParameters<TDelegate>() where TDelegate : Delegate {
        return s_delegateTypeToParametersMap.GetOrAdd(typeof(TDelegate), type => GetInvokeMethod(type).GetParameters());
    }

    /// <summary>
    /// Gets the Invoke method for the specified delegate type.
    /// </summary>
    /// <param name="delegateType">The delegate type.</param>
    /// <returns>The <see cref="MethodInfo"/> representing the Invoke method.</returns>
    public static MethodInfo GetInvokeMethod(Type delegateType) {
        return s_delegateTypeToMethodInfoMap.GetOrAdd(delegateType, 
            type => type.GetMethod("Invoke") ??
                    throw new InvalidOperationException($"The type {type.Name} does not have an 'Invoke' method.")
        );
    }

    /// <summary>
    /// Gets the Invoke method for the specified delegate type.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type.</typeparam>
    /// <returns>The <see cref="MethodInfo"/> representing the Invoke method.</returns>
    public static MethodInfo GetInvokeMethod<TDelegate>() where TDelegate : Delegate {
        return s_delegateTypeToMethodInfoMap.GetOrAdd(typeof(TDelegate), 
            type => type.GetMethod("Invoke") ??
                    throw new InvalidOperationException($"The type {type.Name} does not have an 'Invoke' method.")
        );
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
    /// Determines whether the specified delegate type has an argument.
    /// </summary>
    /// <param name="delegateType">The delegate type to check.</param>
    /// <returns>True if the delegate type has an argument; otherwise, false.</returns>
    public static bool HasArg(Type delegateType) { 
        var genericArguments = GetGenericArguments(delegateType);
        return IsAction(delegateType) ? genericArguments.Length > 1 : genericArguments.Length > 2;
    }
    /// <summary>
    /// Determines whether the specified delegate type has an argument.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type to check.</typeparam>
    /// <returns>True if the delegate type has an argument; otherwise, false.</returns>
    public static bool HasArg<TDelegate>() where TDelegate : Delegate {
        var genericArguments = GetGenericArguments<TDelegate>();
        return IsAction<TDelegate>() ? genericArguments.Length > 1 : genericArguments.Length > 2;
    }

    /// <summary>
    /// Determines whether the specified delegate has an argument of the given type.
    /// </summary>
    /// <param name="delegate">The delegate to check.</param>
    /// <typeparam name="TArg">The argument type to check for.</typeparam>
    /// <returns>True if the delegate has an argument; otherwise, false.</returns>
    public static bool HasArgOfType<TArg>(Delegate @delegate) {
        var genericArguments = GetGenericArguments(@delegate);
        return IsAction(@delegate) 
            ? genericArguments.Length > 1 && genericArguments[1] == typeof(TArg) 
            : genericArguments.Length > 2 && genericArguments[1] == typeof(TArg);
    }
    /// <summary>
    /// Determines whether the specified delegate type has an argument of the given type.
    /// </summary>
    /// <typeparam name="TArg">The argument type to check for.</typeparam>
    /// <param name="delegateType">The delegate type to check.</param>
    /// <returns>True if the delegate type has an argument of the specified type; otherwise, false.</returns>
    public static bool HasArgOfType<TArg>(Type delegateType) {
        var genericArguments = GetGenericArguments(delegateType);
        return IsAction(delegateType) 
            ? genericArguments.Length > 1 && genericArguments[1] == typeof(TArg) 
            : genericArguments.Length > 2 && genericArguments[1] == typeof(TArg);
    }
    /// <summary>
    /// Determines whether the specified delegate type has an argument of the given type.
    /// </summary>
    /// <typeparam name="TArg">The argument type to check for.</typeparam>
    /// <typeparam name="TDelegate">The delegate type to check.</typeparam>
    /// <returns>True if the delegate type has an argument of the specified type; otherwise, false.</returns>
    public static bool HasArgOfType<TArg, TDelegate>() where TDelegate : Delegate {
        var genericArguments = GetGenericArguments<TDelegate>();
        return IsAction<TDelegate>()
            ? genericArguments.Length > 1 && genericArguments[1] == typeof(TArg)
            : genericArguments.Length > 2 && genericArguments[1] == typeof(TArg);
    }
}