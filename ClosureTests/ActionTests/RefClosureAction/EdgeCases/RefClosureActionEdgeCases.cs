using Lh.Closures;

namespace ClosureTests.ActionTests.RefClosureAction.EdgeCases;

[TestFixture]
public class RefClosureActionEdgeCases {
    class TestClass { public int Value { get; set; } }

    // Default edge cases
    
    [Test]
    public void RefClosureAction_NullContext_Invoke_DoesNotThrow() {
        int? context = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int? ctx) => Assert.That(ctx, Is.Null, "Context should be null"));
            closure.Invoke();
        }, "Closure with null context should not throw an exception");
    }
    
    [Test]
    public void RefClosureAction_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (RefAction<int>)null);
            closure.Invoke();
        }, "Closure with null delegate should not throw an exception");
    }

    [Test]
    public void RefClosureAction_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (RefAction<int>)null);
            closure.Add((ref int ctx) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Invoke();
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void RefClosureAction_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Add(null);
            closure.Invoke();
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void RefClosureAction_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        void Handler(ref int ctx) { /* no-op */ }
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (RefAction<int>)null);
            closure.Remove(Handler);
            closure.Invoke();
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void RefClosureAction_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Remove(null);
            closure.Invoke();
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void RefClosureAction_ExceptionDuringInvocation_Throws() {
        int context = 5;
        Assert.Throws<InvalidOperationException>(() => {
            var closure = Closure.Action(ref context, (ref int ctx) => throw new InvalidOperationException("Test exception"));
            closure.Invoke();
        }, "Closure should throw an exception during invocation");
    }

    [Test]
    public void RefClosureAction_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Action(ref context, (ref int ctx) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke();
            Assert.Fail("Closure should throw an exception during invocation");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }
    
    // Ref closure edge cases
    
    [Test]
    public void RefClosureAction_NullContext_Invoke_SetsValueToOriginalContext() {
        int? context = null;
        int expected = 42;
        
        var closure = Closure.Action(ref context, (ref int? ctx) => {
            Assert.That(ctx, Is.Null, "Context should be null");
            ctx = expected; // Set a value to ensure the closure can modify it
        });
        
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected), "Context should be modified by the closure");
    }
}