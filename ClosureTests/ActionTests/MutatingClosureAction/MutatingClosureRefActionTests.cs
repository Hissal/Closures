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

        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int a) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }

    [Test]
    public void MutatingClosureRefAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;

        var closure = MutatingClosure.RefAction((testContext, expected), (ref (TestClass testContext, int expected) ctx, ref int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
    
    // Argument tests

    [Test]
    public void MutatingClosureRefAction_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;

        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }

    // Ref argument tests

    [Test]
    public void MutatingClosureRefAction_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;

        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(ref arg);
    }

    [Test]
    public void MutatingClosureRefAction_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;

        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    [Test]
    public void MutatingClosureRefAction_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;

        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    // Mutating tests

    [Test]
    public void MutatingClosureRefAction_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;

        var closure = MutatingClosure.RefAction(context, (ref int ctx, ref int arg) => ctx += arg);
        closure.Invoke(addition);

        Assert.That(closure.Context, Is.EqualTo(expected));
    }
}