# Closures
Closure structs that capture a context and a delegate that is invoked with the given context.

```csharp
using Closures;

var closure = Closure.Create(100, (context) => $"Captured Closure {context}");
Console.WriteLine(closure.Invoke()); // Output: Captured Closure 100
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

## Why?
Due to the nature of closures, 
the context is captured as reference.

The following code might not work as expected:
```csharp
List<Action> actions = new List<Action>();

for (int i = 0; i < 3; i++) {
    var action = new Action(() => Console.WriteLine($"{i}"));
    actions.Add(action);
}

foreach (var action in actions) {
    action.Invoke();
    // Output: 3, 3, 3
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
    var capturedI = i; // Capture the current value of i
    var action = new Action(() => Console.WriteLine($"{capturedI}"));
    actions.Add(action);
}

foreach (var action in actions) {
    action.Invoke();
    // Output: 0, 1, 2
}
```
Though this works, it allows `CapturedI` to escape its default lifetime when getting captured by the lambda,
creating a closure allocation capture that is allocated on the heap.

To avoid this overhead, you can use the `Closure` structs provided in this library:
```csharp
using Closures;

List<ActionClosure<int>> closures = new List<ActionClosure<int>>();

for (int i = 0; i < 3; i++) {
    // Create a closure that captures the current value of i
    var closure = Closure.Create(i, (context) => Console.WriteLine($"{context}"));
    closures.Add(closure);
}

foreach (var closure in closures) {
    closure.Invoke();
    // Output: 0, 1, 2
}
```
This way, you avoid unnecessary heap allocations when capturing variables, 
which in turn reduces garbage collection overhead.
This is especially beneficial in performance-critical scenarios, such as game development,
where minimizing allocations and maximizing efficiency are crucial.

## Closure Types
- `ActionClosure<TContext>`: Captures a context of type `TContext` and invokes an action with that context.
- `ActionClosureRef<TContext>`: Captures a context of type `TContext` by value and invokes a ref action that can mutate the stored context.
- `ActionClosure<TContext, TArg>`: Captures a context of type `TContext` and invokes an action with an argument of type `TArg`.
- `ActionClosureRef<TContext, TArg>`: Captures a context of type `TContext` by value and invokes an action with an argument, allowing mutation of the stored context.
- `RefActionClosure<TContext, TArg>`: Captures a context of type `TContext` and invokes an action with a ref argument of type `TArg`.
- `RefActionClosureRef<TContext, TArg>`: Captures a context of type `TContext` by value and invokes a ref action with a ref argument, allowing mutation of both context and argument.
- `PassedRefActionClosure<TContext>`: Captures a reference to a context variable of type `TContext` and invokes a ref action, mutating the original variable.
- `PassedRefActionClosure<TContext, TArg>`: Captures a reference to a context variable and invokes an action with an argument, mutating the original variable.
- `PassedRefRefActionClosure<TContext, TArg>`: Captures a reference to a context variable and invokes a ref action with a ref argument, mutating both.


- `FuncClosure<TContext, TResult>`: Captures a context of type `TContext` and invokes a function returning `TResult`.
- `FuncClosureRef<TContext, TResult>`: Captures a context of type `TContext` by value and invokes a ref function, allowing mutation of the stored context.
- `FuncClosure<TContext, TArg, TResult>`: Captures a context of type `TContext` and invokes a function with an argument, returning `TResult`.
- `FuncClosureRef<TContext, TArg, TResult>`: Captures a context of type `TContext` by value and invokes a function with an argument, allowing mutation of the stored context.
- `RefFuncClosure<TContext, TArg, TResult>`: Captures a context of type `TContext` and invokes a function with a ref argument, returning `TResult`.
- `RefFuncClosureRef<TContext, TArg, TResult>`: Captures a context of type `TContext` by value and invokes a ref function with a ref argument, allowing mutation of both context and argument.
- `PassedRefFuncClosure<TContext, TResult>`: Captures a reference to a context variable and invokes a ref function, mutating the original variable.
- `PassedRefFuncClosure<TContext, TArg, TResult>`: Captures a reference to a context variable and invokes a function with an argument, mutating the original variable.
- `PassedRefRefFuncClosure<TContext, TArg, TResult>`: Captures a reference to a context variable and invokes a ref function with a ref argument, mutating both.


## Performance
The closures in this library are designed to minimize heap allocations and improve performance by capturing context values directly. This is particularly useful in scenarios where closures are frequently created and invoked, such as in loops or event handlers.

## Usage
To use the closures, simply create an instance of the desired closure type using the `Closure.Create` method, passing in the context and the delegate to be invoked. The closures can then be invoked like any other delegate.

### Basic closures
- `ActionClosure<TContext>` that captures a string context.
- `FuncClosure<TContext, TResult>` that captures an integer context and returns a doubled value.

```csharp
using Closures;
// Example of ActionClosure
var actionClosure = Closure.Create("captured context", (string context) => Console.WriteLine($"Action with context: {context}"));
actionClosure.Invoke(); // Output: Action with context: captured context

