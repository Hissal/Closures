using Closures;

namespace ClosureTests.FuncTests.MutatingClosureFuncs.EdgeCases;

[TestFixture]
public class MutatingClosureFuncEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    [Test]
    public void MutatingClosureFunc_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.Func<TestClass?, int>(testContext, (ref TestClass? ctx) => ctx == null ? 0 : ctx.Value);
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_NullDelegate_Invoke_Throws() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = MutatingClosure.Func<int, int>(context, (RefFunc<int, int>)null);
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = MutatingClosure.Func<int, int>(context, (ref int ctx) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => closure.Invoke());
    }

    [Test]
    public void MutatingClosureFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = MutatingClosure.Func<int, int>(context, (ref int ctx) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke();
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }

    // Mutating closure edge cases

    [Test]
    public void MutatingClosureFunc_ConcurrentInvoke_SharesContextAcrossThreads() {
        int context = 0;
        int taskCount = 10;

        int callSum = 0;

        int Handler(ref int ctx) {
            Interlocked.Add(ref callSum, 1);
            ctx++;
            Assert.That(ctx, Is.EqualTo(callSum), "Context should match call sum after each invocation");
            return ctx;
        }

        var closure = MutatingClosure.Func(context, Handler);
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