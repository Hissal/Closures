using Closures;
using Closures.Anonymous;

namespace ClosureTests.Anonymous.AnonymousClosures;

[TestFixture]
[TestOf(typeof(AnonymousClosureFunc<>))]
public class AnonymousClosureFuncTest {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests

    [Test]
    public void AnonymousClosureFunc_ReceivesContext() {
        int context = 5;
        int expected = 5;

        var anon = AnonymousClosure.Func<int>(AnonymousValue.From(context), (int ctx) => {
            Assert.That(ctx, Is.EqualTo(expected));
            return ctx;
        });

        anon.Invoke();
    }

    [Test]
    public void AnonymousClosureFunc_ReceivesTupleContext_AndModifiesTestContextValue() {
        int expected = 5;
        var testContext = new TestClass();

        var anon = AnonymousClosure.Func<int>(AnonymousValue.From((testContext, expected)),
            ((TestClass testContext, int expected) ctx) => {
                ctx.testContext.Value = ctx.expected;
                return ctx.expected;
            });

        anon.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Func tests

    [Test]
    public void AnonymousClosureFunc_ReturnsExpectedValue() {
        int context = 10;
        int expected = context * 2;

        var anon = AnonymousClosure.Func<int>(AnonymousValue.From(context), (int ctx) => ctx * 2);

        int result = anon.Invoke();
        Assert.That(result, Is.EqualTo(expected));
    }

    // Anonymous Func tests

    [Test]
    public void AnonymousClosureFunc_TryInvoke_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int expected = context * 3;

        var anon = AnonymousClosure.Func<int>(AnonymousValue.From(context), (int ctx) => ctx * 3);

        var result = anon.TryInvoke();
        
        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(expected));
        });
    }

    [Test]
    public void AnonymousClosureFunc_TryInvoke_ReturnsFailure_OnException() {
        int context = 1;
        var anon = AnonymousClosure.Func<int>(AnonymousValue.From(context),
            (Func<int, int>)((int ctx) => throw new InvalidCastException("fail")));

        var result = anon.TryInvoke();
        
        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }

    [Test]
    public void AnonymousClosureFunc_TryInvokeBool_ReturnsTrueAndResult_OnValidCall() {
        int context = 5;
        int expected = context * 3;

        var anon = AnonymousClosure.Func<int>(AnonymousValue.From(context), (int ctx) => ctx * 3);

        bool success = anon.TryInvoke(out int result);
        
        Assert.Multiple(() => {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(expected));
        });
    }

    [Test]
    public void AnonymousClosureFunc_TryInvokeBool_ReturnsFalseAndDefault_OnException() {
        int context = 1;
        var anon = AnonymousClosure.Func<int>(AnonymousValue.From(context),
            (Func<int, int>)((int ctx) => throw new InvalidCastException("fail")));

        bool success = anon.TryInvoke(out int result);
        
        Assert.Multiple(() => {
            Assert.That(success, Is.False);
            Assert.That(result, Is.EqualTo(default(int)));
        });
    }

    // Anonymous Tests
    
    [Test]
    public void AnonymousClosureFunc_Is_Works() {
        var anon = AnonymousClosure.Func(42, (ref int ctx) => ctx += 1);

        Assert.Multiple(() => {
            Assert.That(anon.Is<MutatingClosureFunc<int, int>>(), Is.True);
            Assert.That(anon.Is<ClosureFunc<int, int>>(), Is.False);
        });
    }
    
    [Test]
    public void AnonymousClosureFunc_InvokableAs_Works() {
        var closure = AnonymousClosure.Func(10, (int ctx) => ctx * 2);

        Assert.Multiple(() => {
            Assert.That(closure.InvokableAs<Func<int>>(), Is.True);
            Assert.That(closure.InvokableAs<Action<int>>(), Is.False);
        });
    }
    
    [Test]
    public void AnonymousClosureFunc_Equals_And_HashCode_Work() {
        int context = 7;
        Func<int, int> func = ctx => ctx + 1;

        var a = AnonymousClosure.Func<int>(AnonymousValue.From(context), func);
        a.Invoke();

        var b = AnonymousClosure.Func<int>(AnonymousValue.From(context), func);

        Assert.That(a, Is.EqualTo(b));
        Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }
}