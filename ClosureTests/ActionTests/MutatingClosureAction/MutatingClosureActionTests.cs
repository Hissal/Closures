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

        var closure = Closure.Action(context, (ref int ctx) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke();
    }

    [Test]
    public void MutatingClosureAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;

        var closure = Closure.Action((testContext, expected), (ref (TestClass testContext, int expected) ctx) => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    [Test]
    public void MutatingClosureAction_AddAndRemove_Works() {
        int context = 1;
        int callSum = 0;
        void Handler(ref int ctx) => callSum += ctx;

        var closure = Closure.Action(context, Handler);
        closure.Add(Handler);
        closure.Invoke();
        closure.Remove(Handler);
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(3 * context));
    }

    [Test]
    public void MutatingClosureAction_Add_MultipleTimes_Works() {
        int context = 2;
        int actionCount = 5;
        int callSum = 0;
        void Handler(ref int ctx) => callSum += ctx;

        var closure = Closure.Action(context, Handler);
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(actionCount * context));
    }

    [Test]
    public void MutatingClosureAction_Remove_MultipleTimes_Works() {
        int context = 3;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context;
        int callSum = 0;
        void Handler(ref int ctx) => callSum += ctx;

        var closure = Closure.Action(context, Handler);
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        for (int i = 0; i < actionCount - amountToCall; i++) {
            closure.Remove(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }
    
    // Mutating tests

    [Test]
    public void MutatingClosureAction_Retain_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = Closure.Action(context, (ref int ctx) => ctx += addition, MutatingClosureBehaviour.Retain);
        closure.Invoke();
            
        Assert.That(closure.Context, Is.EqualTo(expected)); // Closure's context modified
    }
    
    [Test]
    public void MutatingClosureAction_Reset_ResetsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context;
            
        var closure = Closure.Action(context, (ref int ctx) => ctx += addition, MutatingClosureBehaviour.Reset);
        closure.Invoke();
            
        Assert.That(closure.Context, Is.EqualTo(expected)); // Closure's context modified
    }
}