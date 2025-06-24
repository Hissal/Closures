using System.Reflection;

namespace Lh.Closures.Reflection.Experimental;

public static class ClosureConverter {
    public static bool TryConvertToTypedClosure<TAnonymousClosure, TConvertedClosure>(this TAnonymousClosure closure, out TConvertedClosure convertedClosure) 
        where TAnonymousClosure : struct, IAnonymousClosure
        where TConvertedClosure : struct, IClosure {
        
        // Get the context and delegate properties from the closure
        var context = typeof(TAnonymousClosure).GetProperty("Context")?.GetValue(closure, null);
        var @delegate = typeof(TAnonymousClosure).GetProperty("Delegate")?.GetValue(closure, null);
        
        if (context is null || @delegate is null) {
            convertedClosure = default;
            return false;
        }
        
        var conversionType = GetClosureTypeOrAnonymous(@delegate.GetType());
        
        if (typeof(TConvertedClosure).IsAssignableFrom(conversionType)) {
            convertedClosure = new TConvertedClosure();
            convertedClosure.GetType().GetProperty("Context")?.SetValue(convertedClosure, context, null);
            convertedClosure.GetType().GetProperty("Delegate")?.SetValue(convertedClosure, @delegate, null);
            return true;
        }
        
        convertedClosure = default;
        return false;
    }
    
    public static bool TryConvertToTypedClosure<TContext, TDelegate>(this AnonymousClosure<TContext, TDelegate> closure, out IClosure<TContext, TDelegate>? convertedClosure) 
        where TDelegate : Delegate {
        
        convertedClosure = null;
        
        if (!TryConvertToTypedClosure<AnonymousClosure<TContext, TDelegate>>(closure, out var anonymousToIClosure))
            return false;

        if (anonymousToIClosure is not IClosure<TContext, TDelegate> converted)
            return false;
        
        convertedClosure = converted;
        return true;
    }

    static bool TryConvertToTypedClosure<TClosure>(this TClosure closure, out IClosure? convertedClosure)
        where TClosure : struct, IClosure {
        
        // Get the context and delegate properties from the closure
        var context = typeof(TClosure).GetProperty("Context")?.GetValue(closure, null);
        var @delegate = typeof(TClosure).GetProperty("Delegate")?.GetValue(closure, null);
        
        if (context is null || @delegate is null) {
            convertedClosure = null;
            return false;
        }
        
        var conversionType = GetClosureTypeOrAnonymous(@delegate.GetType());
        
        var method = typeof(Closure)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                    m is { Name: nameof(Closure.Create), IsGenericMethodDefinition: true }
                    && m.GetGenericArguments().Length == 3
                    && m.GetParameters().Length == 2 // or 3 for the overload with mutatingBehaviour
            );
        
        if (method is null) {
            convertedClosure = null;
            return false;
        }

        var genericMethod = method.MakeGenericMethod(context.GetType(), @delegate.GetType(), conversionType);
        convertedClosure = (IClosure?)genericMethod.Invoke(null, [context, @delegate]);
        
        return convertedClosure != null;
    }
    
    public static Type GetClosureTypeOrAnonymous(Type delegateType) {
        var genericArgs = delegateType.GetGenericArguments();
        
        if (genericArgs.Length == 0)
            throw new ArgumentException("Delegate type must have at least one generic argument.");
        
        var contextType = genericArgs[0];
        var argType = genericArgs.Length > 1 ? genericArgs[1] : null;
        
        if (contextType == null || delegateType == null)
            throw new ArgumentNullException(nameof(contextType), "delegate type and context (first argument) of the delegate type must not be null.");
        
        var genericArgumentCount = argType is null ? 2 : 3;
        
        // Check if the delegate has an argument
        var method = typeof(ClosureConverter).GetMethod(nameof(GetClosureTypeOrAnonymous),
            genericArgumentCount,
            BindingFlags.Public | BindingFlags.Static, 
            null, 
            Type.EmptyTypes, 
            null
        );

#if DEBUG
        if (method is null) {
            throw new InvalidOperationException("Method GetClosureTypeOrAnonymous with 3 generic arguments not found.");
        }
#else
        if (methodWithArg is null) 
            return AnonymousClosureType();
#endif
        
        var genericMethod = method.MakeGenericMethod(contextType, delegateType);
        var type = (Type?)genericMethod.Invoke(null, null);
        
        if (type is not null)
            return type;
        
        // Return AnonymousClosure if no specific type is found
        return AnonymousClosureType();
        
        Type AnonymousClosureType() => typeof(AnonymousClosure<,>).MakeGenericType(contextType, delegateType);
    }
    
    public static Type GetClosureTypeOrAnonymous<TContext, TDelegate>() where TDelegate : Delegate {
        if (typeof(TDelegate).IsGenericType is false)
            return typeof(AnonymousClosure<TContext, TDelegate>);
        
        return typeof(TDelegate) switch {
            { } type when type.GetGenericTypeDefinition() == typeof(Action<>) => typeof(ClosureAction<TContext>),
            { } type when type.GetGenericTypeDefinition() == typeof(RefAction<>) => typeof(MutatingClosureAction<TContext>),
            _ => typeof(AnonymousClosure<TContext, TDelegate>)
        };
    }
    
    public static Type GetClosureTypeOrAnonymous<TContext, TArg, TDelegate>() where TDelegate : Delegate {
        if (typeof(TDelegate).IsGenericType is false)
            return typeof(AnonymousClosure<TContext, TDelegate>);
        
        return typeof(TDelegate) switch {
            { } type when type.GetGenericTypeDefinition() == typeof(Action<TContext, TArg>) => typeof(ClosureAction<TContext, TArg>),
            { } type when type.GetGenericTypeDefinition() == typeof(RefActionWithNormalContext<TContext, TArg>) => typeof(ClosureRefAction<TContext, TArg>),
            { } type when type.GetGenericTypeDefinition() == typeof(ActionWithRefContext<TContext, TArg>) => typeof(MutatingClosureAction<TContext, TArg>),
            { } type when type.GetGenericTypeDefinition() == typeof(RefAction<TContext, TArg>) => typeof(MutatingClosureRefAction<TContext, TArg>),
            _ => typeof(AnonymousClosure<TContext, TDelegate>)
        };
    }
}