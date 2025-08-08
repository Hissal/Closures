using System.Reflection;

namespace Closures.Converting.Experimental {
    public enum ClosureDelegateType {
        Unknown,
        Action,
        Func,
    }

    /// <summary>
    /// Reflection based type resolver for resolving the type of closure from a delegate type
    /// </summary>
    /// <remarks>
    /// <b>Warning:</b> This is an experimental feature and may not be fully optimized or stable.
    /// It is also possible for this to receive major changes or be removed / renamed in the future causing breaking changes.
    /// Use with caution.
    /// </remarks>
    [Obsolete("Experimental feature, may change or be removed in the future.")]
    public class ClosureTypeResolver {
        static ClosureTypeResolver? s_defaultResolver;
        public static ClosureTypeResolver DefaultResolver => s_defaultResolver ??= new ClosureTypeResolver();

    
        readonly Dictionary<Type, Type> delegateToClosureType = new ();
    
        public ClosureTypeResolver AddResolution<TDelegate, TClosure>() where TDelegate : Delegate where TClosure : IClosure => 
            AddResolution(typeof(TDelegate), typeof(TClosure));
        public ClosureTypeResolver AddResolution(Type delegateType, Type closureType) {
            if (!delegateToClosureType.TryAdd(delegateType, closureType))
                throw new InvalidOperationException($"Delegate type {delegateType} is already registered.");

            return this;
        }

        public Type Resolve<TDelegate>() where TDelegate : Delegate =>
            Resolve(typeof(TDelegate));
    
        public Type Resolve(Type delegateType) =>
            delegateToClosureType.TryGetValue(delegateType, out var type)
                ? type
                : ResolveType(delegateType);

        public static Type ResolveType<TDelegate>() {
            return ResolveType(typeof(TDelegate));
        }
    
        public static Type ResolveType(Type delegateType) {
            if (!delegateType.IsGenericType)
                return ResolveUnknownType(delegateType);

            return ResolveClosureDelegateType(delegateType) switch {
                ClosureDelegateType.Action => ResolveActionType(delegateType),
                ClosureDelegateType.Func => ResolveFuncType(delegateType),
                _ => ResolveUnknownType(delegateType)
            };
        }
    
        public static Type ResolveActionType<TDelegate>() where TDelegate : Delegate => 
            ResolveActionType(typeof(TDelegate));

        public static Type ResolveActionType(Type delegateType) {
            var genericArgs = delegateType.GetGenericArguments();
        
            if (genericArgs.Length == 0)
                throw new ArgumentException("Delegate type must have at least one generic argument.");
        
            var contextType = genericArgs[0];
            var argType = genericArgs.Length > 1 ? genericArgs[1] : null;
        
            if (contextType == null || delegateType == null)
                throw new ArgumentNullException(nameof(contextType), "delegate type and context (first argument) of the delegate type must not be null.");
        
            var genericArgumentCount = argType is null ? 2 : 3;
        
            // Check if the delegate has an argument
            var method = typeof(ClosureTypeResolver).GetMethod(nameof(ResolveActionType),
                genericArgumentCount,
                BindingFlags.Public | BindingFlags.Static, 
                null, 
                Type.EmptyTypes, 
                null
            );

#if DEBUG
            if (method is null) {
                throw new InvalidOperationException($"Method ResolveClosureActionType with {genericArgumentCount} generic type arguments not found.");
            }
#else
        if (method is null) 
            return ResolveUnknownType(delegateType);
#endif
        
            var genericMethod = argType != null 
                ? method.MakeGenericMethod(contextType, argType, delegateType)
                : method.MakeGenericMethod(contextType, delegateType);
        
            var type = (Type?)genericMethod.Invoke(null, null);
        
            return type ?? ResolveUnknownType(delegateType);
        }
    
        public static Type ResolveFuncType<TDelegate>() where TDelegate : Delegate => 
            ResolveFuncType(typeof(TDelegate));
    
