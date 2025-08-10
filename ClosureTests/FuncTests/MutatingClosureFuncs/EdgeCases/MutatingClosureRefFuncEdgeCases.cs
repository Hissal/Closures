using Closures;

namespace ClosureTests.FuncTests.MutatingClosureFuncs.EdgeCases;

[TestFixture]
public class MutatingClosureRefFuncEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    [Test]
    public void MutatingClosureRefFunc_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.RefFunc<TestClass?, int, int>(testContext, (ref TestClass? ctx, ref int arg) => ctx == null ? arg : ctx.Value + arg);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void MutatingClosureRefFunc_NullDelegate_Invoke_Throws() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = MutatingClosure.RefFunc<int, int, int>(context, (RefFunc<int, int, int>)null);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void MutatingClosureRefFunc_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = MutatingClosure.RefFunc<int, int, int>(context, (ref int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        Assert.Throws<InvalidOperationException>(() => closure.Invoke(ref arg));
    }

    [Test]
    public void MutatingClosureRefFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = MutatingClosure.RefFunc<int, int, int>(context, (ref int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        try {
            closure.Invoke(arg);
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }
    
    // Argument closure edge cases
    
    [Test]
    public void MutatingClosureRefFunc_NullArgument_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int? arg) => {
                Assert.That(arg, Is.Null, "Argument should be null");
                return arg ?? ctx;
            });
            closure.Invoke(null);
        }, "Null argument should not throw");
    }
    
    // Ref argument closure edge cases

    [Test]
    public void MutatingClosureRefFunc_NullRefArgument_Invoke_DoesNotThrow() {
        string? context = "ctx";
        var closure = MutatingClosure.RefFunc<string?, string?, string?>(context, (ref string ctx, ref string? arg) => arg ?? ctx);
        string? arg = null;
        Assert.DoesNotThrow(() => {
            closure.Invoke(ref arg);
        });
    }

    [Test]
    public void MutatingClosureRefFunc_NullRefArg_SettingValue_ModifiesOriginalRef() {
        int context = 0;
        int expected = 123;
        var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int? arg) => arg = expected);
        int? arg = null;
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    // Mutating closure edge cases
    
    [Test]
    public void MutatingClosureRefFunc_ConcurrentInvoke_SharesContextAcrossThreads() {
        int context = 0;
        int taskCount = 10;

        int callSum = 0;

        int Handler(ref int ctx, ref int arg) {
            Interlocked.Add(ref callSum, 1);
            ctx++;
            Assert.That(ctx, Is.EqualTo(callSum), "Context should match call sum after each invocation");
            return ctx;
        }

        var closure = MutatingClosure.RefFunc<int, int, int>(context, Handler);
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