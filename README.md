# Closures
Closure structs that capture a context and a delegate that is invoked with the given context.

```csharp
using Closures;

// Captures 100 as context and prints it when invoked
var closure = Closure.Action(100, 
    (context) => Console.WriteLine($"Captured context {context})");
    
closure.Invoke(); // Output: Captured context 100
```
## Installation
You can install the Closures library via NuGet Package Manager:

```bash
Install-Package Closures
```
Or by using the .NET CLI:

```bash
dotnet add package Closures
```

## Benchmark Results

| Method        |      Mean |     Error |    StdDev | Allocated |
|---------------|----------:|----------:|----------:|----------:|
| Action        | 6.7512 ns | 0.0819 ns | 0.0726 ns |      88 B |
| ClosureAction | 0.0103 ns | 0.0046 ns | 0.0043 ns |         - |

With the benchmark results we can see that using closures is significantly faster than using regular actions or functions that capture variables in a lambda expression.

The closure struct has a negligible allocation overhead compared to the action that captures variables in a lambda expression.
This is due to the lambda expressions having to box the captured variables and allocate them on the heap,
while the closure struct captures the context by value and does not require heap allocation.

On top of this since the lambda is capturing the variable it has to allocate a new delegate instance every time it is created
while the closure struct allows for a stateless delegates making it a static method behind the scenes.

