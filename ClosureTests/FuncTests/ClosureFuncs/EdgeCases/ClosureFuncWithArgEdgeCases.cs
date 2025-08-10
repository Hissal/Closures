using Closures;

namespace ClosureTests.FuncTests.ClosureFuncs.EdgeCases;

[TestFixture]
public class ClosureFuncWithArgEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    // Default edge cases

    [Test]
    public void ClosureFuncWithArg_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(testContext, (TestClass? ctx, int arg) => ctx?.Value + arg ?? arg);
            closure.Invoke(42);
        });
    }

    [Test]
    public void ClosureFuncWithArg_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = Closure.Func(context, (Func<int, int, int>)null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void ClosureFuncWithArg_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => closure.Invoke(42));
    }

    [Test]
    public void ClosureFuncWithArg_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke(42);
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }
    
    // Argument closure edge cases
    
    [Test]
    public void ClosureFuncWithArg_NullArgument_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(context, (int ctx, int? arg) => {
                Assert.That(arg, Is.Null, "Argument should be null");
                return arg ?? ctx;
            });
            closure.Invoke(null);
        }, "Null argument should not throw");
    }
}