// Example of FuncClosure
var funcClosure = Closure.Create(10, (int context) => context * 2);
Console.WriteLine(funcClosure.Invoke()); // Output: 20
```

### Closures with an argument
- `ActionClosure<TContext, TArg>` that captures a string context and is called with an int argument.
- `FuncClosure<TContext, TArg, TResult>` that captures an integer context, is called with an int argument and returns the sum of the context and argument.

```csharp
using Closures;
// Example of ActionClosure with an argument
var actionClosureWithArg = Closure.Create("captured context", (string context, int arg) => Console.WriteLine($"Action with context: {context}, Arg: {arg}"));
actionClosureWithArg.Invoke(5); // Output: Action with context: captured context, Arg: 5

// Example of FuncClosure with an argument
var funcClosureWithArg = Closure.Create(10, (int context, int arg) => context + arg);
Console.WriteLine(funcClosureWithArg.Invoke(5)); // Output: 15
```

### Ref closures
- `ActionClosureRef<int>` that captures a mutable context.
- `FuncClosureRef<int, int>` that captures a mutable context and returns the sum of modifications.

```csharp
using Closures;
// Example of ActionClosureRef
var mutableContext = 10;
var actionClosureRef = Closure.Create(mutableContext, (ref int context) => {
    Console.WriteLine(context)
    context += 5
});
actionClosureRef.AddAction((ref int context) => Console.WriteLine(context));
actionClosureRef.Invoke(); // Output: 10, 15

// Example of FuncClosureRef
var mutableFuncContext = 20;
var funcClosureRef = Closure.Create(mutableFuncContext, (ref int context) => context *= 2);
funcClosureRef.AddFunc((ref int context) => context += 10);
Console.WriteLine(funcClosureRef.Invoke()); // Output: 50
```

### Passed reference closures
These are ref structs that capture a reference to a mutable context variable, allowing you to modify the original value.

- `PassedRefActionClosure<int>` that captures a reference to a mutable context.
- `PassedRefFuncClosure<int, int>` that captures a reference to a mutable context and returns the modified value.

```csharp
using Closures;
// Example of PassedRefActionClosure
var passedMutableContext = 30;
var passedRefActionClosure = Closure.Create(ref passedMutableContext, (ref int context) => {
    Console.WriteLine(context);
    context += 10;
});
passedRefActionClosure.Invoke(); // Output: 30
Console.WriteLine(passedMutableContext); // Output: 40

// Example of PassedRefFuncClosure
var passedMutableFuncContext = 40;
var passedRefFuncClosure = Closure.Create(ref passedMutableFuncContext, (ref int context) => {
    context *= 2;
    return context;
});
Console.WriteLine(passedRefFuncClosure.Invoke()); // Output: 80
Console.WriteLine(passedMutableFuncContext); // Output: 80
```

## Contributing
If you would like to contribute to the Closures library,
feel free to submit a pull request or open an issue on the GitHub repository.
Contributions are welcome, and we appreciate any feedback or improvements to the code.

## License
This project is licensed under the MIT License.