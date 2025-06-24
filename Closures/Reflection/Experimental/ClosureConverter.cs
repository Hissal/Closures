using System.Reflection;

namespace Lh.Closures.Reflection.Experimental;

public static class ClosureConverter {
    static ClosureTypeResolver s_defaultResolver;

    static ClosureConverter() {
        s_defaultResolver ??= new ClosureTypeResolver();
    }
    
    public static void SetDefaultClosureTypeResolver(ClosureTypeResolver resolver) => s_defaultResolver = resolver;
    
    
    /// <summary>
    /// Converts a closure to an anonymous closure.
    /// </summary>
    /// <param name="closure">The closure to make anonymous.</param>
    /// <typeparam name="TContext">The type of context used.</typeparam>
    /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
    /// <returns>An <see cref="AnonymousClosure{TContext,TDelegate}"/></returns>
    /// <remarks>
    /// This will box the closure as it has to convert to <see cref="IClosure{TContext, TDelegate}"/> from a value type. <br></br>
    /// Specify the type of closure you are converting to avoid this <see cref="ConvertToAnonymous{TContext, TDelegate, TClosure}"/>
    /// </remarks>
    public static AnonymousClosure<TContext, TDelegate> ConvertToAnonymous<TContext, TDelegate>(IClosure<TContext, TDelegate> closure) 
        where TDelegate : Delegate 
        => new AnonymousClosure<TContext, TDelegate> {
            Context = closure.Context,
            Delegate = closure.Delegate
        };
    
    /// <summary>
    /// Converts a closure to an anonymous closure.
    /// </summary>
    /// <param name="closure">The closure to make anonymous.</param>
    /// <typeparam name="TContext">The type of context used.</typeparam>
    /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
    /// <typeparam name="TClosure">The type of closure to convert.</typeparam>
    /// <returns>An <see cref="AnonymousClosure{TContext,TDelegate}"/></returns>
    public static AnonymousClosure<TContext, TDelegate> ConvertToAnonymous<TContext, TDelegate, TClosure>(TClosure closure) 
        where TDelegate : Delegate 
        where TClosure :  IClosure<TContext, TDelegate>
        => new AnonymousClosure<TContext, TDelegate> {
            Context = closure.Context,
            Delegate = closure.Delegate
        };
    
    public static TConvertedClosure Convert<TContext, TDelegate, TConvertedClosure>(AnonymousClosure<TContext, TDelegate> anonymousClosure) 
        where TConvertedClosure : struct, IClosure<TContext, TDelegate>
        where TDelegate : Delegate 
        => new TConvertedClosure() {
            Context = anonymousClosure.Context,
            Delegate = anonymousClosure.Delegate
        };

    public static bool TryConvert<TConvertedClosure>(IAnonymousClosure anonymousClosure, out TConvertedClosure convertedClosure, ClosureTypeResolver? resolver = null) 
        where TConvertedClosure : struct, IClosure {
        
        var anonymousClosureType = anonymousClosure.GetType();
        
        // Get the context and delegate properties from the closure
        var contextProperty = anonymousClosureType.GetProperty("Context")!;
        var delegateProperty = anonymousClosureType.GetProperty("Delegate")!;
        
        resolver ??= s_defaultResolver; // TODO: remove resolving since we know the type at compile time
        var conversionType = resolver.Resolve(delegateProperty.PropertyType);
        
        if (typeof(TConvertedClosure).IsAssignableFrom(conversionType)) {
            var context = contextProperty.GetValue(anonymousClosure, null);
            var @delegate = delegateProperty.GetValue(anonymousClosure, null);
            
            object boxed = new TConvertedClosure();
            var closureType = typeof(TConvertedClosure);
            closureType.GetProperty("Context")!.SetValue(boxed, context, null);
            closureType.GetProperty("Delegate")!.SetValue(boxed, @delegate, null);

            convertedClosure = (TConvertedClosure)boxed;
            
            return true;
        }
        
        convertedClosure = default;
        return false;
    }
    
    public static bool TryConvert<TContext, TDelegate>(AnonymousClosure<TContext, TDelegate> anonymousClosure, out IClosure<TContext, TDelegate> convertedClosure, ClosureTypeResolver? resolver = null) 
        where TDelegate : Delegate {
        
        convertedClosure = new AnonymousClosure<TContext, TDelegate>(anonymousClosure.Context, anonymousClosure.Delegate);
        
        if (!TryConvert<AnonymousClosure<TContext, TDelegate>>(anonymousClosure, out var anonymousToIClosure, resolver))
            return false;

        if (anonymousToIClosure is not IClosure<TContext, TDelegate> converted)
            return false;
        
        convertedClosure = converted;
        return true;
    }

    static bool TryConvert<TAnonymousClosure>(TAnonymousClosure closure, out IClosure? convertedClosure, ClosureTypeResolver? resolver = null)
        where TAnonymousClosure : struct, IAnonymousClosure {
        
        // Get the context and delegate properties from the closure
        var contextProperty = typeof(TAnonymousClosure).GetProperty("Context")!;
        var delegateProperty = typeof(TAnonymousClosure).GetProperty("Delegate")!;
        
        var contextType = contextProperty.PropertyType;
        var delegateType = delegateProperty.PropertyType;
        
        resolver ??= s_defaultResolver;
        var conversionType = resolver.Resolve(delegateType);

        if (typeof(IAnonymousClosure).IsAssignableFrom(conversionType)) {
            convertedClosure = null;
            return false;
        }
        
        var method = typeof(Closure)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                    m is { Name: nameof(Closure.Create), IsGenericMethodDefinition: true }
                    && m.GetGenericArguments().Length == 3
                    && m.GetParameters().Length == 2 // or 3 for the overload with mutatingBehaviour
            );

#if DEBUG
            if (method == null) {
                throw new InvalidOperationException("Could not find the Create method in Closure.");
            }
#else
            if (method == null) {
                convertedClosure = null;
                return false;
            }
#endif
        
        var context = contextProperty.GetValue(closure, null);
        var @delegate = delegateProperty.GetValue(closure, null);

        var genericMethod = method.MakeGenericMethod(contextType, delegateType, conversionType);
        convertedClosure = (IClosure?)genericMethod.Invoke(null, [context, @delegate]);
        
        return convertedClosure != null;
    }
}