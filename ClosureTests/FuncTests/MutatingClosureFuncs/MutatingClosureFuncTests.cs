using Closures;

namespace ClosureTests.FuncTests.MutatingClosureFuncs;

[TestFixture]
public class MutatingClosureFuncTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests
    [Test]
    public void MutatingClosureFunc_ReceivesContext() {
        int context = 5;
        int expected = 5;
        var closure = MutatingClosure.Func(context, (ref int ctx) => { Assert.That(ctx, Is.EqualTo(expected)); return ctx; });
        closure.Invoke();
    }

    [Test]
    public void MutatingClosureFunc_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        var closure = MutatingClosure.Func((testContext, expected), (ref (TestClass testContext, int expected) ctx) => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Mutating tests
    [Test]
    public void MutatingClosureFunc_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
        
        var closure = MutatingClosure.Func(context, (ref int ctx) => ctx += addition);
        closure.Invoke();
        
        Assert.That(closure.Context, Is.EqualTo(expected));
    }

    // Func tests
    [Test]
    public void MutatingClosureFunc_ReturnsExpectedValue() {
        int context = 10;
        int expected = context * 2;
        
        var closure = MutatingClosure.Func(context, (ref int ctx) => ctx * 2);
        int result = closure.Invoke();
        
        Assert.That(result, Is.EqualTo(expected));
    }
}