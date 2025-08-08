namespace Closures.Converting {
    public static class ClosureConverter {
        /// <summary>
        /// Converts a closure to an anonymous closure.
        /// </summary>
        /// <param name="closure">The closure to make anonymous.</param>
        /// <typeparam name="TContext">The type of context used.</typeparam>
        /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
        /// <returns>An <see cref="CustomClosure{TContext,TDelegate}"/> containing the same context and delegate as the input closure.</returns>
        /// <remarks>
        /// This will box the closure as it has to convert to <see cref="IClosure{TContext, TDelegate}"/> from a value type. <br/>
        /// Specify the type of closure you are converting to avoid this <see cref="ConvertToAnonymous{TContext, TDelegate, TClosure}"/>.
        /// </remarks>
        public static CustomClosure<TContext, TDelegate> ConvertToAnonymous<TContext, TDelegate>(IClosure<TContext, TDelegate> closure) 
            where TDelegate : Delegate 
            => new CustomClosure<TContext, TDelegate> {
                Context = closure.Context,
                Delegate = closure.Delegate
            };
    
        /// <summary>
        /// Converts a strongly-typed closure to an anonymous closure.
        /// </summary>
        /// <param name="closure">The strongly-typed closure to convert.</param>
        /// <typeparam name="TContext">The type of context used.</typeparam>
        /// <typeparam name="TDelegate">The type of delegate used.</typeparam>
        /// <typeparam name="TClosure">The type of the strongly-typed closure to convert.</typeparam>
        /// <returns>An <see cref="CustomClosure{TContext,TDelegate}"/> containing the same context and delegate as the input closure.</returns>
        /// <remarks>
        /// This overload avoids boxing by specifying the closure type explicitly.
        /// </remarks>
        public static CustomClosure<TContext, TDelegate> ConvertToAnonymous<TContext, TDelegate, TClosure>(TClosure closure) 
            where TDelegate : Delegate 
            where TClosure :  IClosure<TContext, TDelegate>
            => new CustomClosure<TContext, TDelegate> {
                Context = closure.Context,
                Delegate = closure.Delegate
            };
    
        /// <summary>
        /// Converts an <see cref="CustomClosure{TContext,TDelegate}"/> to a strongly-typed closure of the specified type.
        /// </summary>
        /// <typeparam name="TContext">The type of the context used in the closure.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate used in the closure.</typeparam>
        /// <typeparam name="TConvertedClosure">The type of the closure to convert to. Must be a struct implementing <see cref="IClosure{TContext, TDelegate}"/>.</typeparam>
        /// <param name="customClosure">The anonymous closure to convert.</param>
        /// <returns>A closure of type <typeparamref name="TConvertedClosure"/> with the same context and delegate as the anonymous closure.</returns>
        /// <remarks>
        /// This method is useful for converting an <see cref="CustomClosure{TContext,TDelegate}"/> back to a known closure type, preserving the context and delegate.
        /// </remarks>
        public static TConvertedClosure Convert<TContext, TDelegate, TConvertedClosure>(CustomClosure<TContext, TDelegate> customClosure) 
            where TConvertedClosure : struct, IClosure<TContext, TDelegate>
            where TDelegate : Delegate 
            => new TConvertedClosure() {
                Context = customClosure.Context,
                Delegate = customClosure.Delegate
            };
    }
}