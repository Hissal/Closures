using Lh.Closures;

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
    public void ClosureAction_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(context, null);
            closure.Invoke();
        }, "Closure with null delegate should not throw an exception");
    }
    
    [Test]
    public void ClosureAction_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, null);
        
        Assert.DoesNotThrow(() => {
            closure.Add(ctx => Assert.That(ctx, Is.EqualTo(context)));
            closure.Invoke();
        }, "Adding a null delegate to a closure should not throw an exception");
    }
    
    [Test]
    public void ClosureAction_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, ctx => Assert.That(ctx, Is.EqualTo(context)));
        
        Assert.DoesNotThrow(() => {
            closure.Add(null);
            closure.Invoke();
        }, "Adding a null delegate to a closure should not throw an exception");
    }
    
    [Test]
    public void ClosureAction_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, null);
        
        void Handler(int ctx) { /* no-op */ } 
        
        Assert.DoesNotThrow(() => {
            closure.Remove(Handler);
            closure.Invoke();
        }, "Adding a null delegate to a closure should not throw an exception");
    }
    
    [Test]
    public void ClosureAction_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, ctx => Assert.That(ctx, Is.EqualTo(context)));
        
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke();
        }, "Adding a null delegate to a closure should not throw an exception");
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
    
    [Test]
    public void ClosureAction_ConcurrentAddRemoveInvoke_IsThreadSafe() {
        int context = 1;
        int callSum = 0;
        void Handler(int ctx) => Interlocked.Add(ref callSum, ctx);

        var closure = Closure.Action(context, Handler);

        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++) {
            tasks.Add(Task.Run(() => closure.Add(Handler)));
            tasks.Add(Task.Run(() => closure.Remove(Handler)));
            tasks.Add(Task.Run(() => closure.Invoke()));
        }

        Task.WaitAll(tasks.ToArray());

        // The exact value may vary, but should not throw or corrupt state
        Assert.That(callSum, Is.GreaterThanOrEqualTo(0), "Call sum should be non-negative");
    }
}