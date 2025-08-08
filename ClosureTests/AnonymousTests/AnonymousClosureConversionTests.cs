using Closures;

namespace ClosureTests.AnonymousTests;

[TestFixture]
public class AnonymousClosureConversionTests {
    // Normal Actions
    [Test]
    public void ClosureActionAsAnonymousAndBack() {
        int expected = 10;
        int received = 0;
        
        var closureAction = Closure.Action(expected, context => received = context);
        var anonymous = closureAction.AsAnonymous();
        anonymous.AsClosureAction<int>().Invoke();
        
        Assert.Multiple(() => {
            Assert.That(anonymous.Is<ClosureAction<int>>(), Is.True, "Anonymous closure should be a ClosureAction<int>.");
            Assert.That(received, Is.EqualTo(expected), "Anonymous closure did not receive the expected context value.");
        });
    }

    [Test]
    public void ClosureActionWithArgAsAnonymousAndBack() {
        int context = 10;
        int arg = 5;
        int expected = context + arg; // context + arg
        int received = 0;

        var closureAction = Closure.Action(context, (int c, int a) => received = c + a);
        var anonymous = closureAction.AsAnonymous();
        anonymous.AsClosureAction<int, int>().Invoke(arg);
        
        Assert.Multiple(() => {
            Assert.That(anonymous.Is<ClosureAction<int, int>>(), Is.True, "Anonymous closure should be a ClosureAction<int, int>.");
            Assert.That(received, Is.EqualTo(expected), "Anonymous closure did not receive the expected context and argument values.");
        });
    }
    
    [Test]
    public void ClosureRefActionAsAnonymousAndBack() {
        int context = 10;
        int arg = 5;
        int expected = context + arg; // context + arg
        int received = 0;

        var closureAction = Closure.Action(context, (int c, ref int a) => received = c + a);
        var anonymous = closureAction.AsAnonymous();
        anonymous.AsClosureRefAction<int, int>().Invoke(arg);
        
        Assert.Multiple(() => {
            Assert.That(anonymous.Is<ClosureRefAction<int, int>>(), Is.True, "Anonymous closure should be a ClosureAction<int, int>.");
            Assert.That(received, Is.EqualTo(expected), "Anonymous closure did not receive the expected context and argument values.");
        });
    }
    
