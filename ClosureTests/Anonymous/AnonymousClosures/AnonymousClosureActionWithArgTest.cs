using Closures;
using Closures.Anonymous;

namespace ClosureTests.Anonymous.AnonymousClosures;

[TestFixture]
[TestOf(typeof(AnonymousClosureAction<>))]
public class AnonymousClosureActionWithArgTest {

    class TestClass {
        public int Value { get; set; }
    }

    // Default tests

    [Test]
    public void AnonymousClosureActionWithArg_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), (int ctx, int a) => { Assert.That(ctx, Is.EqualTo(expected)); });
        anon.Invoke(arg);
    }

    [Test]
    public void AnonymousClosureActionWithArg_ReceivesTupleContext_AndModifiesTestContextValue() {
        int expected = 5;
        int arg = 1;
        var testContext = new TestClass();

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From((testContext, expected)), ((TestClass testContext, int expected) ctx, int a) => { ctx.testContext.Value = ctx.expected; });
        anon.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Argument tests

    [Test]
    public void AnonymousClosureActionWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), (int ctx, int a) => { Assert.That(ctx + a, Is.EqualTo(expected)); });
        anon.Invoke(arg);
    }

    // Ref argument tests

    [Test]
    public void AnonymousClosureActionWithArg_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), (int ctx, ref int a) => { Assert.That(ctx + a, Is.EqualTo(expected)); });
        anon.Invoke(ref arg);
    }

    [Test]
    public void AnonymousClosureActionWithArg_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), (int ctx, ref int val) => { val += ctx; });
        anon.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    [Test]
    public void AnonymousClosureActionWithArg_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), (int ctx, ref int val) => { val += ctx; });
        anon.Invoke(ref arg);
        anon.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    // Mutating context tests

    [Test]
    public void AnonymousClosureActionWithArg_MutatingContext_RetainsModifiedContext() {
        int context = 10;
        int arg = 3;
        int addition = 7;
        int expected = context + addition;

        var anon = AnonymousClosure.Action<int>(
            AnonymousValue.From(context),
            (ref int ctx, int a) => { ctx += addition; },
            MutatingBehaviour.Mutate
        );
        anon.Invoke(arg);
        Assert.That(anon.Context.As<int>(), Is.EqualTo(expected));
    }

    [Test]
    public void AnonymousClosureActionWithArg_MutatingTupleContext_RetainsModifiedContext() {
        var testContext = new TestClass();
        int expected = 42;
        int arg = 0;

        var anon = AnonymousClosure.Action<int>(
            AnonymousValue.From((testContext, 0)),
            (ref (TestClass testContext, int value) ctx, int a) => { ctx.testContext.Value = ctx.value = expected; },
            MutatingBehaviour.Mutate
        );
        anon.Invoke(arg);

        Assert.Multiple(() => {
            Assert.That(testContext.Value, Is.EqualTo(expected));
            Assert.That(anon.Context.As<(TestClass, int)>().Item2, Is.EqualTo(expected));
        });
    }

    // Anonymous Action Tests

    [Test]
    public void AnonymousClosureActionWithArg_TryInvoke_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int calledWith = 0;
        Action<int, int> action = (ctx, arg) => { calledWith = ctx * arg; };

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), action);

        var result = anon.TryInvoke(3);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(calledWith, Is.EqualTo(15));
        });
    }

    [Test]
    public void AnonymousClosureActionWithArg_TryInvoke_ByRef_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int calledWith = 0;
        Action<int, int> action = (ctx, arg) => { calledWith = ctx * arg; };

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), action);

        int arg = 4;
        var result = anon.TryInvoke(ref arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(calledWith, Is.EqualTo(20));
        });
    }

    [Test]
    public void AnonymousClosureActionWithArg_TryInvoke_ReturnsFailure_OnException() {
        int context = 1;
        Action<int, int> action = (ctx, arg) => { throw new InvalidCastException("fail"); };

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), action);

        var result = anon.TryInvoke(2);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }

    [Test]
    public void AnonymousClosureActionWithArg_TryInvoke_ByRef_ReturnsFailure_OnException() {
        int context = 1;
        Action<int, int> action = (ctx, arg) => { throw new InvalidCastException("fail"); };

        var anon = AnonymousClosure.Action<int>(AnonymousValue.From(context), action);

        int arg = 2;
        var result = anon.TryInvoke(ref arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }
        
    // Anonymous Tests
    
    [Test]
    public void AnonymousClosureActionWithArg_Is_Works() {
        var anon = AnonymousClosure.Action(42, (int ctx, ref int arg) => { });

        Assert.Multiple(() => {
            Assert.That(anon.Is<ClosureRefAction<int, int>>(), Is.True);
            Assert.That(anon.Is<ClosureAction<int, int>>(), Is.False);
        });
    }
    
    [Test]
    public void AnonymousClosureActionWithArg_Equals_And_HashCode_Work() {
        int context = 7;
        Action<int, int> action = (ctx, arg) => { };

        var a = AnonymousClosure.Action<int>(AnonymousValue.From(context), action);
        // Invoke to cache the invoker delegate (shouldn't affect equality)
        int arg = 1;
        a.Invoke(arg);

        var b = AnonymousClosure.Action<int>(AnonymousValue.From(context), action);

        Assert.That(a, Is.EqualTo(b));
        Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }
}