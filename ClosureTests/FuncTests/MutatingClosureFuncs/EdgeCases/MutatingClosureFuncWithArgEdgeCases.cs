using Closures;

namespace ClosureTests.FuncTests.MutatingClosureFuncs.EdgeCases;

[TestFixture]
public class MutatingClosureFuncWithArgEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    [Test]
    public void MutatingClosureFuncWithArg_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.Func<TestClass?, int, int>(testContext, (ref TestClass? ctx, int arg) => ctx == null ? arg : ctx.Value + arg);
            closure.Invoke(42);
        });
    }

    [Test]
    public void MutatingClosureFuncWithArg_NullDelegate_Invoke_Throws() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = MutatingClosure.Func<int, int, int>(context, (FuncWithRefContext<int, int, int>)null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void MutatingClosureFuncWithArg_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = MutatingClosure.Func<int, int, int>(context, (ref int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => closure.Invoke(42));
    }

    [Test]
    public void MutatingClosureFuncWithArg_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = MutatingClosure.Func<int, int, int>(context, (ref int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke(42);
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }

    // Argument closure edge cases
    
    [Test]
    public void MutatingClosureFuncWithArg_NullArgument_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.Func(context, (ref int ctx, int? arg) => {
                Assert.That(arg, Is.Null, "Argument should be null");
                return arg ?? ctx;
            });
            closure.Invoke(null);
        }, "Null argument should not throw");
    }
    
    // Mutating closure edge cases

    [Test]
    public void MutatingClosureFuncWithArg_ConcurrentInvoke_SharesContextAcrossThreads() {
        int context = 0;
        int taskCount = 10;

        int callSum = 0;

        int Handler(ref int ctx, int arg) {
            Interlocked.Add(ref callSum, 1);
            ctx++;
            Assert.That(ctx, Is.EqualTo(callSum), "Context should match call sum after each invocation");
            return ctx;
        }

        var closure = MutatingClosure.Func<int, int, int>(context, Handler);
        var tasks = new List<Task>();

        for (int i = 0; i < taskCount; i++) {
            tasks.Add(Task.Run(() => closure.Invoke(0)));
        }

        Task.WaitAll(tasks.ToArray());
        Assert.Multiple(() => {
            Assert.That(callSum, Is.EqualTo(taskCount), "Call sum should equal number of tasks invoked");
            Assert.That(closure.Context, Is.EqualTo(taskCount), "Closure context should equal number of tasks invoked");
        });
    }
}