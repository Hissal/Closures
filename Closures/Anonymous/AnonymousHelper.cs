using System.Runtime.CompilerServices;

namespace Closures;

internal static class AnonymousHelper {
    static readonly Dictionary<Type, Type> s_closureTypeToInterfaceTypeMap = new Dictionary<Type, Type>();
    static readonly Dictionary<Type, Type[]> s_interfaceTypeToGenericArgumentsMap = new Dictionary<Type, Type[]>();
    static readonly Dictionary<Type, Type[]> s_delegateTypeToGenericArgumentsMap = new Dictionary<Type, Type[]>();

    public static Type GetInterfaceType<TClosureType>() where TClosureType : IClosure {
        if (s_closureTypeToInterfaceTypeMap.TryGetValue(typeof(TClosureType), out var interfaceType)) {
            return interfaceType;
        }

        // If not found, create the interface type dynamically
        interfaceType = typeof(TClosureType).GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IClosure<,>));

        if (interfaceType is null)
            throw new InvalidOperationException($"The type {typeof(TClosureType).Name} does not implement IClosure<> interface.");

        s_closureTypeToInterfaceTypeMap.Add(typeof(TClosureType), interfaceType);
        return interfaceType;
    }
    
    public static Type[] GetInterfaceGenericArguments(Type interfaceType) {
        if (s_interfaceTypeToGenericArgumentsMap.TryGetValue(interfaceType, out var genericArguments)) {
            return genericArguments;
        }

        // If not found, get the generic arguments from the interface type
        genericArguments = interfaceType.GetGenericArguments();
        s_interfaceTypeToGenericArgumentsMap.Add(interfaceType, genericArguments);
        return genericArguments;
    }

    public static bool CanConvert<TClosureType, TClosure>(TClosure closure) where TClosureType : IClosure where TClosure : IAnonymousClosure {
        var anonymousContextType = closure.Context.GetUnderlyingType();
        var anonymousDelegateType = closure.Delegate.GetType();
        var conversionInterfaceType = GetInterfaceType<TClosureType>();
        var genericArguments = GetInterfaceGenericArguments(conversionInterfaceType);
        
        return (genericArguments[0] == anonymousContextType || genericArguments[0] == typeof(AnonymousValue))
               && genericArguments[1] == anonymousDelegateType;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ShouldThrow(Exception ex) {
        return ex is InvalidOperationException or InvalidCastException or ArgumentException;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAction(Delegate del) => del.Method.ReturnType == typeof(void);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFunc(Delegate del) => del.Method.ReturnType != typeof(void);
    
    public static Type[] GetGenericArguments(Delegate del) {
        var delegateType = del.GetType();
        
        if (s_delegateTypeToGenericArgumentsMap.TryGetValue(delegateType, out var genericArguments))
            return genericArguments;
        
        // If not found, get the generic arguments from the delegate type
        genericArguments = delegateType.GetGenericArguments();
        s_delegateTypeToGenericArgumentsMap.Add(delegateType, genericArguments);
        return genericArguments;
    }
    public static bool HasArgOfType<TArg>(Delegate del) {
        var genericArguments = GetGenericArguments(del);
        return genericArguments.Length > 1 && genericArguments[1] == typeof(TArg);
    }
}