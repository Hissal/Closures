using Closures;

namespace ClosureTests;

[TestFixture]
[TestOf(typeof(ClosureExtensions))]
public class ClosureToDelegateConversionTest {
    // Action tests
    [Test]
    public void ClosureAction_AsAction() {
        int context = 10;
        int expected = 10;
        int calledWith = 0;
        
        var closure = Closure.Action(context, ctx => calledWith = ctx);
        
        Action action = closure.AsAction();
        action.Invoke();
        
        Assert.That(calledWith, Is.EqualTo(expected));
    }

    [Test]
    public void ClosureActionWithArg_AsAction() {
        int context = 20;
        int arg = 5;
        int expected = context + arg;
        int calledWith = 0;
        
        var closure = Closure.Action(context, (int ctx, int a) => calledWith = ctx + a);
        Action<int> action = closure.AsAction();
        action.Invoke(arg);
        
        Assert.That(calledWith, Is.EqualTo(expected));
    }
    
    // Func tests
    [Test]
    public void ClosureFunc_AsFunc() {
        int context = 15;
        int expected = 25;

        var closure = Closure.Func(context, ctx => ctx + 10);
        Func<int> func = closure.AsFunc();
        int result = func.Invoke();

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ClosureFuncWithArg_AsFunc() {
        int context = 30;
        int arg = 10;
        int expected = context + arg;

        var closure = Closure.Func(context, (int ctx, int a) => ctx + a);
        Func<int, int> func = closure.AsFunc();
        int result = func.Invoke(arg);
        
        Assert.That(result, Is.EqualTo(expected));
    }
}