    // Mutating Actions
    [Test]
    public void MutatingClosureActionAsAnonymousAndBack() {
        int expected = 10;
        int received = 0;

        var closure = Closure.Action(expected, (ref int c) => received = c);
        var anonymous = closure.AsAnonymous();
        anonymous.AsMutatingClosureAction<int>().Invoke();

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<MutatingClosureAction<int>>(), Is.True, "Anonymous closure should be a MutatingClosureAction<int>.");
            Assert.That(received, Is.EqualTo(expected), "MutatingClosureAction did not mutate context as expected.");
        });
    }

    [Test]
    public void MutatingClosureActionWithArgAsAnonymousAndBack() {
        int context = 10;
        int arg = 7;
        int expected = context + arg;
        int received = 0;

        var closure = Closure.Action(context, (ref int c, int a) => received = c + a);
        var anonymous = closure.AsAnonymous();
        anonymous.AsMutatingClosureAction<int, int>().Invoke(arg);

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<MutatingClosureAction<int, int>>(), Is.True, "Anonymous closure should be a MutatingClosureAction<int, int>.");
            Assert.That(received, Is.EqualTo(expected), "MutatingClosureActionWithArg did not mutate context as expected.");
        });
    }

    [Test]
    public void MutatingClosureRefActionAsAnonymousAndBack() {
        int context = 10;
        int arg = 3;
        int expected = context + arg;
        int received = 0;

        var closure = Closure.Action(context, (ref int c, ref int a) => received = c + a);
        var anonymous = closure.AsAnonymous();
        anonymous.AsMutatingClosureRefAction<int, int>().Invoke(ref arg);

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<MutatingClosureRefAction<int, int>>(), Is.True, "Anonymous closure should be a MutatingClosureRefAction<int, int>.");
            Assert.That(received, Is.EqualTo(expected), "MutatingClosureRefAction did not mutate context as expected.");
        });
    }
    
    // Normal Funcs
    [Test]
    public void ClosureFuncAsAnonymousAndBack() {
        int expected = 10;
        int received = 0;

        var closureFunc = Closure.Func(expected, context => context);
        var anonymous = closureFunc.AsAnonymous();
        received = anonymous.AsClosureFunc<int, int>().Invoke();

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<ClosureFunc<int, int>>(), Is.True, "Anonymous closure should be a ClosureFunc<int>.");
            Assert.That(received, Is.EqualTo(expected), "Anonymous closure did not receive the expected context value.");
        });
    }

    [Test]
    public void ClosureFuncWithArgAsAnonymousAndBack() {
        int context = 10;
        int arg = 5;
        int expected = context + arg; // context + arg
        int received = 0;

        var closureFunc = Closure.Func(context, (int c, int a) => c + a);
        var anonymous = closureFunc.AsAnonymous();
        received = anonymous.AsClosureFunc<int, int, int>().Invoke(arg);

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<ClosureFunc<int, int, int>>(), Is.True, "Anonymous closure should be a ClosureFunc<int, int>.");
            Assert.That(received, Is.EqualTo(expected), "Anonymous closure did not receive the expected context and argument values.");
        });
    }

    [Test]
    public void ClosureRefFuncAsAnonymousAndBack() {
        int context = 10;
        int arg = 5;
        int expected = context + arg; // context + arg
        int received = 0;

        var closureFunc = Closure.Func(context, (int c, ref int a) => c + a);
        var anonymous = closureFunc.AsAnonymous();
        received = anonymous.AsClosureRefFunc<int, int, int>().Invoke(arg);

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<ClosureRefFunc<int, int, int>>(), Is.True, "Anonymous closure should be a ClosureRefFunc<int, int>.");
            Assert.That(received, Is.EqualTo(expected), "Anonymous closure did not receive the expected context and argument values.");
        });
    }
    
    // Mutating Funcs
    [Test]
    public void MutatingClosureFuncAsAnonymousAndBack() {
        int expected = 10;
        int received = 0;

        var closure = Closure.Func(expected, (ref int c) => received = c);
        var anonymous = closure.AsAnonymous();
        received = anonymous.AsMutatingClosureFunc<int, int>().Invoke();

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<MutatingClosureFunc<int, int>>(), Is.True,
                "Anonymous closure should be a MutatingClosureFunc<int>.");
            Assert.That(received, Is.EqualTo(expected), "MutatingClosureFunc did not mutate context as expected.");
        });
    }

    [Test]
    public void MutatingClosureFuncWithArgAsAnonymousAndBack() {
        int context = 10;
        int arg = 7;
        int expected = context + arg;
        int received = 0;

        var closure = Closure.Func(context, (ref int c, int a) => received = c + a);
        var anonymous = closure.AsAnonymous();
        received = anonymous.AsMutatingClosureFunc<int, int, int>().Invoke(arg);

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<MutatingClosureFunc<int, int, int>>(), Is.True,
                "Anonymous closure should be a MutatingClosureFunc<int, int>.");
            Assert.That(received, Is.EqualTo(expected),
                "MutatingClosureFuncWithArg did not mutate context as expected.");
        });
    }

    [Test]
    public void MutatingClosureRefFuncAsAnonymousAndBack() {
        int context = 10;
        int arg = 3;
        int expected = context + arg;
        int received = 0;

        var closure = Closure.Func(context, (ref int c, ref int a) => received = c + a);
        var anonymous = closure.AsAnonymous();
        received = anonymous.AsMutatingClosureRefFunc<int, int, int>().Invoke(ref arg);

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<MutatingClosureRefFunc<int, int, int>>(), Is.True,
                "Anonymous closure should be a MutatingClosureRefFunc<int, int>.");
            Assert.That(received, Is.EqualTo(expected),
                "MutatingClosureRefFunc did not mutate context as expected.");
        });
    }
    
    // Wrong types
    [Test]
    public void ClosureActionAsAnonymousWrongType_Throws() {
        var closureAction = Closure.Action(10, (int c) => { });
        var anonymous = closureAction.AsAnonymous();

        Assert.Multiple(() => {
            Assert.Throws<InvalidCastException>(() => { anonymous.AsClosureAction<string>(); },
                "Should throw InvalidCastException when trying to convert to wrong type.");
            Assert.Throws<InvalidCastException>(() => { anonymous.AsMutatingClosureAction<int>(); },
                "Should throw InvalidCastException when trying to convert to wrong type with argument.");
        });
    }
    
    [Test]
    public void ClosureActionAsAnonymousIsWrongType_ReturnsFalse() {
        var closureAction = Closure.Action(10, (int c) => { });
        var anonymous = closureAction.AsAnonymous();

        Assert.Multiple(() => {
            Assert.That(anonymous.Is<ClosureAction<string>>(), Is.False,
                "Anonymous closure should not be a ClosureAction<string>.");
            Assert.That(anonymous.Is<MutatingClosureAction<int>>(), Is.False,
                "Anonymous closure should not be a MutatingClosureAction<int>.");
        });
    }
}