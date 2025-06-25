using Lh.Closures;

namespace ClosureTests.ActionTests.RefClosureAction.EdgeCases;

[TestFixture]
public class RefClosureRefActionEdgeCases {
    class TestClass { public int Value { get; set; } }
    
    // Default edge cases
    
    [Test]
    public void RefClosureRefAction_NullContext_Invoke_DoesNotThrow() {
        int? context = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int? ctx, ref int arg) => Assert.That(ctx, Is.Null, "Context should be null"));
            int arg = 42;
            closure.Invoke(ref arg);
        }, "Closure with null context should not throw an exception");
    }

    [Test]
    public void RefClosureRefAction_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (RefAction<int, int>)null);
            int arg = 42;
            closure.Invoke(arg);
        }, "Closure with null delegate should not throw an exception");
    }

    [Test]
    public void RefClosureRefAction_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (RefAction<int, int>)null);
            closure.Add((ref int ctx, ref int arg) => Assert.That(ctx, Is.EqualTo(context)));
            int arg = 42;
            closure.Invoke(ref arg);
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void RefClosureRefAction_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx, ref int arg) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Add(null);
            int arg = 42;
            closure.Invoke(ref arg);
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void RefClosureRefAction_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        void Handler(ref int ctx, ref int arg) { /* no-op */ }
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (RefAction<int, int>)null);
            closure.Remove(Handler);
            int arg = 42;
            closure.Invoke(ref arg);
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void RefClosureRefAction_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx, ref int arg) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Remove(null);
            int arg = 42;
            closure.Invoke(ref arg);
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void RefClosureRefAction_ExceptionDuringInvocation_Throws() {
        int context = 5;
        int arg = 42;
        Assert.Throws<InvalidOperationException>(() => {
            var closure = Closure.Action(ref context, (ref int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
            closure.Invoke(ref arg);
        }, "Closure should throw an exception during invocation");
    }

    [Test]
    public void RefClosureRefAction_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Action(ref context, (ref int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        try {
            closure.Invoke(ref arg);
            Assert.Fail("Closure should throw an exception during invocation");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }

    // Argument edge cases
    
    [Test]
    public void RefClosureRefAction_NullArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx, ref int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            closure.Invoke(null);
        }, "Closure with null argument should not throw an exception");
    }

    // Ref argument edge cases
    
    [Test]
    public void RefClosureRefAction_NullDelegate_Invoke_RefArg_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (RefAction<int, int>)null);
            int arg = 42;
            closure.Invoke(ref arg);
        }, "Closure with null delegate should not throw an exception");
    }
    
    [Test]
    public void RefClosureRefAction_NullRefArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(ref context, (ref int ctx, ref int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            int? arg = null;
            closure.Invoke(ref arg);
        }, "Closure with null ref argument should not throw an exception");
    }

    [Test]
    public void RefClosureRefAction_NullRefArg_SettingValue_ModifiesOriginalRef() {
        int context = 5;
        var closure = Closure.Action(ref context, (ref int ctx, ref int? arg) => {
            Assert.That(arg, Is.Null, "Argument should be null");
            arg = 42;
        });
        int? arg = null;
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(42), "Ref argument should be set to the value provided in the closure");
    }
    
    // Ref closure edge cases
    
    [Test]
    public void RefClosureRefAction_NullContext_Invoke_SetsValueToOriginalContext() {
        int? context = null;
        int expected = 42;
        
        var closure = Closure.Action(ref context, (ref int? ctx, ref int arg) => {
            Assert.That(ctx, Is.Null, "Context should be null");
            ctx = expected; // Set a value to ensure the closure can modify it
        });
        
        closure.Invoke(1);
        Assert.That(context, Is.EqualTo(expected), "Context should be modified by the closure");
    }

}