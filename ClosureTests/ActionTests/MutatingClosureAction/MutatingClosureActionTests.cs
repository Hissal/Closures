using Closures;

namespace ClosureTests.ActionTests.MutatingClosureAction;

[TestFixture]
public class MutatingClosureActionTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests
    
    [Test]
    public void MutatingClosureAction_ReceivesContext() {
        int context = 5;
        int expected = 5;

        var closure = MutatingClosure.Action(context, (ref int ctx) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke();
    }

    [Test]
    public void MutatingClosureAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;

        var closure = MutatingClosure.Action((testContext, expected), (ref (TestClass testContext, int expected) ctx) => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
    
    // Mutating tests

    [Test]
    public void MutatingClosureAction_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = MutatingClosure.Action(context, (ref int ctx) => ctx += addition);
        closure.Invoke();
            
        Assert.That(closure.Context, Is.EqualTo(expected)); // Closure's context modified
    }
}