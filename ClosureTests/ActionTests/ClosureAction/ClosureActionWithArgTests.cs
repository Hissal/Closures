using Closures;

namespace ClosureTests.ActionTests.ClosureAction;

[TestFixture]
public class ClosureActionWithArgTests {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default tests

    [Test]
    public void ClosureActionWithArg_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;

        var closure = Closure.Action(context, (int ctx, int a) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }

    [Test]
    public void ClosureActionWithArg_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;

        var closure = Closure.Action((testContext, expected), ((TestClass testContext, int expected) ctx, int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
    
    // Argument tests
    
    [Test]
    public void ClosureActionWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = Closure.Action(context, (int ctx, int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }
}