using Lh.Closures;

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
    
    [Test]
    public void ClosureAction_AddAndRemove_Works() {
        int context = 1;
        int callSum = 0;
        void Handler(int ctx) => callSum += ctx;

        var closure = Closure.Action(context, Handler);
        closure.Add(Handler);
        closure.Invoke();
        closure.Remove(Handler);
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(3 * context));
    }
    
    [Test]
    public void ClosureAction_Add_MultipleTimes_Works() {
        int context = 2;
        int actionCount = 5;
            
        int callSum = 0;
        void Handler(int ctx) => callSum += ctx;

        var closure = Closure.Action(context, Handler);
        
        // Add multiple actions (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(actionCount * context));
    }
        
    [Test]
    public void ClosureAction_RemoveMultipleTimes_Works() {
        int context = 3;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context;
        
        int callSum = 0;
        void Handler(int ctx) => callSum += ctx;

        var closure = Closure.Action(context, Handler);
        
        // Add multiple actions (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
            
        // Remove all but the specified number of actions
        for (int i = 0; i < actionCount - amountToCall; i++) {
            closure.Remove(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }
}