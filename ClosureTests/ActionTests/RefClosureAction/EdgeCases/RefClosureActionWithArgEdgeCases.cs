using Closures;

namespace ClosureTests.ActionTests.RefClosureAction.EdgeCases;

[TestFixture]
public class RefClosureActionWithArgEdgeCases {
    class TestClass { public int Value { get; set; } }
    
    // Default edge cases
    
    [Test]
    public void RefClosureActionWithArg_NullContext_Invoke_DoesNotThrow() {
        int? context = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int? ctx, int arg) => Assert.That(ctx, Is.Null, "Context should be null"));
            closure.Invoke(42);
        }, "Closure with null context should not throw an exception");
    }
    
    [Test]
    public void RefClosureActionWithArg_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ActionWithRefContext<int, int>)null);
            closure.Invoke(42);
        }, "Closure with null delegate should not throw an exception");
    }

    [Test]
    public void RefClosureActionWithArg_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ActionWithRefContext<int, int>)null);
            closure.Add((ref int ctx, int arg) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Invoke(42);
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void RefClosureActionWithArg_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx, int arg) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Add(null);
            closure.Invoke(42);
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void RefClosureActionWithArg_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        void Handler(ref int ctx, int arg) { /* no-op */ }
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ActionWithRefContext<int, int>)null);
            closure.Remove(Handler);
            closure.Invoke(42);
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void RefClosureActionWithArg_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx, int arg) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Remove(null);
            closure.Invoke(42);
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void RefClosureActionWithArg_ExceptionDuringInvocation_Throws() {
        int context = 5;
        Assert.Throws<InvalidOperationException>(() => {
            var closure = Closure.Action(ref context, (ref int ctx, int arg) => throw new InvalidOperationException("Test exception"));
            closure.Invoke(42);
        }, "Closure should throw an exception during invocation");
    }

    [Test]
    public void RefClosureActionWithArg_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Action(ref context, (ref int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke(42);
            Assert.Fail("Closure should throw an exception during invocation");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }

    // Argument edge cases
    
    [Test]
    public void RefClosureActionWithArg_NullArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx, int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            closure.Invoke(null);
        }, "Closure with null argument should not throw an exception");
    }
    
    // Ref closure edge cases
    
    [Test]
    public void RefClosureActionWithArg_NullContext_Invoke_SetsValueToOriginalContext() {
        int? context = null;
        int expected = 42;
        
        var closure = Closure.Action(ref context, (ref int? ctx, int arg) => {
            Assert.That(ctx, Is.Null, "Context should be null");
            ctx = expected; // Set a value to ensure the closure can modify it
        });
        
        closure.Invoke(1);
        Assert.That(context, Is.EqualTo(expected), "Context should be modified by the closure");
    }

}