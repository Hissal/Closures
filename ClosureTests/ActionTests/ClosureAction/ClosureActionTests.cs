using Closures;

namespace ClosureTests.ActionTests.ClosureAction;

[TestFixture]
public class ClosureActionTests {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default tests
        
    [Test]
    public void ClosureAction_ReceivesContext() {
        int context = 5;
        int expected = 5;
            
        var closure = Closure.Action(context, ctx => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke();
    }
        
    [Test]
    public void ClosureAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        int expected = 5;

        var testContext = new TestClass();

        var closure = Closure.Action((testContext, expected), ctx => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
}