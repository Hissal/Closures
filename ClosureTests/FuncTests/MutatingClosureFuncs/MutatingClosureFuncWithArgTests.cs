using Closures;

namespace ClosureTests.FuncTests.MutatingClosureFuncs;

[TestFixture]
public class MutatingClosureFuncWithArgTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests
    [Test]
    public void MutatingClosureFuncWithArg_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;
        var closure = MutatingClosure.Func(context, (ref int ctx, int a) => { Assert.That(ctx, Is.EqualTo(expected)); return ctx; });
        closure.Invoke(arg);
    }

    [Test]
    public void MutatingClosureFuncWithArg_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;
        var closure = MutatingClosure.Func((testContext, expected), (ref (TestClass testContext, int expected) ctx, int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
    
    // Argument tests
    
    [Test]
    public void MutatingClosureFuncWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
        var closure = MutatingClosure.Func(context, (ref int ctx, int a) => { Assert.That(ctx + a, Is.EqualTo(expected)); return ctx; });
        closure.Invoke(arg);
    }

    // Mutating tests
    [Test]
    public void MutatingClosureFuncWithArg_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
        var closure = MutatingClosure.Func(context, (ref int ctx, int arg) => ctx += arg);
        closure.Invoke(addition);
        Assert.That(closure.Context, Is.EqualTo(expected));
    }

    // Func tests
    [Test]
    public void MutatingClosureFuncWithArg_ReturnsExpectedValue() {
        int context = 10;
        int addition = 5;
        int expected = context + addition;
        var closure = MutatingClosure.Func(context, (ref int ctx, int arg) => ctx + arg);
        int result = closure.Invoke(addition);
        Assert.That(result, Is.EqualTo(expected));
    }
}