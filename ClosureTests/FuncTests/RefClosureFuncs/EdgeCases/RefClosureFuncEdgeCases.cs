using Lh.Closures;

namespace ClosureTests.FuncTests.RefClosureFuncs.EdgeCases;

[TestFixture]
public class RefClosureFuncEdgeCases {
    class TestClass { public int Value { get; set; } }

    [Test]
    public void RefClosureFunc_NullContext_Invoke_DoesNotThrow() {
        int? context = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int? ctx) => ctx ?? 0);
            closure.Invoke();
        });
    }

    [Test]
    public void RefClosureFunc_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (RefFunc<int, int>)null);
            closure.Invoke();
        });
    }

    [Test]
    public void RefClosureFunc_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (RefFunc<int, int>)null);
            closure.Add((ref int ctx) => ctx + 1);
            closure.Invoke();
        });
    }

    [Test]
    public void RefClosureFunc_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int ctx) => ctx + 1);
            closure.Add(null);
            closure.Invoke();
        });
    }

    [Test]
    public void RefClosureFunc_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int ctx) => ctx + 1);
            closure.Remove(null);
            closure.Invoke();
        });
    }

    [Test]
    public void RefClosureFunc_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int ctx) => ctx + 1);
            closure.Remove(null);
            closure.Invoke();
        });
    }

    [Test]
    public void RefClosureFunc_ExceptionDuringInvocation_Throws() {
        int context = 5;
        Assert.Throws<InvalidOperationException>(() => {
            var closure = Closure.Func(ref context, (ref int ctx) => {
                throw new InvalidOperationException("Test exception");
                return 2;
            });
            closure.Invoke();
        });
    }

    [Test]
    public void RefClosureFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Func(ref context, (ref int ctx) => {
            throw new InvalidOperationException("Test exception");
            return 2;
        });
        try {
            closure.Invoke();
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }
        
    // Ref closure edge cases
    
    [Test]
    public void RefClosureFunc_NullContext_Invoke_SetsValueToOriginalContext() {
        int? context = null;
        int expected = 42;
        
        var closure = Closure.Func(ref context, (ref int? ctx) => {
            Assert.That(ctx, Is.Null, "Context should be null");
            return ctx = expected; // Set a value to ensure the closure can modify it
        });
        
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected), "Context should be modified by the closure");
    }
    
    // Func closure edge cases

    [Test]
    public void RefClosureFunc_NullDelegate_ReturnsDefault() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (RefFunc<int,int>)null);
            int result = closure.Invoke();
            Assert.That(result, Is.EqualTo(0), "Default return value should be 0 when no delegate is set");
        }, "ClosureFunc with null delegate should not throw");
    }
}