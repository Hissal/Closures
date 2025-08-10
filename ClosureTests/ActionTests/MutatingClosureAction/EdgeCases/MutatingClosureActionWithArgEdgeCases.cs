using Closures;

namespace ClosureTests.ActionTests.MutatingClosureAction.EdgeCases;

[TestFixture]
public class MutatingClosureActionWithArgEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default edge cases

    [Test]
    public void MutatingClosureActionWithArg_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.Action(testContext, (ref TestClass? ctx, int arg) => Assert.That(ctx, Is.Null, "Context should be null"));
            closure.Invoke(42);
        }, "Closure with null context should not throw an exception");
    }

    [Test]
    public void MutatingClosureActionWithArg_NullDelegate_Invoke_Throws() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = MutatingClosure.Action(context, (ActionWithRefContext<int, int>)null);
            closure.Invoke(42);
        }, "Closure with null delegate should not throw an exception");
    }

    [Test]
    public void MutatingClosureActionWithArg_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = MutatingClosure.Action(context, (ref int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => {
            closure.Invoke(42);
        }, "Closure should throw an exception during invocation");
    }

    [Test]
    public void MutatingClosureActionWithArg_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = MutatingClosure.Action(context, (ref int ctx, int arg) => throw new InvalidOperationException("Test exception"));
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
    public void MutatingClosureActionWithArg_NullArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.Action(context, (ref int ctx, int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            closure.Invoke(null);
        }, "Closure with null argument should not throw an exception");
    }
    
    // Mutating closure edge cases

    [Test]
    public void MutatingClosureActionWithArg_ConcurrentInvoke_SharesContextAcrossThreads() {
        // TODO: the ref ctx across multiple threads is not safe since it passes the same reference to all
        int context = 0;
        int taskCount = 10;

        int callSum = 0;

        void Handler(ref int ctx, int arg) {
            Interlocked.Add(ref callSum, 1);
            ctx++;
            Assert.That(ctx, Is.EqualTo(callSum), "Context should match call sum after each invocation");
        }

        var closure = MutatingClosure.Action<int, int>(context, Handler);
        var tasks = new List<Task>();

        for (int i = 0; i < taskCount; i++) {
            tasks.Add(Task.Run(() => closure.Invoke(1)));
        }

        Task.WaitAll(tasks.ToArray());
        Assert.Multiple(() => {
            Assert.That(callSum, Is.EqualTo(taskCount), "Call sum should equal number of tasks invoked");
            Assert.That(closure.Context, Is.EqualTo(taskCount), "Closure context should equal number of tasks invoked");
        });
    }
}