using System.Reflection;

namespace Closures.Converting.Experimental {
    /// <summary>
    /// Reflection based converter for anonymous closures.
    /// </summary>
    /// <remarks>
    /// <b>Warning:</b> This is an experimental feature and may not be fully optimized or stable.
    /// It is also possible for this to receive major changes or be removed / renamed in the future causing breaking changes.
    /// Use with caution.
    /// </remarks>
    [Obsolete("Experimental feature, may change or be removed in the future.")]
    public static class ReflectionClosureConverter {
        static ClosureTypeResolver? s_defaultResolver;
        static ClosureTypeResolver DefaultResolver => s_defaultResolver ??= ClosureTypeResolver.DefaultResolver;
    
        public static void SetDefaultClosureTypeResolver(ClosureTypeResolver resolver) => s_defaultResolver = resolver;

        public static bool TryConvert<TConvertedClosure>(IAnonymousClosure anonymousClosure, out TConvertedClosure convertedClosure, ClosureTypeResolver? resolver = null) 
            where TConvertedClosure : struct, IClosure {
        
            var anonymousClosureType = anonymousClosure.GetType();
        
            // Get the context and delegate properties from the closure
            var contextProperty = anonymousClosureType.GetProperty("Context")!;
            var delegateProperty = anonymousClosureType.GetProperty("Delegate")!;
        
            resolver ??= DefaultResolver; // TODO: remove resolving since we know the type at compile time
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
        
            resolver ??= DefaultResolver;
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
            convertedClosure = (IClosure?)genericMethod.Invoke(null, new []{context, @delegate});
        
            return convertedClosure != null;
        }
    }
}