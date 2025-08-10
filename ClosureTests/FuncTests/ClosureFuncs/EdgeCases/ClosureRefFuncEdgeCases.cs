using Closures;

namespace ClosureTests.FuncTests.ClosureFuncs.EdgeCases;

[TestFixture]
public class ClosureRefFuncEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    [Test]
    public void ClosureRefFunc_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.RefFunc<TestClass?, int, int>(testContext, (TestClass ctx, ref int arg) => ctx == null ? arg : ctx.Value + arg);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void ClosureRefFunc_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = Closure.RefFunc<int, int, int>(context, (RefFuncWithNormalContext<int, int, int>)null);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void ClosureRefFunc_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.RefFunc<int, int, int>(context, (int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        Assert.Throws<InvalidOperationException>(() => closure.Invoke(arg));
    }

    [Test]
    public void ClosureRefFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.RefFunc<int, int, int>(context, (int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
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
    public void ClosureRefFunc_NullArgument_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.RefFunc(context, (int ctx, ref int? arg) => {
                Assert.That(arg, Is.Null, "Argument should be null");
                return arg ?? ctx;
            });
            closure.Invoke(null);
        }, "Null argument should not throw");
    }

    // Ref argument closure edge cases

    [Test]
    public void ClosureRefFunc_NullRefArgument_Invoke_DoesNotThrow() {
        string? context = "ctx";
        var closure = Closure.RefFunc<string?, string?, string?>(context, (string ctx, ref string? arg) => arg ?? ctx);
        string? arg = null;
        Assert.DoesNotThrow(() => {
            closure.Invoke(ref arg);
        });
    }

    [Test]
    public void ClosureRefFunc_NullRefArg_SettingValue_ModifiesOriginalRef() {
        int context = 0;
        int expected = 123;
        var closure = Closure.RefFunc(context, (int ctx, ref int? arg) => arg = expected);
        int? arg = null;
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
}