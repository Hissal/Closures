using Closures;

namespace ClosureTests.FuncTests.RefClosureFuncs;

[TestFixture]
public class RefClosureFuncTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests
    [Test]
    public void RefClosureFunc_ReceivesContext() {
        int context = 5;
        int expected = 5;
        var closure = Closure.Func(ref context, (ref int ctx) => { Assert.That(ctx, Is.EqualTo(expected)); return ctx; });
        closure.Invoke();
    }

    [Test]
    public void RefClosureFunc_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        var tupleContext = (testContext, expected);
        var closure = Closure.Func(ref tupleContext, (ref (TestClass testContext, int expected) ctx) => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureFunc_AddAndRemove_Works() {
        int context = 1;
        int callSum = 0;
        int Handler(ref int ctx) => callSum += ctx;
        var closure = Closure.Func(ref context, Handler);
        closure.Add(Handler);
        closure.Invoke();
        closure.Remove(Handler);
        closure.Invoke();
        Assert.That(callSum, Is.EqualTo(3 * context));
    }

    [Test]
    public void RefClosureFunc_Add_MultipleTimes_Works() {
        int context = 2;
        int actionCount = 5;
        int callSum = 0;
        int Handler(ref int ctx) => callSum += ctx;
        var closure = Closure.Func(ref context, Handler);
        for (int i = 0; i < actionCount - 1; i++) closure.Add(Handler);
        closure.Invoke();
        Assert.That(callSum, Is.EqualTo(actionCount * context));
    }

    [Test]
    public void RefClosureFunc_Remove_MultipleTimes_Works() {
        int context = 3;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context;
        int callSum = 0;
        int Handler(ref int ctx) => callSum += ctx;
        var closure = Closure.Func(ref context, Handler);
        for (int i = 0; i < actionCount - 1; i++) closure.Add(Handler);
        for (int i = 0; i < actionCount - amountToCall; i++) closure.Remove(Handler);
        closure.Invoke();
        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }

    // Ref closure tests
    [Test]
    public void RefClosureFunc_ModifiesOriginalContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
        var closure = Closure.Func(ref context, (ref int ctx) => ctx += addition);
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureFunc_ModifiesOriginalContext_MultipleDelegates() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 3;
        var closure = Closure.Func(ref context, (ref int ctx) => ctx += addition);
        closure.Add((ref int ctx) => ctx += addition);
        closure.Add((ref int ctx) => ctx += addition);
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureFunc_ModifiesOriginalContext_MultipleInvocations() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 2;
        var closure = Closure.Func(ref context, (ref int ctx) => ctx += addition);
        closure.Invoke();
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureFunc_ModifyingOriginalContext_ModifiesContextAndRefContext() {
        int context = 5;
        int expected = 7;
        var closure = Closure.Func(ref context, (ref int ctx) => ctx += 2);
        context = expected;
        Assert.That(closure.Context, Is.EqualTo(expected));
        Assert.That(closure.RefContext, Is.EqualTo(expected));
    }

    // Func tests
    [Test]
    public void RefClosureFunc_ReturnsExpectedValue() {
        int context = 10;
        int expected = context * 2;
        var closure = Closure.Func(ref context, (ref int ctx) => ctx * 2);
        int result = closure.Invoke();
        Assert.That(result, Is.EqualTo(expected));
    }
}