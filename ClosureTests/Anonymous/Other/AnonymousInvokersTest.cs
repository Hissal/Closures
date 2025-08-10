using Closures;

namespace ClosureTests.Anonymous.Other;

[TestFixture]
[TestOf(typeof(AnonymousInvokers))]
public class AnonymousInvokersTest {
    [Test]
    public void GetActionInvoker_InvokesNormalAction() {
        int called = 0;
        Action<int> action = ctx => called = ctx + 1;
        var anon = AnonymousValue.From(41);

        var invoker = AnonymousInvokers.GetActionInvoker(action);
        invoker(action, ref anon, MutatingBehaviour.Reset);

        Assert.Multiple(() => {
            Assert.That(called, Is.EqualTo(42));
            Assert.That(anon.As<int>(), Is.EqualTo(41)); // context not mutated
        });
    }

    [Test]
    public void GetActionInvoker_InvokesMutatingAction() {
        RefAction<int> action = (ref int ctx) => ctx += 10;
        var anon = AnonymousValue.From(5);

        var invoker = AnonymousInvokers.GetActionInvoker(action);
        invoker(action, ref anon, MutatingBehaviour.Mutate);

        Assert.That(anon.As<int>(), Is.EqualTo(15));
    }

    [Test]
    public void GetActionInvokerTArg_InvokesNormalActionWithArg() {
        int called = 0;
        Action<int, int> action = (ctx, arg) => called = ctx + arg;
        var anon = AnonymousValue.From(10);
        int arg = 5;

        var invoker = AnonymousInvokers.GetActionInvoker<int>(action);
        invoker(action, ref anon, MutatingBehaviour.Reset, ref arg);

        Assert.Multiple(() => {
            Assert.That(called, Is.EqualTo(15));
            Assert.That(anon.As<int>(), Is.EqualTo(10)); // context not mutated
        });
    }

    [Test]
    public void GetActionInvokerTArg_InvokesMutatingActionWithArg() {
        ActionWithRefContext<int, int> action = (ref int ctx, int arg) => ctx += arg;
        var anon = AnonymousValue.From(7);
        int arg = 3;

        var invoker = AnonymousInvokers.GetActionInvoker<int>(action);
        invoker(action, ref anon, MutatingBehaviour.Mutate, ref arg);

        Assert.That(anon.As<int>(), Is.EqualTo(10));
    }

    [Test]
    public void GetFuncInvoker_InvokesNormalFunc() {
        Func<int, int> func = ctx => ctx * 2;
        var anon = AnonymousValue.From(21);

        var invoker = AnonymousInvokers.GetFuncInvoker<int>(func);
        var result = invoker(func, ref anon, MutatingBehaviour.Reset);

        Assert.Multiple(() => {
            Assert.That(result, Is.EqualTo(42));
            Assert.That(anon.As<int>(), Is.EqualTo(21)); // context not mutated
        });
    }

    [Test]
    public void GetFuncInvoker_InvokesMutatingFunc() {
        RefFunc<int, int> func = (ref int ctx) => ++ctx;
        var anon = AnonymousValue.From(99);

        var invoker = AnonymousInvokers.GetFuncInvoker<int>(func);
        var result = invoker(func, ref anon, MutatingBehaviour.Mutate);

        Assert.Multiple(() => {
            Assert.That(result, Is.EqualTo(100));
            Assert.That(anon.As<int>(), Is.EqualTo(100));
        });
    }

    [Test]
    public void GetFuncInvokerTArgTReturn_InvokesNormalFuncWithArg() {
        Func<int, int, int> func = (ctx, arg) => ctx + arg;
        var anon = AnonymousValue.From(10);
        int arg = 5;

        var invoker = AnonymousInvokers.GetFuncInvoker<int, int>(func);
        var result = invoker(func, ref anon, MutatingBehaviour.Reset, ref arg);

        Assert.Multiple(() => {
            Assert.That(result, Is.EqualTo(15));
            Assert.That(anon.As<int>(), Is.EqualTo(10)); // context not mutated
        });
    }

    [Test]
    public void GetFuncInvokerTArgTReturn_InvokesMutatingFuncWithArg() {
        RefFunc<int, int, int> func = (ref int ctx, ref int arg) => ctx += arg;
        var anon = AnonymousValue.From(3);
        int arg = 4;

        var invoker = AnonymousInvokers.GetFuncInvoker<int, int>(func);
        var result = invoker(func, ref anon, MutatingBehaviour.Mutate, ref arg);

        Assert.Multiple(() => {
            Assert.That(result, Is.EqualTo(7));
            Assert.That(anon.As<int>(), Is.EqualTo(7));
        });
    }

    [Test]
    public void GetActionInvoker_ThrowsOnInvalidDelegate() {
        Func<int, int> func = ctx => ctx;
        Assert.Throws<ArgumentException>(() => AnonymousInvokers.GetActionInvoker(func));
    }

    [Test]
    public void GetFuncInvoker_ThrowsOnInvalidReturnType() {
        Func<int, int> func = ctx => ctx;
        Assert.Throws<InvalidCastException>(() => AnonymousInvokers.GetFuncInvoker<string>(func));
    }

    [Test]
    public void GetFuncInvokerTArgTReturn_ThrowsOnInvalidArgType() {
        Func<int, int, int> func = (ctx, arg) => ctx + arg;
        Assert.Throws<InvalidCastException>(() => AnonymousInvokers.GetFuncInvoker<string, int>(func));
    }
}