## Table of Contents
- [Basic Closures](#basic-closures)
- [Mutating Closures](#mutating-closures)
- [Ref Closures](#ref-closures)
- [Anonymous Closures](#anonymous-closures)
- [Custom Closure](#custom-closure)
- [Converting to Delegates](#converting-to-delegates)
- [Cache Management](#cache-management)
- [Closure Types](#closure-types)
- [Example Scenarios](#example-scenarios)

Closures Explained: [Closures | In 210 Seconds](https://youtu.be/jHd0FczIjAE?si=5slaULcQxYZN3EES)
<br>
Video demonstration of a similar concept: [Fix Closure Issues in 10 Minutes and Boost Performance](https://youtu.be/xiz24OqwEVI?si=gUapklV8JF0FaLTm)

## Usage
To use the closures, simply create an instance of the desired closure type using the `Closure.Action` or `Closure.Func` methods, 
passing in the context and the delegate to be invoked. The closures can then be invoked like a delegate.

### Basic closures
- `ClosureAction<TContext>` that captures a string context.
- `ClosureFunc<TContext, TReturn>` that captures an int context and returns a doubled value.

```csharp
// Example of ClosureAction
var closureAction = Closure.Action("captured context", 
    (string context) => Console.WriteLine($"Action with context - {context}"));

closureAction.Invoke(); // Output: Action with context - captured context

// Example of ClosureFunc
var closureFunc = Closure.Func(10, (int context) => context * 2);
Console.WriteLine(closureFunc.Invoke()); // Output: 20
```

### Passing an argument
Closures can also take in an argument when invoking them.
The context is always passed as the first parameter, followed by the argument.
- `ClosureAction<TContext, TArg>` that captures an int context and is called with an int argument.
- `ClosureFunc<TContext, TArg, TResult>` that captures an int context, is called with an int argument and returns the sum of the context and argument.

```csharp 
// Example of ClosureAction with an argument
var closureActionWithArg = Closure.Action(10, (int context, int arg) => 
    Console.WriteLine($"Context: {context}, Arg: {arg}"));

closureActionWithArg.Invoke(5); // Output: Context: 10, Arg: 5
    
// Example of ClosureFunc with an argument
var closureFuncWithArg = Closure.Func(10, (int context, int arg) => context + arg);
Console.WriteLine(closureFuncWithArg.Invoke(5)); // Output: 15
```

Arguments can also be passed as `ref`, allowing the closure to modify the argument value.
- `ClosureRefAction<TContext, TArg>` that captures an int context and is called with a ref int argument which is set to the value of context.
- `ClosureRefFunc<TContext, TArg, TResult>` that captures an int context, is called with a ref int argument which is set to the value of context and returns the sum of the context and argument.

```csharp
// Example of ClosureAction with a ref argument
int arg = 5;
var closureActionWithRefArg = Closure.RefAction(10, (int context, ref int arg) => arg = context);
closureActionWithRefArg.Invoke(ref arg);
Console.WriteLine(arg); // Output: 10
    
// Example of ClosureFunc with a ref argument
var funcArg = 5;
var closureFuncWithRefArg = Closure.RefFunc(10, (int context, ref int arg) => {
    var sum = context + arg;
    arg = context;
    return sum;
});
var returnValue = closureFuncWithRefArg.Invoke(ref funcArg);
Console.WriteLine(returnValue); // Output: 15
Console.WriteLine(funcArg); // Output: 10
```

### Mutating closures
Mutating closures allow you to modify the captured context within the delegate. 
They are useful when you want to invoke the closure multiple times with the context being modified each time.

- `MutatingClosureAction<TContext>` that captures a mutable context.
- `MutatingClosureFunc<TContext, TReturn>` that captures a mutable context and returns the sum of modifications.

```csharp
// Example of MutatingClosureAction
var mutatingClosureAction = MutatingClosure.Action(10, (ref int context) => {
    Console.WriteLine(context)
    context += 5
});
mutatingClosureAction.Invoke(); // Output: 10
mutatingClosureAction.Invoke(); // Output: 15

// Example of MutatingClosureFunc
var mutatingClosureFunc = Closure.Func(20, (ref int context) => context *= 2);
Console.WriteLine(mutatingClosureFunc.Invoke()); // Output: 40
Console.WriteLine(mutatingClosureFunc.Invoke()); // Output: 80
```

### Ref closures
Ref closures work similarly to mutating closures with the difference being that 
they are ref structs that capture a reference to a context variable, allowing it to modify the original variable.

- `RefClosureAction<TContext>` that captures a reference to a mutable context.
- `RefClosureFunc<TContext, TReturn>` that captures a reference to a mutable context and returns a modified value.

```csharp
// Example of RefActionClosure
var refContext = 10;
var refClosureAction = RefClosure.Action(ref refContext, (ref int context) => {
    Console.WriteLine(context);
    context += 10;
});

refClosureAction.Invoke(); // Output: 10
Console.WriteLine(refContext); // Output: 20

refClosureAction.Invoke(); // Output: 20
Console.WriteLine(refContext); // Output: 30

// Example of RefFuncClosure
var refFuncContext = 10;
var refClosureFunc = RefClosure.Func(ref refFuncContext, (ref int context) => {
    context *= 2;
    return context * 2;
});

Console.WriteLine(refClosureFunc.Invoke()); // Output: 40
Console.WriteLine(refFuncContext); // Output: 20

Console.WriteLine(refClosureFunc.Invoke()); // Output: 80
Console.WriteLine(refFuncContext); // Output: 40
```

### Anonymous closures

Anonymous closures provide a type-erased, flexible way to store and invoke closures (delegates with captured context) without knowing the exact context or delegate type at compile time. They are useful when you need to work with closures in a generic or dynamic fashion, such as storing heterogeneous closures in collections, passing them through APIs, or bridging between different closure types.

Anonymous closures encapsulate:
- A delegate (action or function, with or without arguments)
- An `AnonymousValue` context (a type-erased container for any value or reference type)
- A `MutatingBehaviour` (controls whether the context is mutated or reset after invocation)

#### Types

- `AnonymousClosure`: The core type-erased closure struct. It can represent any closure (action or function, with or without arguments or return value). It exposes generic `Invoke` and `TryInvoke` methods for all supported delegate signatures.
- `AnonymousClosureAction`: Represents an anonymous closure for a delegate with no arguments and no return value (action).
- `AnonymousClosureAction<TArg>`: Represents an anonymous closure for a delegate with a single argument and no return value.
- `AnonymousClosureFunc<TReturn>`: Represents an anonymous closure for a delegate with no arguments and a return value.
- `AnonymousClosureFunc<TArg, TReturn>`: Represents an anonymous closure for a delegate with a single argument and a return value.

All anonymous closure types implement the `IAnonymousClosure` interface, which provides access to the context, delegate, and mutating behaviour, as well as:
- `Is<TClosureType>()`: Checks if the anonymous closure can be converted to a specific strongly-typed closure type.
- `InvokableAs<TDelegate>()`: Checks if the anonymous closure can be invoked as if it was a delegate of type `TDelegate`.

#### Mutating Behaviour

The `MutatingBehaviour` enum controls whether the context is mutated (`Mutate`) or reset (`Reset`) after invocation. This allows you to choose between closures that modify their captured context (like mutating closures) and those that do not.

#### Usage

You can create anonymous closures using the static factory methods on `AnonymousClosure`, such as:

```csharp
// Create an anonymous action closure
var anonAction = AnonymousClosure.Action(AnonymousValue.From(42), (int ctx) => Console.WriteLine(ctx));

// Create an anonymous function closure with a return value
var anonFunc = AnonymousClosure.Func<int>(AnonymousValue.From("hello"), (string ctx) => ctx.Length);

// Create an anonymous closure from a strongly-typed closure
var closure = Closure.Action(100, (int ctx) => Console.WriteLine(ctx));
AnonymousClosure anon = closure.AsAnonymous();
```

You can invoke anonymous closures using their `Invoke` or `TryInvoke` methods. The `TryInvoke` methods return a `Result` or `Result<T>` indicating success or failure, and allow you to specify an `ExceptionHandlingPolicy` to control how exceptions are handled.

```csharp
anonAction.Invoke(); // Invokes the action
var result = anonFunc.TryInvoke(); // Returns Result<int>
if (result.IsSuccess) {
    Console.WriteLine(result.Value);
}
```

#### Type Checking and Invocation

You can use `Is<TClosureType>()` to check if an anonymous closure can be converted to a specific strongly-typed closure type, and then convert it back using the appropriate extension method:

You can use `InvokableAs<TDelegate>()` to check if an anonymous closure can be invoked as if it was a specific delegate type.

Anonymous closures can be converted back to strongly-typed closures (if the types match) using extension methods such as `AsClosureAction<TContext>()` or `AsClosureFunc<TContext, TResult>()`.

Example:

```csharp
// Store different closures in a single list
var list = new List<AnonymousClosure>();
list.Add(Closure.Action(1, (int ctx) => Console.WriteLine(ctx)).AsAnonymous());
list.Add(Closure.Func("abc", (string ctx) => ctx.Length).AsAnonymous());

foreach (var anon in list) {
    if (anon.InvokableAs<Func<int>>())
        Console.WriteLine(anon.Invoke<int>());
    else if (anon.InvokableAs<Action>())
        anon.Invoke();
}

// Example: Checking type and converting back to a strongly-typed closure
var anonClosure = Closure.Func("hello", (string ctx) => ctx.Length).AsAnonymous();
if (anonClosure.Is<ClosureFunc<string, int>>()) {
    var typed = anonClosure.AsClosureFunc<string, int>();
    Console.WriteLine(typed.Invoke()); // Output: 5
}
```

#### Remarks

- Anonymous closures are type-erased and do not enforce type safety at compile time. You must ensure that the delegate and context types are compatible when converting or invoking.
- Useful for storing closures of different types in a single collection, or for APIs that need to accept arbitrary closures.

With the flexibility comes some performance overhead compared to strongly-typed closures, so use them judiciously in performance-critical scenarios.
If possible converting back to strongly-typed closures is recommended.

| Method                    |       Mean |     Error |    StdDev | Allocated |
|---------------------------|-----------:|----------:|----------:|----------:|
| Action                    |  4.7119 ns | 0.0502 ns | 0.0445 ns |      88 B |
| ClosureAction             |  0.0221 ns | 0.0044 ns | 0.0039 ns |         - |
| ClosureAction_AsAnonymous | 25.8292 ns | 0.0883 ns | 0.0783 ns |         - |

### Custom Closure
You can use `Closure.Custom<TContext, TDelegate>(TContext, TDelegate)` to create a closure with a custom delegate type.
This allows you to define your own delegate type and use it with the closure.
However, you must call the delegate manually using the `Delegate` property and pass the context and arguments as needed.
Essentially this just wraps the delegate and context in a struct, allowing you to use any delegate type with the closure.
```csharp
delegate void CustomDelegate(int context, string message, ref int mutatableInt);

var customClosure = Closure.Custom<int, CustomDelegate>(100, 
    (int context, string message, ref int mutatableInt) => {
    Console.WriteLine($"Context: {context}, message: {message}, mutatableInt: {mutatableInt}");
    mutatableInt = context;
}));
    
int mutatableInt = 1;
customClosure.Delegate.Invoke(customClosure.Context, "Hello World!", ref mutatableInt); 
    // Output: Context: 100, message: Hello World!, mutatableInt: 1
    
Console.WriteLine(mutatableInt); // Output: 100
```

## Converting to delegates

Closures can be easily converted to standard .NET delegates (such as `Action` or `Func`) using the provided extension methods.

You can convert a closure to a delegate using the `AsAction()` or `AsFunc()` extension methods:

```csharp
var closure = Closure.Action(42, (int ctx) => Console.WriteLine(ctx));
Action action = closure.AsAction();
action.Invoke(); // Output: 42

var closureFunc = Closure.Func("abc", (string ctx) => ctx.Length);
Func<int> func = closureFunc.AsFunc();
Console.WriteLine(func.Invoke()); // Output: 3

// You can also convert closures with arguments:
var closureWithArg = Closure.Action(10, (int ctx, int arg) => 
    Console.WriteLine($"Context: {ctx}, Arg: {arg}"));

Action<int> actionWithArg = closureWithArg.AsAction<int>();
actionWithArg.Invoke(5); // Output: Context: 10, Arg: 5
```

However, there is a performance overhead when converting closures to delegates, as it involves boxing the closure and creating a delegate instance.
This is only done once for each closure and then the "invoker" delegate is cached for subsequent invocations.
This means that doing this only once is not better than creating a normal action that captures the context.

Where can be useful is for example when you want to pass a callback to a method that expects a delegate and need to capture some context.
In this case, you can create a closure and convert it to a delegate, which will be cached for subsequent conversions,
making it essentially allocation free.

| Method                 |     Mean |     Error |    StdDev | Allocated |
|------------------------|---------:|----------:|----------:|----------:|
| Action                 | 4.850 ns | 0.0602 ns | 0.0563 ns |      88 B |
| ClosureAction_AsAction | 8.985 ns | 0.0445 ns | 0.0395 ns |         - |

## Cache Management

The `ClosureManager` class provides a static method `ClearCache()` that clears all internal caches used by the Closures system.
This can help reduce memory usage in scenarios where many closures, invokers and delegate conversions have been cached but are no longer relevant.
For example, in a video game, you might want to call `ClosureManager.ClearCache()` during a major loading screen or scene transition, 
when much of the code and its associated closures are about to change.

```csharp
// Clear all closure-related caches to free up memory
ClosureManager.ClearCache();
```

## Closure Types
Closures are categorized into several types based on their functionality and usage patterns. Below is a summary of the different closure types available in the Closures library:

### Action Closures
- `ClosureAction<TContext>`: Captures a context of type `TContext` and invokes an action with that context.
   <br><br>
- `ClosureAction<TContext, TArg>`: Captures a context of type `TContext` and invokes an action with an argument of type `TArg`.
   <br><br>
- `ClosureRefAction<TContext, TArg>`: Captures a context of type `TContext` and invokes an action with a ref argument of type `TArg`.
   <br><br>
- `MutatingClosureAction<TContext>`: Captures a context of type `TContext` by value and invokes a ref action that can mutate the stored context.
  <br><br>
- `MutatingClosureAction<TContext, TArg>`: Captures a context of type `TContext` by value and invokes an action with an argument, allowing mutation of the stored context.
  <br><br>
- `MutatingClosureRefAction<TContext, TArg>`: Captures a context of type `TContext` by value and invokes a ref action with a ref argument, allowing mutation of both context and argument.
  <br><br>
- `RefClosureAction<TContext>`: Captures a reference to a context variable of type `TContext` and invokes a ref action, mutating the original variable.
  <br><br>
- `RefClosureAction<TContext, TArg>`: Captures a reference to a context variable and invokes an action with an argument, mutating the original variable.
  <br><br>
- `RefClosureRefAction<TContext, TArg>`: Captures a reference to a context variable and invokes a ref action with a ref argument, mutating both.

### Function Closures
- `ClosureFunc<TContext, TResult>`: Captures a context of type `TContext` and invokes a function returning `TResult`.
  <br><br>
- `ClosureFunc<TContext, TArg, TResult>`: Captures a context of type `TContext` and invokes a function with an argument, returning `TResult`.
  <br><br>
- `ClosureRefFunc<TContext, TArg, TResult>`: Captures a context of type `TContext` and invokes a function with a ref argument, returning `TResult`.
  <br><br>
- `MutatingClosureFunc<TContext, TResult>`: Captures a context of type `TContext` by value and invokes a ref function, allowing mutation of the stored context.
  <br><br>
- `MutatingClosureFunc<TContext, TArg, TResult>`: Captures a context of type `TContext` by value and invokes a function with an argument, allowing mutation of the stored context.
  <br><br>
- `MutatingClosureRefFunc<TContext, TArg, TResult>`: Captures a context of type `TContext` by value and invokes a ref function with a ref argument, allowing mutation of both context and argument.
  <br><br>
- `RefClosureFunc<TContext, TResult>`: Captures a reference to a context variable and invokes a ref function, mutating the original variable.
  <br><br>
- `RefClosureFunc<TContext, TArg, TResult>`: Captures a reference to a context variable and invokes a function with an argument, mutating the original variable.
  <br><br>
- `RefClosureRefFunc<TContext, TArg, TResult>`: Captures a reference to a context variable and invokes a ref function with a ref argument, mutating both.

### Anonymous Closures
- `AnonymousClosure`: Represents an anonymous closure that can be used with any delegate type.
<br><br>
- `AnonymousCLosureAction`: Represents an anonymous closure that can be used with a delegate without a return type.
<br><br>
- `AnonymousCLosureAction<TArg>`: Represents an anonymous closure that can be used with a delegate that has an argument without a return type.
<br><br>
- `AnonymousClosureFunc<TReturn>`: Represents an anonymous closure that can be used with a delegate with a return type.
<br><br>
- `AnonymousCLosureFunc<TArg, TReturn>`: Represents an anonymous closure that can be used with a delegate that has an argument with a return type.


### Custom Closure
- `CustomClosure<TContext, TDelegate>`: Represents a custom closure that can be used with any delegate type, allowing you to define your own delegate and use it with the closure.

## Example scenarios

### Unexpected value captured by closure

When creating delegates that capture values, it's important to understand how variable capture works in C#.
When you create a delegate that uses a variable from the outside scope, it boxes it and values get turned to references.
This not only means that it will create a heap allocation for the captured variable,
but also that the variable is captured by reference, not by value. 
If you create an action inside a loop like below it may lead to unexpected behaviour.

The following code might not work as expected:
```csharp
List<Action> actions = new List<Action>();

for (int i = 0; i < 3; i++) {
    var action = new Action(() => Console.WriteLine($"{i}"));
    actions.Add(action);
}

foreach (var action in actions) {
    action.Invoke(); // Output: 3, 3, 3
}
```
You would see the output as `3, 3, 3`
because the variable `i` is captured by reference,
and by the time the actions are invoked,
`i` has already reached its final value of `3`.

To fix this, you can capture the current value of `i` in a separate variable in each iteration:
```csharp
List<Action> actions = new List<Action>();

for (int i = 0; i < 3; i++) {
    var tempI = i; // Capture the current value of i
    var action = new Action(() => Console.WriteLine($"{tempI}"));
    actions.Add(action);
}

foreach (var action in actions) {
    action.Invoke(); // Output: 0, 1, 2
}
```
You would see the output as `0, 1, 2` Though this works, it still allows `tempI` to be captured by the lambda escaping its scope,
creating a heap allocation.

To avoid this, you can use `Closure` structs:
```csharp
List<ClosureAction<int>> actions = new List<ClosureAction<int>>();

for (int i = 0; i < 3; i++) {
    // Create a closure that captures the current value of i
    var action = Closure.Action(i, (context) => Console.WriteLine($"{context}"));
    actions.Add(closure);
}

foreach (var action in actions) {
    action.Invoke(); // Output: 0, 1, 2
}
```
This way, you avoid unnecessary heap allocations when capturing variables,
which in turn reduces garbage collection.

This is especially beneficial in performance-critical scenarios, such as game development,
where minimizing allocations and maximizing efficiency is crucial.

### Callback with context
In scenarios where you need to pass a callback to a method that requires a context, closures can be particularly useful.
You can create a closure that captures the context and then convert it to a delegate to pass it to the method. 
This allows you to avoid heap allocations and maintain stable performance without gc spikes.

```csharp
public class Bullet {
    public void Fire(Vector3 direction, Action<int> onHit) {
        // Simulate a hit and invoke the callback with the hit damage
        int damage = 10; // Example damage value
        onHit.Invoke(damage);
    }
}

public class Gun {
    int totalDamageDealt;

    public void Fire(Bullet bullet) {
        // Capture this as context and add the damage to the total
        bullet.Fire(
            transform.forward, 
            Closure.Action(this, (Gun gun, int damage) => gun.totalDamageDealt += damage).AsAction<int>()
        );
    }
}
```

This way, you can pass the closure as a delegate to the `Fire` method of the `Bullet` class,
capturing the context of the `Gun` instance allowing for modification of its state without unnecessary heap allocations.

## License
This project is licensed under the MIT License.