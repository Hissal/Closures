using Closures;

namespace ClosureTests.ActionTests.RefClosureAction;

public class RefClosureRefActionTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests

    [Test]
    public void RefClosureRefAction_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;

        var closure = RefClosure.RefAction(ref context, (ref int ctx, ref int a) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }

    [Test]
    public void RefClosureRefAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;

        var tupleContext = (testContext, expected);
        var closure = RefClosure.RefAction(ref tupleContext, (ref (TestClass testContext, int expected) ctx, ref int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Argument tests

    [Test]
    public void RefClosureRefAction_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = RefClosure.RefAction(ref context, (ref int ctx, ref int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(ref arg);
    }

    // Ref argument tests
    
    [Test]
    public void RefClosureRefAction_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = RefClosure.RefAction(ref context, (ref int ctx, ref int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(ref arg);
    }

    [Test]
    public void RefClosureRefAction_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;
            
        var closure = RefClosure.RefAction(ref context, (ref int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureRefAction_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;
            
        var closure = RefClosure.RefAction(ref context, (ref int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    // Ref closure tests

    [Test]
    public void RefClosureRefAction_ModifiesOriginalContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = RefClosure.RefAction(ref context, (ref int ctx, ref int arg) => ctx += arg);
        closure.Invoke(addition);
        Assert.That(context, Is.EqualTo(expected)); // Context modified after invoke
    }

    [Test]
    public void RefClosureRefAction_ModifiesOriginalContext_MultipleInvocations() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 2;
            
        var closure = RefClosure.RefAction(ref context, (ref int ctx, ref int arg) => ctx += arg);
        
        closure.Invoke(addition);
        closure.Invoke(addition);
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureRefAction_ModifyingOriginalContext_ModifiesContextAndRefContext() {
        int context = 5;
        int expected = 7;
        
        var closure = RefClosure.RefAction(ref context, (ref int ctx, ref int arg) => ctx += arg);
        context = expected;

        Assert.That(closure.Context, Is.EqualTo(expected));
        Assert.That(closure.RefContext, Is.EqualTo(expected));
    }
}