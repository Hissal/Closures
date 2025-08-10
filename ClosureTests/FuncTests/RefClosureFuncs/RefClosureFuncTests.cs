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
        var closure = RefClosure.Func(ref context, (ref int ctx) => { Assert.That(ctx, Is.EqualTo(expected)); return ctx; });
        closure.Invoke();
    }

    [Test]
    public void RefClosureFunc_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        var tupleContext = (testContext, expected);
        var closure = RefClosure.Func(ref tupleContext, (ref (TestClass testContext, int expected) ctx) => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Ref closure tests
    [Test]
    public void RefClosureFunc_ModifiesOriginalContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
        var closure = RefClosure.Func(ref context, (ref int ctx) => ctx += addition);
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureFunc_ModifiesOriginalContext_MultipleInvocations() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 2;
        var closure = RefClosure.Func(ref context, (ref int ctx) => ctx += addition);
        closure.Invoke();
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureFunc_ModifyingOriginalContext_ModifiesContextAndRefContext() {
        int context = 5;
        int expected = 7;
        var closure = RefClosure.Func(ref context, (ref int ctx) => ctx += 2);
        context = expected;
        Assert.That(closure.Context, Is.EqualTo(expected));
        Assert.That(closure.RefContext, Is.EqualTo(expected));
    }

    // Func tests
    [Test]
    public void RefClosureFunc_ReturnsExpectedValue() {
        int context = 10;
        int expected = context * 2;
        var closure = RefClosure.Func(ref context, (ref int ctx) => ctx * 2);
        int result = closure.Invoke();
        Assert.That(result, Is.EqualTo(expected));
    }
}