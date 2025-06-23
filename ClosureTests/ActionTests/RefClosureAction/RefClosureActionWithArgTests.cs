using Lh.Closures;

namespace ClosureTests.ActionTests.RefClosureAction;

[TestFixture]
public class RefClosureActionWithArgTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests

    [Test]
    public void RefClosureActionWithArg_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;

        var closure = Closure.Action(ref context, (ref int ctx, int a) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }

    [Test]
    public void RefClosureActionWithArg_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;

        var tupleContext = (testContext, expected);
        var closure = Closure.Action(ref tupleContext, (ref (TestClass testContext, int expected) ctx, int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureActionWithArg_AddAndRemove_Works() {
        int context = 1;
        int arg = 2;
        int callSum = 0;
        void Handler(ref int ctx, int a) => callSum += ctx + a;

        var closure = Closure.Action<int, int>(ref context, Handler);
        closure.Add(Handler);
        closure.Invoke(arg);
        closure.Remove(Handler);
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(3 * (context + arg)));
    }

    [Test]
    public void RefClosureActionWithArg_Add_MultipleTimes_Works() {
        int context = 2;
        int arg = 3;
        int actionCount = 5;
        int callSum = 0;
        void Handler(ref int ctx, int a) => callSum += ctx + a;

        var closure = Closure.Action<int, int>(ref context, Handler);
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(actionCount * (context + arg)));
    }

    [Test]
    public void RefClosureActionWithArg_Remove_MultipleTimes_Works() {
        int context = 3;
        int arg = 4;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context + arg;
        int callSum = 0;
        void Handler(ref int ctx, int a) => callSum += ctx + a;

        var closure = Closure.Action<int, int>(ref context, Handler);
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        for (int i = 0; i < actionCount - amountToCall; i++) {
            closure.Remove(Handler);
        }
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }

    // Argument tests

    [Test]
    public void RefClosureActionWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = Closure.Action(ref context, (ref int ctx, int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }
    
    // Ref closure tests

    [Test]
    public void RefClosureActionWithArg_ModifiesOriginalContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = Closure.Action(ref context, (ref int ctx, int arg) => ctx += arg);
        closure.Invoke(addition);
        Assert.That(context, Is.EqualTo(expected)); // Context modified after invoke
    }

    [Test]
    public void RefClosureActionWithArg_ModifiesOriginalContext_MultipleActions() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 3;
            
        var closure = Closure.Action(ref context, (ref int ctx, int arg) => ctx += arg);
        closure.Add((ref int ctx, int arg) => ctx += arg);
        closure.Add((ref int ctx, int arg) => ctx += arg);
        
        closure.Invoke(addition);
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureActionWithArg_ModifiesOriginalContext_MultipleInvocations() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 2;
            
        var closure = Closure.Action(ref context, (ref int ctx, int arg) => ctx += arg);
        
        closure.Invoke(addition);
        closure.Invoke(addition);
        Assert.That(context, Is.EqualTo(expected));
    }
    
    [Test]
    public void RefClosureActionWithArg_ModifyingOriginalContext_ModifiesContextAndRefContext() {
        int context = 5;
        int expected = 7;
        
        var closure = Closure.Action(ref context, (ref int ctx, int arg) => ctx += arg);
        context = expected;

        Assert.That(closure.Context, Is.EqualTo(expected));
        Assert.That(closure.RefContext, Is.EqualTo(expected));
    }
}