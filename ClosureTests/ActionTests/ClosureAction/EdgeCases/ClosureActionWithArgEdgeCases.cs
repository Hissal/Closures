using Closures;

namespace ClosureTests.ActionTests.ClosureAction.EdgeCases;

[TestFixture]
public class ClosureActionWithArgEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default edge cases

    [Test]
    public void ClosureActionWithArg_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(testContext, (TestClass ctx, int arg) => Assert.That(ctx, Is.Null, "Context should be null"));
            closure.Invoke(42);
        }, "Closure with null context should not throw an exception");
    }

    [Test]
    public void ClosureActionWithArg_NullDelegate_Invoke_Throws() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = Closure.Action(context, (Action<int, int>)null);
            closure.Invoke(42);
        }, "Closure with null delegate should not throw an exception");
    }

    [Test]
    public void ClosureActionWithArg_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Action(context, (int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => {
            closure.Invoke(42);
        }, "Closure should throw an exception during invocation");
    }

    [Test]
    public void ClosureActionWithArg_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Action(context, (int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke(42);
            Assert.Fail("Closure should throw an exception during invocation");
        }
        catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }
    
    // Argument edge cases
    
    [Test]
    public void ClosureActionWithArg_NullArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(context, (int ctx, int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            closure.Invoke(null);
        }, "Closure with null argument should not throw an exception");
    }
}