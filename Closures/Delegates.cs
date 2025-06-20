namespace Closures;

/// <summary> Represents a delegate that operates on a value by reference. </summary>
public delegate void RefAction<TArg>(ref TArg value);
/// <summary> Represents a delegate that operates on a value by reference. </summary>
public delegate void RefAction<TArg1, TArg2>(ref TArg1 value1, ref TArg2 value2);

/// <summary> Represents a delegate that operates on a value by reference with a context. </summary>
public delegate void RefActionWithNormalContext<in TContext, TArg>(TContext context, ref TArg value);

/// <summary> Represents a delegate that operates on a context by reference and takes in an argument. </summary>
public delegate void ActionWithRefContext<TContext, in TArg>(ref TContext context, TArg value);

/// <summary> Represents a delegate that returns a result and operates on a value by reference. </summary>
public delegate TResult RefFunc<TArg, out TResult>(ref TArg value);

/// <summary> Represents a delegate that returns a result and operates on a value by reference with a context. </summary>
public delegate TResult RefFuncWithContext<in TContext, TArg, out TResult>(TContext context, ref TArg value);
