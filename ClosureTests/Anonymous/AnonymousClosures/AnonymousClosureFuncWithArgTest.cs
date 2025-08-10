using Closures;

namespace ClosureTests.Anonymous.AnonymousClosures;

[TestFixture]
[TestOf(typeof(AnonymousClosureFunc<,>))]
public class AnonymousClosureFuncWithArgTest {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests

    [Test]
    public void AnonymousClosureFuncWithArg_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), (int ctx, int a) => {
            Assert.That(ctx, Is.EqualTo(expected));
            return ctx + a;
        });
        anon.Invoke(arg);
    }

    [Test]
    public void AnonymousClosureFuncWithArg_ReceivesTupleContext_AndModifiesTestContextValue() {
        int expected = 5;
        int arg = 1;
        var testContext = new TestClass();

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From((testContext, expected)),
            ((TestClass testContext, int expected) ctx, int a) => {
                ctx.testContext.Value = ctx.expected;
                return ctx.expected + a;
            });
        anon.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Argument tests

    [Test]
    public void AnonymousClosureFuncWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), (int ctx, int a) => {
            Assert.That(ctx + a, Is.EqualTo(expected));
            return ctx + a;
        });
        anon.Invoke(arg);
    }

    // Ref argument tests

    [Test]
    public void AnonymousClosureFuncWithArg_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), (int ctx, ref int a) => {
            Assert.That(ctx + a, Is.EqualTo(expected));
            return ctx + a;
        });
        anon.Invoke(ref arg);
    }

    [Test]
    public void AnonymousClosureFuncWithArg_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), (int ctx, ref int val) => {
            val += ctx;
            return val;
        });
        anon.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    [Test]
    public void AnonymousClosureFuncWithArg_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), (int ctx, ref int val) => {
            val += ctx;
            return val;
        });
        anon.Invoke(ref arg);
        anon.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    // Mutating context tests

    [Test]
    public void AnonymousClosureFuncWithArg_MutatingContext_RetainsModifiedContext() {
        int context = 10;
        int arg = 3;
        int addition = 7;
        int expected = context + addition;

        var anon = AnonymousClosure.Func<int, int>(
            AnonymousValue.From(context),
            (ref int ctx, int a) => {
                ctx += addition;
                return ctx + a;
            },
            MutatingBehaviour.Mutate
        );
        anon.Invoke(arg);
        Assert.That(anon.Context.As<int>(), Is.EqualTo(expected));
    }

    [Test]
    public void AnonymousClosureFuncWithArg_MutatingTupleContext_RetainsModifiedContext() {
        var testContext = new TestClass();
        int expected = 42;
        int arg = 0;

        var anon = AnonymousClosure.Func<int, int>(
            AnonymousValue.From((testContext, 0)),
            (ref (TestClass testContext, int value) ctx, int a) => {
                ctx.testContext.Value = ctx.value = expected;
                return ctx.value + a;
            },
            MutatingBehaviour.Mutate
        );
        anon.Invoke(arg);

        Assert.Multiple(() => {
            Assert.That(testContext.Value, Is.EqualTo(expected));
            Assert.That(anon.Context.As<(TestClass, int)>().Item2, Is.EqualTo(expected));
        });
    }

    // Func tests

    [Test]
    public void AnonymousClosureFuncWithArg_ReturnsExpectedValue() {
        int context = 10;
        int arg = 5;
        int expected = context * arg;

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), (int ctx, int a) => ctx * a);
        int result = anon.Invoke(arg);
        Assert.That(result, Is.EqualTo(expected));
    }

    // Anonymous Func tests

    [Test]
    public void AnonymousClosureFuncWithArg_TryInvoke_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int arg = 3;
        int expected = context * arg;

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), (int ctx, int a) => ctx * a);

        var result = anon.TryInvoke(arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(expected));
        });
    }

    [Test]
    public void AnonymousClosureFuncWithArg_TryInvoke_ReturnsFailure_OnException() {
        int context = 1;
        int arg = 2;
        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context),
            (Func<int, int, int>)((int ctx, int a) => throw new InvalidCastException("fail")));

        var result = anon.TryInvoke(arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }

    [Test]
    public void AnonymousClosureFuncWithArg_TryInvokeBool_ReturnsTrueAndResult_OnValidCall() {
        int context = 5;
        int arg = 3;
        int expected = context * arg;

        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), (int ctx, int a) => ctx * a);

        bool success = anon.TryInvoke(arg, out int result);

        Assert.Multiple(() => {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(expected));
        });
    }

    [Test]
    public void AnonymousClosureFuncWithArg_TryInvokeBool_ReturnsFalseAndDefault_OnException() {
        int context = 1;
        int arg = 2;
        var anon = AnonymousClosure.Func<int, int>(AnonymousValue.From(context),
            (Func<int, int, int>)((int ctx, int a) => throw new InvalidCastException("fail")));

        bool success = anon.TryInvoke(arg, out int result);

        Assert.Multiple(() => {
            Assert.That(success, Is.False);
            Assert.That(result, Is.EqualTo(default(int)));
        });
    }
    
    // Anonymous Tests
    
    [Test]
    public void AnonymousClosureFunc_Is_Works() {
        var anon = AnonymousClosure.Func(42, (ref int ctx, ref int arg) => ctx += arg);

        Assert.Multiple(() => {
            Assert.That(anon.Is<MutatingClosureRefFunc<int, int, int>>(), Is.True);
            Assert.That(anon.Is<ClosureRefFunc<int, int, int>>(), Is.False);
        });
    }

    [Test]
    public void AnonymousClosureFuncWithArg_Equals_And_HashCode_Work() {
        int context = 7;
        Func<int, int, int> func = (ctx, arg) => ctx + arg;

        var a = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), func);
        a.Invoke(1);

        var b = AnonymousClosure.Func<int, int>(AnonymousValue.From(context), func);

        Assert.That(a, Is.EqualTo(b));
        Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }
}