using Closures;

namespace ClosureTests.ActionTests.MutatingClosureAction.EdgeCases;

[TestFixture]
public class MutatingClosureRefActionEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    // Default edge cases
    
    [Test]
    public void MutatingClosureRefAction_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.RefAction(testContext, (ref TestClass? ctx, ref int arg) => Assert.That(ctx, Is.Null, "Context should be null"));
            int arg = 42;
            closure.Invoke(ref arg);
        }, "Closure with null context should not throw an exception");
    }

    [Test]
    public void MutatingClosureRefAction_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = MutatingClosure.RefAction(context, (RefAction<int, int>)null);
            int arg = 42;
            closure.Invoke(arg);
        }, "Closure with null delegate should not throw an exception");
    }

    [Test]
    public void MutatingClosureRefAction_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        Assert.Throws<InvalidOperationException>(() => {
            closure.Invoke(ref arg);
        }, "Closure should throw an exception during invocation");
    }

    [Test]
    public void MutatingClosureRefAction_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;

        try {
            closure.Invoke(ref arg);
            Assert.Fail("Closure should throw an exception during invocation");
        }
        catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }
    
    // Argument edge cases
    
    [Test]
    public void MutatingClosureRefAction_NullArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            closure.Invoke(null);
        }, "Closure with null argument should not throw an exception");
    }

    // Ref argument edge cases
    
    [Test]
    public void MutatingClosureRefAction_NullRefArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            int? arg = null;
            closure.Invoke(ref arg);
        }, "Closure with null ref argument should not throw an exception");
    }

    [Test]
    public void MutatingClosureRefAction_NullRefArg_SettingValue_ModifiesOriginalRef() {
        int context = 5;
        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int? arg) => {
            Assert.That(arg, Is.Null, "Argument should be null");
            arg = 42;
        });
        int? arg = null;
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(42), "Ref argument should be set to the value provided in the closure");
    }
    
    // Mutating closure edge cases

    [Test]
    public void MutatingClosureActionWithArg_ConcurrentInvoke_SharesContextAcrossThreads() {
        int context = 0;
        int taskCount = 10;

        int callSum = 0;

        void Handler(ref int ctx, ref int arg) {
            Interlocked.Add(ref callSum, 1);
            ctx++;
            Assert.That(ctx, Is.EqualTo(callSum), "Context should match call sum after each invocation");
        }

        var closure = MutatingClosure.RefAction<int, int>(context, Handler);
        var tasks = new List<Task>();

        for (int i = 0; i < taskCount; i++) {
            var arg = 1;
            tasks.Add(Task.Run(() => closure.Invoke(arg)));
        }

        Task.WaitAll(tasks.ToArray());
        Assert.Multiple(() => {
            Assert.That(callSum, Is.EqualTo(taskCount), "Call sum should equal number of tasks invoked");
            Assert.That(closure.Context, Is.EqualTo(taskCount), "Closure context should equal number of tasks invoked");
        });
    }
}