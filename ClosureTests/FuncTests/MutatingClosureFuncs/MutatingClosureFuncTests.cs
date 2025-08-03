using Closures;

namespace ClosureTests.FuncTests.MutatingClosureFuncs;

[TestFixture]
public class MutatingClosureFuncTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests
    [Test]
    public void MutatingClosureFunc_ReceivesContext() {
        int context = 5;
        int expected = 5;
        var closure = Closure.Func(context, (ref int ctx) => { Assert.That(ctx, Is.EqualTo(expected)); return ctx; });
        closure.Invoke();
    }

    [Test]
    public void MutatingClosureFunc_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        var closure = Closure.Func((testContext, expected), (ref (TestClass testContext, int expected) ctx) => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    [Test]
    public void MutatingClosureFunc_AddAndRemove_Works() {
        int context = 1;
        int callSum = 0;
        int Handler(ref int ctx) => callSum += ctx;
        var closure = Closure.Func(context, Handler);
        closure.Add(Handler);
        closure.Invoke();
        closure.Remove(Handler);
        closure.Invoke();
        Assert.That(callSum, Is.EqualTo(3 * context));
    }

    [Test]
    public void MutatingClosureFunc_Add_MultipleTimes_Works() {
        int context = 2;
        int actionCount = 5;
        int callSum = 0;
        int Handler(ref int ctx) => callSum += ctx;
        var closure = Closure.Func(context, Handler);
        for (int i = 0; i < actionCount - 1; i++) closure.Add(Handler);
        closure.Invoke();
        Assert.That(callSum, Is.EqualTo(actionCount * context));
    }

    [Test]
    public void MutatingClosureFunc_Remove_MultipleTimes_Works() {
        int context = 3;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context;
        int callSum = 0;
        int Handler(ref int ctx) => callSum += ctx;
        var closure = Closure.Func(context, Handler);
        for (int i = 0; i < actionCount - 1; i++) closure.Add(Handler);
        for (int i = 0; i < actionCount - amountToCall; i++) closure.Remove(Handler);
        closure.Invoke();
        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }

    // Mutating tests
    [Test]
    public void MutatingClosureFunc_Retain_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
        var closure = Closure.Func(context, (ref int ctx) => ctx += addition, MutatingClosureBehaviour.Retain);
        closure.Invoke();
        Assert.That(closure.Context, Is.EqualTo(expected));
    }

    [Test]
    public void MutatingClosureFunc_Reset_ResetsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context;
        var closure = Closure.Func(context, (ref int ctx) => ctx += addition, MutatingClosureBehaviour.Reset);
        closure.Invoke();
        Assert.That(closure.Context, Is.EqualTo(expected));
    }

    // Func tests
    [Test]
    public void MutatingClosureFunc_ReturnsExpectedValue() {
        int context = 10;
        int expected = context * 2;
        var closure = Closure.Func(context, (ref int ctx) => ctx * 2);
        int result = closure.Invoke();
        Assert.That(result, Is.EqualTo(expected));
    }
}