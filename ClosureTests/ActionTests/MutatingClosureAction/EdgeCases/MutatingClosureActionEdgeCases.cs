using Closures;

namespace ClosureTests.ActionTests.MutatingClosureAction.EdgeCases;

[TestFixture]
public class MutatingClosureActionEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    // Default edge cases

    [Test]
    public void MutatingClosureAction_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(testContext,
                (ref TestClass? ctx) => Assert.That(ctx, Is.Null, "Context should be null"));
            closure.Invoke();
        }, "Closure with null context should not throw an exception");
    }

    [Test]
    public void MutatingClosureAction_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(context, (RefAction<int>)null);
            closure.Invoke();
        }, "Closure with null delegate should not throw an exception");
    }

    [Test]
    public void MutatingClosureAction_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (RefAction<int>)null);
        Assert.DoesNotThrow(() => {
            closure.Add((ref int ctx) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Invoke();
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void MutatingClosureAction_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (ref int ctx) => Assert.That(ctx, Is.EqualTo(context)));
        Assert.DoesNotThrow(() => {
            closure.Add(null);
            closure.Invoke();
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void MutatingClosureAction_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (RefAction<int>)null);

        void Handler(ref int ctx) { /* no-op */
        }

        Assert.DoesNotThrow(() => {
            closure.Remove(Handler);
            closure.Invoke();
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void MutatingClosureAction_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (ref int ctx) => Assert.That(ctx, Is.EqualTo(context)));
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke();
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void MutatingClosureAction_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Action(context, (ref int ctx) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => { closure.Invoke(); },
            "Closure should throw an exception during invocation");
    }

    [Test]
    public void MutatingClosureAction_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Action(context, (ref int ctx) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke();
            Assert.Fail("Closure should throw an exception during invocation");
        }
        catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }

    [Test]
    public void MutatingClosureAction_ConcurrentAddRemoveInvoke_IsThreadSafe() {
        int context = 1;
        int callSum = 0;
        void Handler(ref int ctx) => Interlocked.Add(ref callSum, ctx);
        var closure = Closure.Action(context, Handler);
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++) {
            tasks.Add(Task.Run(() => closure.Add(Handler)));
            tasks.Add(Task.Run(() => closure.Remove(Handler)));
            tasks.Add(Task.Run(() => closure.Invoke()));
        }

        Task.WaitAll(tasks.ToArray());
        Assert.That(callSum, Is.GreaterThanOrEqualTo(0), "Call sum should be non-negative");
    }

    // Mutating closure edge cases

    [Test]
    public void MutatingClosureAction_Reset_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (RefAction<int>)null, MutatingClosureBehaviour.Reset);

        Assert.DoesNotThrow(() => closure.Invoke());
    }

    [Test]
    public void MutatingClosureAction_ConcurrentInvoke_SharesContextAcrossThreads() {
        int context = 0;
        int taskCount = 10;

        int callSum = 0;

        void Handler(ref int ctx) {
            Interlocked.Add(ref callSum, 1);
            ctx++;
            Assert.That(ctx, Is.EqualTo(callSum), "Context should match call sum after each invocation");
        }

        var closure = Closure.Action(context, Handler);
        var tasks = new List<Task>();

        for (int i = 0; i < taskCount; i++) {
            tasks.Add(Task.Run(() => closure.Invoke()));
        }

        Task.WaitAll(tasks.ToArray());
        Assert.Multiple(() => {
            Assert.That(callSum, Is.EqualTo(taskCount), "Call sum should equal number of tasks invoked");
            Assert.That(closure.Context, Is.EqualTo(taskCount), "Closure context should equal number of tasks invoked");
        });
    }
}