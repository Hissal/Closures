using Closures;

namespace ClosureTests.ActionTests.MutatingClosureAction;

[TestFixture]
public class MutatingClosureRefActionTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests
    
    [Test]
    public void MutatingClosureRefAction_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;

        var closure = Closure.Action(context, (ref int ctx, ref int a) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }

    [Test]
    public void MutatingClosureRefAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;

        var closure = Closure.Action((testContext, expected), (ref (TestClass testContext, int expected) ctx, ref int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    [Test]
    public void MutatingClosureRefAction_AddAndRemove_Works() {
        int context = 1;
        int arg = 2;
        int callSum = 0;
        void Handler(ref int ctx, ref int a) => callSum += ctx + a;

        var closure = Closure.Action<int, int>(context, Handler);
        closure.Add(Handler);
        closure.Invoke(arg);
        closure.Remove(Handler);
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(3 * (context + arg)));
    }

    [Test]
    public void MutatingClosureRefAction_Add_MultipleTimes_Works() {
        int context = 2;
        int arg = 3;
        int actionCount = 5;
        int callSum = 0;
        void Handler(ref int ctx, ref int a) => callSum += ctx + a;

        var closure = Closure.Action<int, int>(context, Handler);
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(actionCount * (context + arg)));
    }

    [Test]
    public void MutatingClosureRefAction_Remove_MultipleTimes_Works() {
        int context = 3;
        int arg = 4;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context + arg;
        int callSum = 0;
        void Handler(ref int ctx, ref int a) => callSum += ctx + a;

        var closure = Closure.Action<int, int>(context, Handler);
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
    public void MutatingClosureRefAction_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = Closure.Action(context, (ref int ctx, ref int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }
    
    // Ref argument tests
    
    [Test]
    public void MutatingClosureRefAction_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = Closure.Action(context, (int ctx, ref int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(ref arg);
    }
    
    [Test]
    public void MutatingClosureRefAction_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;
            
        var closure = Closure.Action(context, (int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    [Test]
    public void MutatingClosureRefAction_ModifiesRefArgValue_MultipleDelegates() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 3;
            
        var closure = Closure.Action(context, (int ctx, ref int val) => val += ctx);
        closure.Add((int ctx, ref int val) => val += ctx);
        closure.Add((int ctx, ref int val) => val += ctx);
        
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    [Test]
    public void MutatingClosureRefAction_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;
            
        var closure = Closure.Action(context, (int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    // Mutating tests

    [Test]
    public void MutatingClosureRefAction_Retain_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = Closure.Action(context, (ref int ctx, ref int arg) => ctx += arg, MutatingClosureBehaviour.Retain);
        closure.Invoke(addition);
            
        Assert.That(closure.Context, Is.EqualTo(expected));
    }
    
    [Test]
    public void MutatingClosureRefAction_Reset_ResetsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context;
            
        var closure = Closure.Action(context, (ref int ctx, ref int arg) => ctx += arg, MutatingClosureBehaviour.Reset);
        closure.Invoke(addition);
            
        Assert.That(closure.Context, Is.EqualTo(expected));
    }
}