        public static Type ResolveFuncType(Type delegateType) {
            var genericArgs = delegateType.GetGenericArguments();
        
            if (genericArgs.Length <= 1)
                throw new ArgumentException("Delegate type must have at least two generic arguments to qualify as a func closure.");
        
            var contextType = genericArgs[0];
            var argType = genericArgs.Length > 2 ? genericArgs[1] : null;
            var resultType = genericArgs[^1];
        
            if (contextType == null || resultType == null ||  delegateType == null)
                throw new ArgumentNullException(nameof(contextType), "delegate type and context / result (first / last argument) of the delegate type must not be null.");
        
            var genericArgumentCount = argType is null ? 3 : 4;
        
            // Check if the delegate has an argument
            var method = typeof(ClosureTypeResolver).GetMethod(nameof(ResolveFuncType),
                genericArgumentCount,
                BindingFlags.Public | BindingFlags.Static, 
                null, 
                Type.EmptyTypes, 
                null
            );

#if DEBUG
            if (method is null) {
                throw new InvalidOperationException($"Method ResolveClosureFuncType with {genericArgumentCount} generic type arguments not found.");
            }
#else
        if (method is null) 
            return ResolveUnknownType(delegateType);
#endif
        
            var genericMethod = argType != null 
                ? method.MakeGenericMethod(contextType, argType, resultType, delegateType)
                : method.MakeGenericMethod(contextType, resultType, delegateType);
        
            var type = (Type?)genericMethod.Invoke(null, null);
        
            return type ?? ResolveUnknownType(delegateType);
        }

        public static Type ResolveActionType<TContext, TDelegate>() where TDelegate : Delegate {
            if (typeof(TDelegate).IsGenericType is false)
                return typeof(CustomClosure<TContext, TDelegate>);

            return typeof(TDelegate) switch {
                { } type when type.GetGenericTypeDefinition() == typeof(Action<>) => typeof(ClosureAction<TContext>),
                { } type when type.GetGenericTypeDefinition() == typeof(RefAction<>) => typeof(MutatingClosureAction<TContext>),
                _ => typeof(CustomClosure<TContext, TDelegate>)
            };
        }

        public static Type ResolveActionType<TContext, TArg, TDelegate>() where TDelegate : Delegate {
            if (typeof(TDelegate).IsGenericType is false)
                return typeof(CustomClosure<TContext, TDelegate>);
        
            return typeof(TDelegate) switch {
                { } type when type.GetGenericTypeDefinition() == typeof(Action<,>) => typeof(ClosureAction<TContext, TArg>),
                { } type when type.GetGenericTypeDefinition() == typeof(RefActionWithNormalContext<,>) => typeof(ClosureRefAction<TContext, TArg>),
                { } type when type.GetGenericTypeDefinition() == typeof(ActionWithRefContext<,>) => typeof(MutatingClosureAction<TContext, TArg>),
                { } type when type.GetGenericTypeDefinition() == typeof(RefAction<,>) => typeof(MutatingClosureRefAction<TContext, TArg>),
                _ => typeof(CustomClosure<TContext, TDelegate>)
            };
        }
    
        public static Type ResolveFuncType<TContext, TResult, TDelegate>() where TDelegate : Delegate {
            if (typeof(TDelegate).IsGenericType is false)
                return typeof(CustomClosure<TContext, TDelegate>);
        
            return typeof(TDelegate) switch {
                { } type when type.GetGenericTypeDefinition() == typeof(Func<,>) => typeof(ClosureFunc<TContext, TResult>),
                { } type when type.GetGenericTypeDefinition() == typeof(RefFunc<,>) => typeof(MutatingClosureFunc<TContext, TResult>),
                _ => typeof(CustomClosure<TContext, TDelegate>)
            };
        }
    
