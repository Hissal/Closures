using Closures;

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

        var closure = RefClosure.Action(ref context, (ref int ctx, int a) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }

    [Test]
    public void RefClosureActionWithArg_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;

        var tupleContext = (testContext, expected);
        var closure = RefClosure.Action(ref tupleContext, (ref (TestClass testContext, int expected) ctx, int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Argument tests

    [Test]
    public void RefClosureActionWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = RefClosure.Action(ref context, (ref int ctx, int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }
    
    // Ref closure tests

    [Test]
    public void RefClosureActionWithArg_ModifiesOriginalContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = RefClosure.Action(ref context, (ref int ctx, int arg) => ctx += arg);
        closure.Invoke(addition);
        Assert.That(context, Is.EqualTo(expected)); // Context modified after invoke
    }

    [Test]
    public void RefClosureActionWithArg_ModifiesOriginalContext_MultipleInvocations() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 2;
            
        var closure = RefClosure.Action(ref context, (ref int ctx, int arg) => ctx += arg);
        
        closure.Invoke(addition);
        closure.Invoke(addition);
        Assert.That(context, Is.EqualTo(expected));
    }
    
    [Test]
    public void RefClosureActionWithArg_ModifyingOriginalContext_ModifiesContextAndRefContext() {
        int context = 5;
        int expected = 7;
        
        var closure = RefClosure.Action(ref context, (ref int ctx, int arg) => ctx += arg);
        context = expected;

        Assert.That(closure.Context, Is.EqualTo(expected));
        Assert.That(closure.RefContext, Is.EqualTo(expected));
    }
}