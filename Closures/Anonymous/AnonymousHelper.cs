using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Closures;

public static class AnonymousHelper {
    static readonly ConcurrentDictionary<Type, Type> s_closureTypeToInterfaceTypeMap = new ();
    static readonly ConcurrentDictionary<Type, Type[]> s_interfaceTypeToGenericArgumentsMap = new ();
    static readonly ConcurrentDictionary<Type, Type[]> s_delegateTypeToGenericArgumentsMap = new ();

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
    
    public static Type[] GetInterfaceGenericArguments(Type interfaceType) {
        return s_interfaceTypeToGenericArgumentsMap.GetOrAdd(interfaceType, type => type.GetGenericArguments());
    }

    public static bool CanConvert<TFrom, TTo>(TFrom closure) where TTo : IClosure where TFrom : IAnonymousClosure {
        var anonymousContextType = closure.Context.GetUnderlyingType();
        var anonymousDelegateType = closure.Delegate.GetType();
        var conversionInterfaceType = GetInterfaceType<TTo>();
        var genericArguments = GetInterfaceGenericArguments(conversionInterfaceType);
        
        return (genericArguments[0] == anonymousContextType || genericArguments[0] == typeof(AnonymousValue))
               && genericArguments[1] == anonymousDelegateType;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ShouldThrow(Exception ex, ExceptionHandlingPolicy policy) {
        return policy switch {
            ExceptionHandlingPolicy.HandleExpected => 
                ex is not (InvalidOperationException or InvalidCastException or ArgumentException),
            ExceptionHandlingPolicy.HandleAll => true,
            ExceptionHandlingPolicy.HandleNone => false,
            _ => throw new ArgumentOutOfRangeException(nameof(policy), policy, null)
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAction(Delegate del) => del.Method.ReturnType == typeof(void);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFunc(Delegate del) => del.Method.ReturnType != typeof(void);
    
    public static Type[] GetGenericArguments(Delegate @delegate) {
        return GetGenericArguments(@delegate.GetType());
    }
    public static Type[] GetGenericArguments(Type delegateType) {
        return s_delegateTypeToGenericArgumentsMap.GetOrAdd(delegateType, type => type.GetGenericArguments());
    }
    
    public static bool HasArg(Delegate del) { 
        var genericArguments = GetGenericArguments(del);
        return IsAction(del) ? genericArguments.Length > 1 : genericArguments.Length > 2;
    }
    public static bool HasArgOfType<TArg>(Delegate del) {
        var genericArguments = GetGenericArguments(del);
        return IsAction(del) 
            ? genericArguments.Length > 1 && genericArguments[1] == typeof(TArg) 
            : genericArguments.Length > 2 && genericArguments[1] == typeof(TArg);
    }
}