using Closures;

namespace ClosureTests.ActionTests.ClosureAction.EdgeCases;

[TestFixture]
public class ClosureActionEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default edge cases
    
    [Test]
    public void ClosureAction_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(testContext, ctx => Assert.That(ctx, Is.Null, "Context should be null"));
            closure.Invoke();
        }, "Closure with null context should not throw an exception");
    }
    
    [Test]
    public void ClosureAction_NullDelegate_Invoke_Throws() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = Closure.Action<int>(context, (Action<int>)null);
            closure.Invoke();
        }, "Closure with null delegate should throw an exception");
    }
    
    [Test]
    public void ClosureAction_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Action(context, ctx => throw new InvalidOperationException("Test exception"));

        Assert.Throws<InvalidOperationException>(() => {
            closure.Invoke();
        }, "Closure should throw an exception during invocation");
    }
    
    [Test]
    public void ClosureAction_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Action(context, ctx => throw new InvalidOperationException("Test exception"));

        try {
            closure.Invoke();
            Assert.Fail("Closure should throw an exception during invocation");
        } 
        catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }
}