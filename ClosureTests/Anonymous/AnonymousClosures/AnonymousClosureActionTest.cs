using Closures;

namespace ClosureTests.Anonymous.AnonymousClosures;

[TestFixture]
[TestOf(typeof(AnonymousClosureAction))]
public class AnonymousClosureActionTest {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests

    [Test]
    public void AnonymousClosureAction_ReceivesContext() {
        int context = 5;
        int expected = 5;

        var anon = AnonymousClosure.Action(AnonymousValue.From(context),
            (int ctx) => Assert.That(ctx, Is.EqualTo(expected)));
        anon.Invoke();
    }

    [Test]
    public void AnonymousClosureAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        int expected = 5;
        var testContext = new TestClass();

        var anon = AnonymousClosure.Action(AnonymousValue.From((testContext, expected)),
            ((TestClass testContext, int expected) ctx) => { ctx.testContext.Value = ctx.expected; });
        anon.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Mutating context tests

    [Test]
    public void AnonymousClosureAction_MutatingContext_RetainsModifiedContext() {
        int context = 10;
        int addition = 7;
        int expected = context + addition;

        var anon = AnonymousClosure.Action(
            AnonymousValue.From(context),
            (ref int ctx) => { ctx += addition; },
            MutatingBehaviour.Mutate
        );
        anon.Invoke();
        Assert.That(anon.Context.As<int>(), Is.EqualTo(expected));
    }

    [Test]
    public void AnonymousClosureAction_MutatingTupleContext_RetainsModifiedContext() {
        var testContext = new TestClass();
        int expected = 42;

        var anon = AnonymousClosure.Action(
            AnonymousValue.From((testContext, 0)),
            (ref (TestClass testContext, int value) ctx) => { ctx.testContext.Value = ctx.value = expected; },
            MutatingBehaviour.Mutate
        );
        anon.Invoke();

        Assert.Multiple(() => {
            Assert.That(testContext.Value, Is.EqualTo(expected));
            Assert.That(anon.Context.As<(TestClass, int)>().Item2, Is.EqualTo(expected));
        });
    }

    // Anonymous Action Tests

    [Test]
    public void AnonymousClosureAction_TryInvoke_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int calledWith = 0;
        Action<int> action = ctx => calledWith = ctx * 2;

        var anon = AnonymousClosure.Action(AnonymousValue.From(context), action);

        var result = anon.TryInvoke();

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(calledWith, Is.EqualTo(10));
        });
    }

    [Test]
    public void AnonymousClosureAction_TryInvoke_ReturnsFailure_OnException() {
        int context = 1;
        Action<int> action = ctx => throw new InvalidCastException("fail");

        var anon = AnonymousClosure.Action(AnonymousValue.From(context), action);

        var result = anon.TryInvoke();

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }
    
    // Anonymous Tests
    
    [Test]
    public void AnonymousClosureAction_Is_Works() {
        var anon = AnonymousClosure.Action(AnonymousValue.From(42), (int ctx) => { });

        Assert.Multiple(() => {
            Assert.That(anon.Is<ClosureAction<int>>(), Is.True);
            Assert.That(anon.Is<MutatingClosureFunc<int, int>>(), Is.False);
        });
    }
    
    [Test]
    public void AnonymousClosureAction_Equals_And_HashCode_Work() {
        int context = 7;
        Action<int> action = ctx => { };

        var a = AnonymousClosure.Action(AnonymousValue.From(context), action);
        // Invoke to cache the invoker delegate (shouldn't affect equality)
        a.Invoke();

        var b = AnonymousClosure.Action(AnonymousValue.From(context), action);

        Assert.That(a, Is.EqualTo(b));
        Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }
}