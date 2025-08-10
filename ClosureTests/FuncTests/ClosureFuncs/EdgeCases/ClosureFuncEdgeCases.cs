using Closures;

namespace ClosureTests.FuncTests.ClosureFuncs.EdgeCases;

[TestFixture]
public class ClosureFuncEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    // Default edge cases

    [Test]
    public void ClosureFunc_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func<TestClass?, int>(testContext, ctx => ctx?.Value ?? 0);
            closure.Invoke();
        });
    }

    [Test]
    public void ClosureFunc_NullDelegate_Invoke_Throws() {
        int context = 5;
        Assert.Throws<NullReferenceException>(() => {
            var closure = Closure.Func<int, int>(context, (Func<int, int>)null);
            closure.Invoke();
        });
    }

    [Test]
    public void ClosureFunc_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, ctx => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => closure.Invoke());
    }

    [Test]
    public void ClosureFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, ctx => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke();
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }
}