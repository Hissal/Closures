using Closures;

namespace ClosureTests.FuncTests.RefClosureFuncs;

[TestFixture]
public class RefClosureFuncWithArgTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests
    [Test]
    public void RefClosureFuncWithArg_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;
        var closure = RefClosure.Func(ref context, (ref int ctx, int a) => { Assert.That(ctx, Is.EqualTo(expected)); return ctx; });
        closure.Invoke(arg);
    }

    [Test]
    public void RefClosureFuncWithArg_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;
        var tupleContext = (testContext, expected);
        var closure = RefClosure.Func(ref tupleContext, (ref (TestClass testContext, int expected) ctx, int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Argument tests
    [Test]
    public void RefClosureFuncWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
        var closure = RefClosure.Func(ref context, (ref int ctx, int a) => { Assert.That(ctx + a, Is.EqualTo(expected)); return ctx; });
        closure.Invoke(arg);
    }

    // Ref closure tests
    [Test]
    public void RefClosureFuncWithArg_ModifiesOriginalContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
        var closure = RefClosure.Func(ref context, (ref int ctx, int a) => ctx += addition);
        closure.Invoke(0);
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureFuncWithArg_ModifiesOriginalContext_MultipleInvocations() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 2;
        var closure = RefClosure.Func(ref context, (ref int ctx, int a) => ctx += addition);
        closure.Invoke(0);
        closure.Invoke(0);
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureFuncWithArg_ModifyingOriginalContext_ModifiesContextAndRefContext() {
        int context = 5;
        int expected = 7;
        var closure = RefClosure.Func(ref context, (ref int ctx, int a) => ctx += 2);
        context = expected;
        Assert.That(closure.Context, Is.EqualTo(expected));
        Assert.That(closure.RefContext, Is.EqualTo(expected));
    }

    // Func tests
    [Test]
    public void RefClosureFuncWithArg_ReturnsExpectedValue() {
        int context = 10;
        int addition = 5;
        int expected = context + addition;
        var closure = RefClosure.Func(ref context, (ref int ctx, int arg) => ctx + arg);
        int result = closure.Invoke(addition);
        Assert.That(result, Is.EqualTo(expected));
    }
}