using Closures;

namespace ClosureTests.FuncTests.RefClosureFuncs.EdgeCases;

[TestFixture]
public class RefClosureFuncWithArgEdgeCases {
    class TestClass { public int Value { get; set; } }

    [Test]
    public void RefClosureFuncWithArg_NullContext_Invoke_DoesNotThrow() {
        int? context = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int? ctx, int arg) => ctx ?? arg);
            closure.Invoke(42);
        });
    }

    [Test]
    public void RefClosureFuncWithArg_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (FuncWithRefContext<int, int, int>)null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void RefClosureFuncWithArg_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (FuncWithRefContext<int, int, int>)null);
            closure.Add((ref int ctx, int arg) => ctx + arg);
            closure.Invoke(42);
        });
    }

    [Test]
    public void RefClosureFuncWithArg_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int ctx, int arg) => ctx + arg);
            closure.Add(null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void RefClosureFuncWithArg_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int ctx, int arg) => ctx + arg);
            closure.Remove(null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void RefClosureFuncWithArg_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int ctx, int arg) => ctx + arg);
            closure.Remove(null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void RefClosureFuncWithArg_ExceptionDuringInvocation_Throws() {
        int context = 5;
        Assert.Throws<InvalidOperationException>(() => {
            var closure = Closure.Func(ref context, (ref int ctx, int arg) => {
                throw new InvalidOperationException("Test exception");
                return 2;
            });
            closure.Invoke(42);
        });
    }

    [Test]
    public void RefClosureFuncWithArg_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Func(ref context, (ref int ctx, int arg) => {
            throw new InvalidOperationException("Test exception");
            return 2;
        });
        try {
            closure.Invoke(42);
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }

    // Argument closure edge cases
    
    [Test]
    public void RefClosureFuncWithArg_NullArgument_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int ctx, int? arg) => {
                Assert.That(arg, Is.Null, "Argument should be null");
                return arg ?? ctx;
            });
            closure.Invoke(null);
        }, "Null argument should not throw");
    }
    
    // Ref closure edge cases
    
    [Test]
    public void RefClosureFuncWithArg_NullContext_Invoke_SetsValueToOriginalContext() {
        int? context = null;
        int expected = 42;
        
        var closure = Closure.Func(ref context, (ref int? ctx, int arg) => {
            Assert.That(ctx, Is.Null, "Context should be null");
            return ctx = expected; // Set a value to ensure the closure can modify it
        });
        
        closure.Invoke(0);
        Assert.That(context, Is.EqualTo(expected), "Context should be modified by the closure");
    }
    
    // Func closure edge cases

    [Test]
    public void ClosureRefFuncWithArg_NullDelegate_ReturnsDefault() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (FuncWithRefContext<int,int,int>)null);
            int result = closure.Invoke(0);
            Assert.That(result, Is.EqualTo(0), "Default return value should be 0 when no delegate is set");
        }, "ClosureFunc with null delegate should not throw");
    }
}