using Closures;

namespace ClosureTests;

[TestFixture]
[TestOf(typeof(CustomClosure<,>))]
public class CustomClosureTest {
    [Test]
    public void CustomClosure_CreatesCorrectly() {
        int context = 42;
        Func<int, int> func = x => x + 1;
        var closure = Closure.Custom(context, func);

        Assert.Multiple(() => {
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(func));
        });
    }

    [Test]
    public void CustomClosure_DelegateCanBeInvoked() {
        string context = "Hello";
        Func<string, string, string> func = (ctx, arg) => $"{ctx}, {arg}!";
        var closure = Closure.Custom(context, func);

        // Manually invoke the delegate, passing context as the first argument
        var result = closure.Delegate.Invoke(closure.Context, "World");
        Assert.That(result, Is.EqualTo("Hello, World!"));
    }

    [Test]
    public void CustomClosure_WorksWithAction() {
        int context = 10;
        int calledWith = 0;
        Action<int, int> action = (ctx, arg) => calledWith = ctx + arg;
        var closure = Closure.Custom(context, action);

        closure.Delegate.Invoke(closure.Context, 5);
        Assert.That(calledWith, Is.EqualTo(15));
    }
}