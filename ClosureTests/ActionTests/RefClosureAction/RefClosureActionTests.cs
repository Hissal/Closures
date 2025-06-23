using Lh.Closures;

namespace ClosureTests.ActionTests.RefClosureAction;

[TestFixture]
public class RefClosureActionTests {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default tests

    [Test]
    public void RefClosureAction_ReceivesContext() {
        int context = 5;
        int expected = 5;

        var closure = Closure.Action(ref context, (ref int ctx) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke();
    }

    [Test]
    public void RefClosureAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        
        var tupleContext = (testContext, expected);
        var closure = Closure.Action(ref tupleContext, (ref (TestClass testContext, int expected) ctx) => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureAction_AddAndRemove_Works() {
        int context = 1;
        int callSum = 0;
        void Handler(ref int ctx) => callSum += ctx;

        var closure = Closure.Action(ref context, Handler);
        closure.Add(Handler);
        closure.Invoke();
        closure.Remove(Handler);
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(3 * context));
    }

    [Test]
    public void RefClosureAction_Add_MultipleTimes_Works() {
        int context = 2;
        int actionCount = 5;
        int callSum = 0;
        void Handler(ref int ctx) => callSum += ctx;

        var closure = Closure.Action(ref context, Handler);
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(actionCount * context));
    }

    [Test]
    public void RefClosureAction_Remove_MultipleTimes_Works() {
        int context = 3;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context;
        int callSum = 0;
        void Handler(ref int ctx) => callSum += ctx;

        var closure = Closure.Action(ref context, Handler);
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        for (int i = 0; i < actionCount - amountToCall; i++) {
            closure.Remove(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }
    
    // Ref closure tests

    [Test]
    public void RefClosureAction_ModifiesOriginalContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = Closure.Action(ref context, (ref int ctx) => ctx += addition);
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected)); // Context modified after invoke
    }

    [Test]
    public void RefClosureAction_ModifiesOriginalContext_MultipleActions() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 3;
            
        var closure = Closure.Action(ref context, (ref int ctx) => ctx += addition);
        closure.Add((ref int ctx) => ctx += addition);
        closure.Add((ref int ctx) => ctx += addition);
        
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureAction_ModifiesOriginalContext_MultipleInvocations() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 2;
            
        var closure = Closure.Action(ref context, (ref int ctx) => ctx += addition);
        
        closure.Invoke();
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureAction_ModifyingOriginalContext_ModifiesContextAndRefContext() {
        int context = 5;
        int expected = 7;
        
        var closure = Closure.Action(ref context, (ref int ctx) => { /* no op */ });
        context = expected;
        
        Assert.That(closure.Context, Is.EqualTo(expected));
        Assert.That(closure.RefContext, Is.EqualTo(expected));
    }
}