        public static Type ResolveFuncType<TContext, TArg, TResult, TDelegate>() where TDelegate : Delegate {
            if (typeof(TDelegate).IsGenericType is false)
                return typeof(CustomClosure<TContext, TDelegate>);
        
            return typeof(TDelegate) switch {
                { } type when type.GetGenericTypeDefinition() == typeof(Func<,,>) => typeof(ClosureFunc<TContext, TArg, TResult>),
                { } type when type.GetGenericTypeDefinition() == typeof(RefFuncWithNormalContext<,,>) => typeof(ClosureRefFunc<TContext, TArg, TResult>),
                { } type when type.GetGenericTypeDefinition() == typeof(FuncWithRefContext<,,>) => typeof(MutatingClosureFunc<TContext, TArg, TResult>),
                { } type when type.GetGenericTypeDefinition() == typeof(RefFunc<,,>) => typeof(MutatingClosureRefFunc<TContext, TArg, TResult>),
                _ => typeof(CustomClosure<TContext, TDelegate>)
            };
        }
    
        public static Type ResolveUnknownType<TDelegate>() where TDelegate : Delegate => 
            ResolveUnknownType(typeof(TDelegate));

        public static Type ResolveUnknownType(Type delegateType) {
            var genericArgs = delegateType.GetGenericArguments();
        
            if (!delegateType.IsGenericType || genericArgs.Length == 0)
#if DEBUG
                throw new ArgumentException("Delegate type must be a generic type with atleast one generic argument", nameof(delegateType));
#else
            return typeof(CustomClosure<,>).MakeGenericType(typeof(object), delegateType);
#endif
            return typeof(CustomClosure<,>).MakeGenericType(genericArgs[0], delegateType);
        }
    
        public static ClosureDelegateType ResolveClosureDelegateType<TDelegate>() where TDelegate : Delegate => 
            ResolveClosureDelegateType(typeof(TDelegate));

        public static ClosureDelegateType ResolveClosureDelegateType(Type delegateType) {
            if (!delegateType.IsGenericType)
                return ClosureDelegateType.Unknown;
        
            var genericTypeDefinition = delegateType.GetGenericTypeDefinition();
        
            return genericTypeDefinition switch {
                not null when genericTypeDefinition == typeof(Action<>) => ClosureDelegateType.Action,
                not null when genericTypeDefinition == typeof(Action<,>) => ClosureDelegateType.Action,
                not null when genericTypeDefinition == typeof(RefAction<>) => ClosureDelegateType.Action,
                not null when genericTypeDefinition == typeof(RefAction<,>) => ClosureDelegateType.Action,
                not null when genericTypeDefinition == typeof(ActionWithRefContext<,>) => ClosureDelegateType.Action,
                not null when genericTypeDefinition == typeof(RefActionWithNormalContext<,>) => ClosureDelegateType.Action,

                not null when genericTypeDefinition == typeof(Func<,>) => ClosureDelegateType.Func,
                not null when genericTypeDefinition == typeof(Func<,,>) => ClosureDelegateType.Func,
                not null when genericTypeDefinition == typeof(RefFunc<,>) => ClosureDelegateType.Func,
                not null when genericTypeDefinition == typeof(RefFunc<,,>) => ClosureDelegateType.Func,
                not null when genericTypeDefinition == typeof(FuncWithRefContext<,,>) => ClosureDelegateType.Func,
                not null when genericTypeDefinition == typeof(RefFuncWithNormalContext<,,>) => ClosureDelegateType.Func,
            
                _ => ClosureDelegateType.Unknown
            };
        }
    
        public static Type ResolveContextType<TDelegate>() where TDelegate : Delegate =>
            ResolveContextType(typeof(TDelegate));

        public static Type ResolveContextType(Type delegateType) {
            if (delegateType.IsGenericType)
                throw new ArgumentException("Delegate type must be generic to resolve context type.");
        
            var genericArgs = delegateType.GetGenericArguments();
        
            if (genericArgs.Length == 0)
                throw new ArgumentException("Delegate type must have at least one generic argument to resolve context type.");
        
            return genericArgs[0];
        }

        public static bool IsOfType<TClosure>(ICustomClosure customClosure) => 
            IsOfType(typeof(TClosure), customClosure);

        public static bool IsOfType(Type closureType, ICustomClosure customClosure,  ClosureTypeResolver? resolver = null) {
            var delegateType = customClosure.GetType().GetProperty("Delegate")?.PropertyType!;

            resolver ??= DefaultResolver;
            var type = resolver.Resolve(delegateType);
            return closureType.IsAssignableFrom(type);
        }
    }
}